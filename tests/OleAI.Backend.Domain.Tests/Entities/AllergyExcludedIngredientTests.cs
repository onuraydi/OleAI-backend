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

        // Act
        var entity = new AllergyExcludedIngredient
        {
            AllergyRestrictionId = allergyId,
            IngredientId = ingredientId
        };

        // Assert
        entity.AllergyRestrictionId.Should().Be(allergyId);
        entity.IngredientId.Should().Be(ingredientId);
    }
}
