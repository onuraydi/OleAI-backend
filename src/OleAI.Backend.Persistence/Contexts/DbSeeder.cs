using System.Text.Json;
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

            // 1. Kategoriler (Genişletilmiş 20 Kategori)
            if (!await context.Categories.AnyAsync())
            {
                var categories = new Dictionary<string, Category>
                {
                    { "Sebzeler", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000001"), "Sebzeler", "Carrot") },
                    { "Mantarlar", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000021"), "Mantarlar", "Mushroom") },
                    { "Meyveler", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000002"), "Meyveler", "Apple") },
                    { "Kırmızı Et", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000003"), "Kırmızı Et", "Beef") },
                    { "Kümes Hayvanları", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000022"), "Kümes Hayvanları", "Drumstick") },
                    { "Şarküteri ve İşlenmiş Etler", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000023"), "Şarküteri ve İşlenmiş Etler", "Sausage") },
                    { "Deniz Ürünleri", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000004"), "Deniz Ürünleri", "Fish") },
                    { "Süt ve Süt Ürünleri", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000005"), "Süt ve Süt Ürünleri", "Cheese") },
                    { "Bitkisel Sütler ve Alternatifler", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000024"), "Bitkisel Sütler ve Alternatifler", "LeafyGreen") },
                    { "Tahıllar ve Unlar", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000006"), "Tahıllar ve Unlar", "Wheat") },
                    { "Bakliyatlar", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000025"), "Bakliyatlar", "Bean") },
                    { "Kuruyemişler ve Tohumlar", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000007"), "Kuruyemişler ve Tohumlar", "Nut") },
                    { "Baharatlar ve Taze Otlar", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000008"), "Baharatlar ve Taze Otlar", "Leaf") },
                    { "Sıvı ve Katı Yağlar", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000009"), "Sıvı ve Katı Yağlar", "Droplet") },
                    { "Soslar, Salçalar ve Çeşniler", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000010"), "Soslar, Salçalar ve Çeşniler", "Bottle") },
                    { "Tatlandırıcılar ve Şuruplar", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000026"), "Tatlandırıcılar ve Şuruplar", "Sugar") },
                    { "Unlu Mamuller", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000011"), "Unlu Mamuller", "Croissant") },
                    { "Çikolata ve Kakao", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000027"), "Çikolata ve Kakao", "Cookie") },
                    { "Hazır ve Konserve Gıdalar", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000028"), "Hazır ve Konserve Gıdalar", "Archive") },
                    { "İçecekler", new Category(Guid.Parse("C0000000-0000-0000-0000-000000000012"), "İçecekler", "CupSoda") }
                };
                await context.Categories.AddRangeAsync(categories.Values);
                await context.SaveChangesAsync();
            }

            // 2. Malzemeler
            if (!await context.Ingredients.AnyAsync())
            {
                var categoryMap = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);
                var ingredients = new List<Ingredient>();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var seedCategories = JsonSerializer.Deserialize<List<SeederCategoryDto>>(DbSeederData.IngredientsJson, options);

                if (seedCategories != null)
                {
                    foreach (var cat in seedCategories)
                    {
                        if (categoryMap.TryGetValue(cat.CategoryName, out var catId))
                        {
                            foreach (var ing in cat.Ingredients)
                            {
                                if (Enum.TryParse<MeasurementCategory>(ing.MeasurementCategory, out var measurement))
                                {
                                    ingredients.Add(new Ingredient(Guid.NewGuid(), ing.Name, catId, measurement));
                                }
                            }
                        }
                    }
                }

                await context.Ingredients.AddRangeAsync(ingredients);
                await context.SaveChangesAsync();
            }

            var securitySettings = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<OleAI.Backend.Application.Features.Auth.Constants.SecuritySettings>>().Value;

            // 3. Kullanıcılar ve Dolaplar (Pantry)
            var users = await context.Users.ToListAsync();
            if (!users.Any())
            {
                var adminUser = new User { Id = Guid.NewGuid(), Username = "admin", FirstName = "System", LastName = "Admin", Email = "admin@oleai.com", IsActive = true, IsEmailVerified = true };

                adminUser.UserOperationClaims.Add(new UserOperationClaim
                {
                    OperationClaimId = securitySettings.AdminRoleId
                });

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
                users.Add(adminUser);
            }

            // Senin özel kullanıcın varsa ona da Admin yetkisi verelim (Eğer henüz verilmediyse)
            var targetUserId = Guid.Parse("ce85ac47-db7b-4d4a-b9bf-2971f644fcea");
            var myUser = await context.Users.Include(u => u.UserOperationClaims).FirstOrDefaultAsync(u => u.Id == targetUserId);
            if (myUser != null && !myUser.UserOperationClaims.Any(c => c.OperationClaimId == securitySettings.AdminRoleId))
            {
                myUser.UserOperationClaims.Add(new UserOperationClaim { OperationClaimId = securitySettings.AdminRoleId });
                await context.SaveChangesAsync();
            }

            // TÜM kullanıcılara malzeme ekle (Senin kullanıcın dahil)
            var allIngredients = await context.Ingredients.ToListAsync();

            // Senin kullanıcı ID'ni de özel olarak garantiye alalım
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

            // 4. Tarifler
            try
            {
                if (!await context.Recipes.AnyAsync())
                {
                    var creatorId = users.First().Id;
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var seedRecipes = JsonSerializer.Deserialize<List<SeederRecipeDto>>(DbSeederData.RecipesJson, options);

                    if (seedRecipes != null)
                    {
                        var recipesToInsert = new List<Recipe>();
                        foreach (var sr in seedRecipes)
                        {
                            _ = Enum.TryParse<RecipeDifficulty>(sr.Difficulty, out var diff);
                            var recipe = Recipe.Create(creatorId, sr.Title, sr.Description, diff, sr.Servings, sr.PrepTime, sr.CookTime, RecipeVisibility.Private);

                            foreach (var ing in sr.Ingredients)
                            {
                                var matchedIng = allIngredients.FirstOrDefault(i => i.Name.Equals(ing.Name, StringComparison.OrdinalIgnoreCase) || i.Name.Contains(ing.Name, StringComparison.OrdinalIgnoreCase));
                                if (matchedIng != null)
                                {
                                    MeasurementUnit unit = ing.Unit switch
                                    {
                                        "Adet" => MeasurementUnit.Piece,
                                        "Gram" => MeasurementUnit.Gram,
                                        "Mililitre" => MeasurementUnit.Milliliter,
                                        "Diş" => MeasurementUnit.Piece,
                                        "Demet" => MeasurementUnit.Bunch,
                                        "Çay Kaşığı" => MeasurementUnit.Pinch,
                                        "Tatlı Kaşığı" => MeasurementUnit.Piece,
                                        "Yemek Kaşığı" => MeasurementUnit.Piece,
                                        "Su Bardağı" => MeasurementUnit.Milliliter,
                                        _ => MeasurementUnit.Other
                                    };
                                    recipe.AddIngredient(matchedIng.Id, null, ing.Amount, (int)unit);
                                }
                            }

                            for (int stepIdx = 0; stepIdx < sr.Steps.Count; stepIdx++)
                            {
                                recipe.AddStep(stepIdx + 1, sr.Steps[stepIdx]);
                            }

                            // Match Categories
                            if (sr.Categories != null && sr.Categories.Any())
                            {
                                var catIds = new List<int>();
                                foreach (var cName in sr.Categories)
                                {
                                    var catMatch = await context.RecipeCategories.FirstOrDefaultAsync(rc => rc.Name == cName);
                                    if (catMatch != null) catIds.Add(catMatch.Id);
                                }
                                recipe.SyncCategories(catIds);
                            }

                            // Match Tags
                            if (sr.Tags != null && sr.Tags.Any())
                            {
                                var tagIds = new List<int>();
                                foreach (var tName in sr.Tags)
                                {
                                    var tagMatch = await context.Tags.FirstOrDefaultAsync(t => t.Name == tName);
                                    if (tagMatch != null) tagIds.Add(tagMatch.Id);
                                }
                                recipe.SyncTags(tagIds);
                            }

                            recipesToInsert.Add(recipe);
                        }
                        await context.Recipes.AddRangeAsync(recipesToInsert);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Recipe seed error: {ex.InnerException?.Message ?? ex.Message}");
            }

            // 5. Ekstra Veriler (Alerjiler, Diyetler, Tarif Kategorileri, Etiketler)
            var extraDataOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var extraData = JsonSerializer.Deserialize<ExtraSeedDataDto>(DbSeederData.ExtraSeedDataJson, extraDataOptions);
            if (extraData != null)
            {
                if (!await context.AllergyRestrictions.AnyAsync())
                {
                    var allergies = extraData.Allergies.Select(a => new AllergyRestriction { Id = Guid.NewGuid(), Name = a.Name, Description = a.Description, IconName = a.IconName }).ToList();
                    await context.AllergyRestrictions.AddRangeAsync(allergies);
                }

                if (!await context.DietaryPreferences.AnyAsync())
                {
                    var diets = extraData.Dietary.Select(d => new DietaryPreference { Id = Guid.NewGuid(), Name = d.Name, Description = d.Description, IconName = d.IconName }).ToList();
                    await context.DietaryPreferences.AddRangeAsync(diets);
                }

                if (!await context.RecipeCategories.AnyAsync())
                {
                    var recCats = extraData.RecipeCategories.Select(c => new RecipeCategory(0, c.Name, c.IconName)).ToList();
                    await context.RecipeCategories.AddRangeAsync(recCats);
                }

                if (!await context.Tags.AnyAsync())
                {
                    var tags = extraData.Tags.Select(t => new Tag(0, t.Name)).ToList();
                    await context.Tags.AddRangeAsync(tags);
                }

                await context.SaveChangesAsync();
            }
        }
    }

    public class SeederCategoryDto
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<SeederIngredientDto> Ingredients { get; set; }
    }

    public class SeederIngredientDto
    {
        public string Name { get; set; }
        public string MeasurementCategory { get; set; }
    }

    public class SeederRecipeDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Difficulty { get; set; }
        public int Servings { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public List<SeederRecipeIngredientDto> Ingredients { get; set; }
        public List<string> Steps { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
    }

    public class SeederRecipeIngredientDto
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Unit { get; set; }
    }

    public class ExtraSeedDataDto
    {
        public List<AllergyDto> Allergies { get; set; }
        public List<DietaryDto> Dietary { get; set; }
        public List<RecipeCategoryDto> RecipeCategories { get; set; }
        public List<TagDto> Tags { get; set; }
    }

    public class AllergyDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconName { get; set; }
    }

    public class DietaryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconName { get; set; }
    }

    public class RecipeCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconName { get; set; }
    }

    public class TagDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
