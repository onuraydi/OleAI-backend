using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;
using Xunit;

namespace OleAI.Backend.Persistence.Tests.Repositories;

public class UserDislikedIngredientRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldPersistMultipleDislikes()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserDislikedIngredientRepository(context);
        var userId = Guid.NewGuid();

        var dislikes = new List<UserDislikedIngredient>
        {
            new UserDislikedIngredient { Id = Guid.NewGuid(), UserId = userId, IngredientId = Guid.NewGuid() },
            new UserDislikedIngredient { Id = Guid.NewGuid(), UserId = userId, IngredientId = Guid.NewGuid() }
        };

        // Act
        await repository.AddRangeAsync(dislikes);

        // Assert
        var saved = await context.UserDislikedIngredients.Where(u => u.UserId == userId).ToListAsync();
        saved.Count.Should().Be(2);
    }

}
