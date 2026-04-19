using MediatR;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.RemoveShoppingListItem;

public class RemoveShoppingListItemCommandHandler : IRequestHandler<RemoveShoppingListItemCommand, string>
{
    private readonly IShoppingListItemRepository _shoppingListItemRepository;
    private readonly ShoppingListBusinessRules _businessRules;

    public RemoveShoppingListItemCommandHandler(
        IShoppingListItemRepository shoppingListItemRepository,
        ShoppingListBusinessRules businessRules)
    {
        _shoppingListItemRepository = shoppingListItemRepository;
        _businessRules = businessRules;
    }

    public async Task<string> Handle(RemoveShoppingListItemCommand command, CancellationToken cancellationToken)
    {
        var item = await _businessRules.GetItemOrThrow(command.ItemId, command.UserId, cancellationToken);

        await _shoppingListItemRepository.DeleteAsync(item, permanent: true, cancellationToken);

        return ShoppingListMessages.ItemRemovedFromList;
    }
}
