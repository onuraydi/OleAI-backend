using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class ShoppingListItemTests
{
    [Fact]
    public void CreateStandard_ShouldInitializeCorrectly()
    {
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();

        var item = ShoppingListItem.CreateStandard(userId, ingredientId, 10, OleAI.Backend.Domain.Enums.MeasurementUnit.Gram);

        item.UserId.Should().Be(userId);
        item.IngredientId.Should().Be(ingredientId);
        item.Quantity.Should().Be(10);
        item.Unit.Should().Be(OleAI.Backend.Domain.Enums.MeasurementUnit.Gram);
        item.IsPurchased.Should().BeFalse();
    }

    [Fact]
    public void MarkAsPurchased_ShouldSetIsPurchased()
    {
        var item = ShoppingListItem.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), 10, OleAI.Backend.Domain.Enums.MeasurementUnit.Gram);
        item.MarkAsPurchased();
        item.IsPurchased.Should().BeTrue();
    }
}
