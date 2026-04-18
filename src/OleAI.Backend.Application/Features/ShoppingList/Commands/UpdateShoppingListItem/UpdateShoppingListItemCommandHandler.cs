using MediatR;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.UpdateShoppingListItem;

public class UpdateShoppingListItemCommandHandler : IRequestHandler<UpdateShoppingListItemCommand, string>
{
    private readonly IShoppingListItemRepository _shoppingListItemRepository;
    private readonly ShoppingListBusinessRules _businessRules;

    public UpdateShoppingListItemCommandHandler(
        IShoppingListItemRepository shoppingListItemRepository,
        ShoppingListBusinessRules businessRules)
    {
        _shoppingListItemRepository = shoppingListItemRepository;
        _businessRules = businessRules;
    }

    public async Task<string> Handle(UpdateShoppingListItemCommand command, CancellationToken cancellationToken)
    {
        // Varlık + sahiplik kontrolü; satın alınmış ürünün miktarı domain içinde guard edilir
        var item = await _businessRules.GetItemOrThrow(command.ItemId, command.UserId, cancellationToken);

        // Domain davranışı: IsPurchased=true ise BusinessException fırlatır
        item.UpdateQuantity(command.NewQuantity, command.NewUnit);

        await _shoppingListItemRepository.UpdateAsync(item, cancellationToken);

        return ShoppingListMessages.ItemUpdated;
    }
}
