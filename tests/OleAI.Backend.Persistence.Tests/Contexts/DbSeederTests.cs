using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using Xunit;

namespace OleAI.Backend.Persistence.Tests.Contexts;

public class DbSeederTests
{
    private static ServiceProvider BuildServiceProvider(string dbName)
    {
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase(dbName));
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task SeedAsync_WhenDatabaseIsEmpty_ShouldSeedDefaultCategories()
    {
        // Arrange
        using var provider = BuildServiceProvider(Guid.NewGuid().ToString());

        // Act
        await DbSeeder.SeedAsync(provider);

        // Assert
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var categories = await context.Set<Category>().ToListAsync();
        categories.Should().HaveCount(8);
        categories.Select(c => c.Name).Should().Contain(new[]
        {
            "Sebzeler", "Meyveler", "Süt ve Süt Ürünleri", "Kuru Bakliyat", "Baharatlar", "Yağlar", "Et ve Tavuk", "Diğer"
        });
    }

    [Fact]
    public async Task SeedAsync_WhenCategoriesAlreadyExist_ShouldNotSeedAgain()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var provider = BuildServiceProvider(dbName);

        using (var scope = provider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Set<Category>().AddAsync(new Category(Guid.NewGuid(), "Mevcut"));
            await context.SaveChangesAsync();
        }

        // Act
        await DbSeeder.SeedAsync(provider);

        // Assert
        using var assertScope = provider.CreateScope();
        var assertContext = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var categories = await assertContext.Set<Category>().ToListAsync();

        categories.Should().HaveCount(1);
        categories[0].Name.Should().Be("Mevcut");
    }
}
