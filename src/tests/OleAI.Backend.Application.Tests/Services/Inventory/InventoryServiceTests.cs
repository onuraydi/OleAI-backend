using FluentAssertions;
using OleAI.Backend.Application.Services.Inventory;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Tests.Services.Inventory;

public class InventoryServiceTests
{
    private readonly InventoryService _service = new();

    [Fact]
    public void DeductIngredientsFromPantry_WhenAmountFallsWithinTolerance_ShouldAddIngredientToShoppingList()
    {
        var ingredientId = Guid.NewGuid();
        var pantryItem = UserIngredient.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), ingredientId, 1.0000000001d, MeasurementUnit.Gram);
        var recipeIngredient = new RecipeIngredient(Guid.NewGuid(), Guid.NewGuid(), ingredientId, 1m, 1);

        var result = _service.DeductIngredientsFromPantry([pantryItem], [recipeIngredient]);

        result.Should().ContainSingle().Which.Should().Be(ingredientId);
        pantryItem.Amount.Should().BeApproximately(0, 0.000001d);
    }

    [Fact]
    public void DeductIngredientsFromPantry_WhenAmountRemainsAboveTolerance_ShouldNotAddIngredientToShoppingList()
    {
        var ingredientId = Guid.NewGuid();
        var pantryItem = UserIngredient.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), ingredientId, 1.01d, MeasurementUnit.Gram);
        var recipeIngredient = new RecipeIngredient(Guid.NewGuid(), Guid.NewGuid(), ingredientId, 1m, 1);

        var result = _service.DeductIngredientsFromPantry([pantryItem], [recipeIngredient]);

        result.Should().BeEmpty();
        pantryItem.Amount.Should().BeApproximately(0.01d, 0.000001d);
    }
}
