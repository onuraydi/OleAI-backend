using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;
using Xunit;

namespace OleAI.Backend.Persistence.Tests.Repositories;

public class DietaryPreferenceRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistDietaryPreference()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new DietaryPreferenceRepository(context);
        var preference = new DietaryPreference
        {
            Id = Guid.NewGuid(),
            Name = "Vegan"
        };

        // Act
        await repository.AddAsync(preference);

        // Assert
        var saved = await context.DietaryPreferences.FindAsync(preference.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Vegan");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveDietaryPreference()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new DietaryPreferenceRepository(context);
        var preference = new DietaryPreference
        {
            Id = Guid.NewGuid(),
            Name = "Keto"
        };

        await context.DietaryPreferences.AddAsync(preference);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(preference, permanent: true);

        // Assert
        (await context.DietaryPreferences.FindAsync(preference.Id)).Should().BeNull();
    }
}
