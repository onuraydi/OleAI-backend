using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Persistence.Contexts
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // 1. Kategoriler
            if (!await context.Categories.AnyAsync())
            {
                var categories = new Dictionary<string, Category>
                {
                    { "Sebze", new Category(Guid.NewGuid(), "Sebzeler") },
                    { "Et", new Category(Guid.NewGuid(), "Et ve Tavuk") },
                    { "Bakliyat", new Category(Guid.NewGuid(), "Kuru Bakliyat") },
                    { "Baharat", new Category(Guid.NewGuid(), "Baharatlar") },
                    { "Sut", new Category(Guid.NewGuid(), "Süt Ürünleri") },
                    { "Yag", new Category(Guid.NewGuid(), "Yağlar") },
                    { "Healthy", new Category(Guid.NewGuid(), "Healthy") }
                };
                await context.Categories.AddRangeAsync(categories.Values);
                await context.SaveChangesAsync();
            }

            // 2. Malzemeler
            if (!await context.Ingredients.AnyAsync())
            {
                var categoryMap = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);
                var ingredients = new List<Ingredient>
                {
                    new Ingredient(Guid.NewGuid(), "Domates", categoryMap["Sebzeler"], MeasurementCategory.Weight),
                    new Ingredient(Guid.NewGuid(), "Biber", categoryMap["Sebzeler"], MeasurementCategory.Weight),
                    new Ingredient(Guid.NewGuid(), "Soğan", categoryMap["Sebzeler"], MeasurementCategory.Weight),
                    new Ingredient(Guid.NewGuid(), "Sarımsak", categoryMap["Sebzeler"], MeasurementCategory.Count),
                    new Ingredient(Guid.NewGuid(), "Tavuk Göğsü", categoryMap["Et ve Tavuk"], MeasurementCategory.Weight),
                    new Ingredient(Guid.NewGuid(), "Kıyma", categoryMap["Et ve Tavuk"], MeasurementCategory.Weight),
                    new Ingredient(Guid.NewGuid(), "Mercimek", categoryMap["Kuru Bakliyat"], MeasurementCategory.Weight),
                    new Ingredient(Guid.NewGuid(), "Makarna", categoryMap["Kuru Bakliyat"], MeasurementCategory.Weight),
                    new Ingredient(Guid.NewGuid(), "Tuz", categoryMap["Baharatlar"], MeasurementCategory.Weight),
                    new Ingredient(Guid.NewGuid(), "Karabiber", categoryMap["Baharatlar"], MeasurementCategory.Weight),
                    new Ingredient(Guid.NewGuid(), "Zeytinyağı", categoryMap["Yağlar"], MeasurementCategory.Volume),
                    new Ingredient(Guid.NewGuid(), "Yoğurt", categoryMap["Süt Ürünleri"], MeasurementCategory.Weight),
                    new Ingredient(Guid.NewGuid(), "Yumurta", categoryMap["Süt Ürünleri"], MeasurementCategory.Count),
                    new Ingredient(Guid.NewGuid(), "Ispanak", categoryMap["Sebzeler"], MeasurementCategory.Weight),
                    new Ingredient(Guid.NewGuid(), "Mantar", categoryMap["Sebzeler"], MeasurementCategory.Weight)
                };
                await context.Ingredients.AddRangeAsync(ingredients);
                await context.SaveChangesAsync();
            }

            // 3. Kullanıcılar ve Dolaplar (Pantry)
            var users = await context.Users.ToListAsync();
            if (!users.Any())
            {
                var adminUser = new User { Id = Guid.NewGuid(), FirstName = "System", LastName = "Admin", Email = "admin@oleai.com", IsActive = true, IsEmailVerified = true };
                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
                users.Add(adminUser);
            }

            // TÜM kullanıcılara malzeme ekle (Senin kullanıcın dahil)
            var allIngredients = await context.Ingredients.ToListAsync();

            // Senin kullanıcı ID'ni de özel olarak garantiye alalım
            var targetUserId = Guid.Parse("ce85ac47-db7b-4d4a-b9bf-2971f644fcea");
            if (!users.Any(u => u.Id == targetUserId))
            {
                // Eğer kullanıcı veritabanında yoksa ama ID'sini biliyorsak, geçici bir kayıt oluşturabiliriz veya 
                // var olanlar üzerinden devam ederiz. Genelde login olduğun için varsın varsayıyoruz.
            }

            foreach (var user in users)
            {
                if (!await context.UserIngredients.AnyAsync(ui => ui.UserId == user.Id) || user.Id == targetUserId)
                {
                    // Senin kullanıcın için dolabı her zaman tazele veya eksikse ekle
                    var currentPantryCount = await context.UserIngredients.CountAsync(ui => ui.UserId == user.Id);
                    if (currentPantryCount < 5)
                    {
                        var someIngredients = allIngredients.OrderBy(x => Guid.NewGuid()).Take(10).ToList();
                        foreach (var ing in someIngredients)
                        {
                            // Mükerrer kaydı önlemek için kontrol
                            if (!await context.UserIngredients.AnyAsync(ui => ui.UserId == user.Id && ui.IngredientId == ing.Id))
                            {
                                var userIng = UserIngredient.CreateStandard(Guid.NewGuid(), user.Id, ing.Id, 1000, MeasurementUnit.Gram);
                                await context.UserIngredients.AddAsync(userIng);
                            }
                        }
                    }
                }
            }
            await context.SaveChangesAsync();

            // 4. Tarifler (Eğer 10'dan az tarif varsa 50 tane daha ekle)
            if (await context.Recipes.CountAsync() < 10)
            {
                var creatorId = users.First().Id;
                var random = new Random();
                var recipes = new List<Recipe>();

                for (int i = 1; i <= 50; i++)
                {
                    var baseIng = allIngredients[random.Next(allIngredients.Count)];
                    var title = i % 3 == 0 ? $"{baseIng.Name} Sote" : i % 2 == 0 ? $"{baseIng.Name} Çorbası" : $"{baseIng.Name} Salatası";
                    var recipe = Recipe.Create(creatorId, title, $"{title} için açıklama metni...", RecipeVisibility.Private);

                    var selectedIngredients = allIngredients.OrderBy(x => random.Next()).Take(random.Next(3, 7)).ToList();
                    foreach (var ing in selectedIngredients)
                    {
                        recipe.AddIngredient(ing.Id, random.Next(1, 500), (int)MeasurementUnit.Gram);
                    }
                    recipe.AddStep(1, "Malzemeleri hazırla.");
                    recipe.AddStep(2, "Hepsini karıştır ve pişir.");
                    recipes.Add(recipe);
                }
                await context.Recipes.AddRangeAsync(recipes);
                await context.SaveChangesAsync();
            }
        }
    }
}
