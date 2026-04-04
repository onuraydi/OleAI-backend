using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.Contexts
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (await context.Categories.AnyAsync())
            {
                return;
            }

            var catSebze = new Category(Guid.NewGuid(), "Sebzeler");
            var catMeyve = new Category(Guid.NewGuid(), "Meyveler");
            var catSut = new Category(Guid.NewGuid(), "Süt ve Süt Ürünleri");
            var catBakliyat = new Category(Guid.NewGuid(), "Kuru Bakliyat");
            var catBaharat = new Category(Guid.NewGuid(), "Baharatlar");
            var catYag = new Category(Guid.NewGuid(), "Yağlar");
            var catEt = new Category(Guid.NewGuid(), "Et ve Tavuk");
            var catDiger = new Category(Guid.NewGuid(), "Diğer");

            var categories = new List<Category> { catSebze, catMeyve, catSut, catBakliyat, catBaharat, catYag, catEt, catDiger };
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
    }
}
