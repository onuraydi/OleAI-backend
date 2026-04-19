using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Domain.Tests.Entities;

public class IngredientTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var ingredient = new Ingredient(id, "Domates", categoryId, MeasurementCategory.Count);

        ingredient.Id.Should().Be(id);
        ingredient.Name.Should().Be("Domates");
        ingredient.CategoryId.Should().Be(categoryId);
        ingredient.UnitFamily.Should().Be(MeasurementCategory.Count);
    }

    [Fact]
    public void Constructor_ShouldInitializeCollections()
    {
        var ingredient = new Ingredient(Guid.NewGuid(), "Tuz", Guid.NewGuid(), MeasurementCategory.Weight);

        ingredient.Aliases.Should().NotBeNull();
        ingredient.UserIngredients.Should().NotBeNull();
        ingredient.ShoppingListItems.Should().NotBeNull();
    }
}
