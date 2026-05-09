using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;
using Xunit;

namespace OleAI.Backend.Persistence.Tests.Repositories;

public class UserAllergyRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldPersistMultipleUserAllergies()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserAllergyRepository(context);
        var userId = Guid.NewGuid();
        
        var allergies = new List<UserAllergy>
        {
            new UserAllergy { UserId = userId, AllergyRestrictionId = Guid.NewGuid() },
            new UserAllergy { UserId = userId, AllergyRestrictionId = Guid.NewGuid() }
        };

        // Act
        await repository.AddRangeAsync(allergies);

        // Assert
        var saved = await context.UserAllergies.Where(u => u.UserId == userId).ToListAsync();
        saved.Count.Should().Be(2);
    }
}
