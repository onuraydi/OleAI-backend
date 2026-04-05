using FluentAssertions;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Domain.Tests.Entities;

public class IngredientTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var ingredient = new Ingredient(id, "Domates", categoryId, "adet");

        ingredient.Id.Should().Be(id);
        ingredient.Name.Should().Be("Domates");
        ingredient.CategoryId.Should().Be(categoryId);
        ingredient.BaseUnitType.Should().Be("adet");
    }

    [Fact]
    public void Constructor_ShouldInitializeCollections()
    {
        var ingredient = new Ingredient(Guid.NewGuid(), "Tuz", Guid.NewGuid(), "gram");

        ingredient.Aliases.Should().NotBeNull();
        ingredient.UserIngredients.Should().NotBeNull();
    }
}
