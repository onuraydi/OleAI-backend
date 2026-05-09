using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;
using Xunit;

namespace OleAI.Backend.Persistence.Tests.Repositories;

public class AllergyRestrictionRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistAllergyRestriction()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new AllergyRestrictionRepository(context);
        var restriction = new AllergyRestriction
        {
            Id = Guid.NewGuid(),
            Name = "Gluten Allergy",
            Description = "No Gluten"
        };

        // Act
        await repository.AddAsync(restriction);

        // Assert
        var saved = await context.AllergyRestrictions.FindAsync(restriction.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Gluten Allergy");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveAllergyRestriction()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new AllergyRestrictionRepository(context);
        var restriction = new AllergyRestriction
        {
            Id = Guid.NewGuid(),
            Name = "Nut Allergy",
            Description = "No nuts"
        };

        await context.AllergyRestrictions.AddAsync(restriction);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(restriction, permanent: true);

        // Assert
        (await context.AllergyRestrictions.FindAsync(restriction.Id)).Should().BeNull();
    }
}
