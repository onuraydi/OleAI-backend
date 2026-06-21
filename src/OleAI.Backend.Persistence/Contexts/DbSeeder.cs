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
