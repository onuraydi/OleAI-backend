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

        var user = new User();
        var ingredient = new Ingredient(Guid.NewGuid(), "Test", Guid.NewGuid());

        // Act
        var entity = new UserDislikedIngredient
        {
            UserId = userId,
            IngredientId = ingredientId,
            User = user,
            Ingredient = ingredient
        };

        // Assert
        entity.UserId.Should().Be(userId);
        entity.IngredientId.Should().Be(ingredientId);
        entity.User.Should().Be(user);
        entity.Ingredient.Should().Be(ingredient);
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
