using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class DietaryPreferenceExcludedIngredientTests
{
    [Fact]
    public void ObjectInitializer_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dietId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();

        var diet = new DietaryPreference();
        var ingredient = new Ingredient(Guid.NewGuid(), "Test", Guid.NewGuid());

        // Act
        var entity = new DietaryPreferenceExcludedIngredient
        {
            DietaryPreferenceId = dietId,
            IngredientId = ingredientId,
            DietaryPreference = diet,
            Ingredient = ingredient
        };

        // Assert
        entity.DietaryPreferenceId.Should().Be(dietId);
        entity.IngredientId.Should().Be(ingredientId);
        entity.DietaryPreference.Should().Be(diet);
        entity.Ingredient.Should().Be(ingredient);
    }
}
