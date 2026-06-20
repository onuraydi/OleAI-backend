using FluentValidation.TestHelper;
using OleAI.Backend.Application.Features.ShoppingList.Commands.AddMultipleShoppingListItems;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Requests;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ShoppingList.Commands.AddMultipleShoppingListItems;

public class AddMultipleShoppingListItemsCommandValidatorTests
{
    private readonly AddMultipleShoppingListItemsCommandValidator _validator;

    public AddMultipleShoppingListItemsCommandValidatorTests()
    {
        _validator = new AddMultipleShoppingListItemsCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Items_Are_Empty()
    {
        var command = new AddMultipleShoppingListItemsCommand
        {
            Request = new AddMultipleShoppingListItemsRequest { Items = new List<AddShoppingListItemRequest>() }
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Request.Items);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Items_Are_Valid()
    {
        var command = new AddMultipleShoppingListItemsCommand
        {
            Request = new AddMultipleShoppingListItemsRequest
            {
                Items = new List<AddShoppingListItemRequest>
                {
                    new AddShoppingListItemRequest { IngredientId = Guid.NewGuid(), Quantity = 2, Unit = MeasurementUnit.Piece }
                }
            }
        };

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.Request.Items);
    }
}
