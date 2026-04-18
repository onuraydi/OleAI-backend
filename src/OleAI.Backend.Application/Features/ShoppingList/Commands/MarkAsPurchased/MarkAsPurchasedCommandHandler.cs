using MediatR;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.MarkAsPurchased;

public class MarkAsPurchasedCommandHandler : IRequestHandler<MarkAsPurchasedCommand, string>
{
    private readonly IShoppingListItemRepository _shoppingListItemRepository;
    private readonly ShoppingListBusinessRules _businessRules;

    public MarkAsPurchasedCommandHandler(
        IShoppingListItemRepository shoppingListItemRepository,
        ShoppingListBusinessRules businessRules)
    {
        _shoppingListItemRepository = shoppingListItemRepository;
        _businessRules = businessRules;
    }

    public async Task<string> Handle(MarkAsPurchasedCommand command, CancellationToken cancellationToken)
    {
        var item = await _businessRules.GetItemOrThrow(command.ItemId, command.UserId, cancellationToken);

        item.MarkAsPurchased();

        await _shoppingListItemRepository.UpdateAsync(item, cancellationToken);

        return ShoppingListMessages.ItemMarkedAsPurchased;
    }
}
