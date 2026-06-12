using FluentValidation;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Requests;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.AddMultipleShoppingListItems;

public class AddMultipleShoppingListItemsCommandValidator : AbstractValidator<AddMultipleShoppingListItemsCommand>
{
    public AddMultipleShoppingListItemsCommandValidator()
    {
        RuleFor(c => c.Request.Items)
            .NotEmpty().WithMessage("Alışveriş listesine eklenecek en az bir malzeme olmalıdır.");

        RuleForEach(c => c.Request.Items).SetValidator(new AddShoppingListItemRequestValidator());
    }
}

public class AddShoppingListItemRequestValidator : AbstractValidator<AddShoppingListItemRequest>
{
    public AddShoppingListItemRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Miktar sıfırdan büyük olmalıdır.");

        RuleFor(x => x.Unit)
            .IsInEnum().WithMessage("Geçerli bir birim seçmelisiniz.");
    }
}
