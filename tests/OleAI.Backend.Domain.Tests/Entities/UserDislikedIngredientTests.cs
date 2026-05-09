using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class UserDislikedIngredientTests
{
    [Fact]
    public void Constructor_WithParameters_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();

        // Act
        var dislikedIngredient = new UserDislikedIngredient
        {
            Id = id,
            UserId = userId,
            IngredientId = ingredientId
        };

        // Assert
        dislikedIngredient.Id.Should().Be(id);
        dislikedIngredient.UserId.Should().Be(userId);
        dislikedIngredient.IngredientId.Should().Be(ingredientId);
    }

    [Fact]
    public void Constructor_Default_ShouldInitializeEmpty()
    {
        // Act
        var dislikedIngredient = new UserDislikedIngredient();

        // Assert
        dislikedIngredient.Id.Should().Be(Guid.Empty);
    }
}
