using FluentAssertions;
using OleAI.Backend.Application.Services.Inventory;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Services.Inventory;

public class InventoryServiceTests
{
    [Fact]
    public void DeductIngredientsFromPantry_WhenUserHasEnough_ShouldDecreaseAmount()
    {
        // Arrange
        var service = new InventoryService();
        var ingredientId = Guid.NewGuid();
        var userIngredient = UserIngredient.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), ingredientId, 100, MeasurementUnit.Gram);
        var recipeIngredient = new RecipeIngredient { IngredientId = ingredientId, Amount = 40 };

        var userIngredients = new List<UserIngredient> { userIngredient };
        var recipeIngredients = new List<RecipeIngredient> { recipeIngredient };

        // Act
        var result = service.DeductIngredientsFromPantry(userIngredients, recipeIngredients);

        // Assert
        userIngredient.Amount.Should().Be(60);
        result.Should().BeEmpty();
    }

    [Fact]
    public void DeductIngredientsFromPantry_WhenUserDepletes_ShouldAddToShoppingList()
    {
        // Arrange
        var service = new InventoryService();
        var ingredientId = Guid.NewGuid();
        var userIngredient = UserIngredient.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), ingredientId, 30, MeasurementUnit.Gram);
        var recipeIngredient = new RecipeIngredient { IngredientId = ingredientId, Amount = 40 };

        var userIngredients = new List<UserIngredient> { userIngredient };
        var recipeIngredients = new List<RecipeIngredient> { recipeIngredient };

        // Act
        var result = service.DeductIngredientsFromPantry(userIngredients, recipeIngredients);

        // Assert
        userIngredient.Amount.Should().Be(0);
        result.Should().Contain(ingredientId);
    }
}
