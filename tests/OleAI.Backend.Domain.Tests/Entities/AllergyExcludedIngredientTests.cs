using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class AllergyExcludedIngredientTests
{
    [Fact]
    public void ObjectInitializer_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var allergyId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();

        var allergy = new AllergyRestriction();
        var ingredient = new Ingredient(Guid.NewGuid(), "Test", Guid.NewGuid());

        // Act
        var entity = new AllergyExcludedIngredient
        {
            AllergyRestrictionId = allergyId,
            IngredientId = ingredientId,
            AllergyRestriction = allergy,
            Ingredient = ingredient
        };

        // Assert
        entity.AllergyRestrictionId.Should().Be(allergyId);
        entity.IngredientId.Should().Be(ingredientId);
        entity.AllergyRestriction.Should().Be(allergy);
        entity.Ingredient.Should().Be(ingredient);
    }
}
