using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OleAI.Backend.Application.Features.Auth.Constants;
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

        var mockSecuritySettings = new SecuritySettings
        {
            AdminRoleId = Guid.NewGuid(),
            DefaultRoleId = Guid.NewGuid()
        };
        services.AddSingleton(Options.Create(mockSecuritySettings));

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
        categories.Should().HaveCount(20);
        categories.Select(c => c.Name).Should().Contain(new[]
        {
            "Sebzeler", "Meyveler", "Kırmızı Et", "Deniz Ürünleri", "Süt ve Süt Ürünleri", "Tahıllar ve Unlar", "Kuruyemişler ve Tohumlar"
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
            var existingCategories = new[]
            {
                "Sebzeler", "Meyveler", "Kırmızı Et", "Deniz Ürünleri", "Süt ve Süt Ürünleri", "Tahıllar ve Unlar", "Kuruyemişler ve Tohumlar"
            };

            foreach (var name in existingCategories)
            {
                await context.Set<Category>().AddAsync(new Category(Guid.NewGuid(), name));
            }
            await context.SaveChangesAsync();
        }

        // Act
        await DbSeeder.SeedAsync(provider);

        // Assert
        using var assertScope = provider.CreateScope();
        var assertContext = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var categories = await assertContext.Set<Category>().ToListAsync();

        categories.Should().HaveCount(7);
        categories.Select(c => c.Name).Should().Contain(new[]
        {
            "Sebzeler", "Meyveler", "Kırmızı Et", "Deniz Ürünleri", "Süt ve Süt Ürünleri", "Tahıllar ve Unlar", "Kuruyemişler ve Tohumlar"
        });
    }
}
