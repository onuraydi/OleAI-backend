using MediatR;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Features.ShoppingList.Events;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.MarkAsPurchased;

public class MarkAsPurchasedCommandHandler : IRequestHandler<MarkAsPurchasedCommand, string>
{
    private readonly IShoppingListItemRepository _shoppingListItemRepository;
    private readonly ShoppingListBusinessRules _businessRules;
    private readonly IMediator _mediator;

    public MarkAsPurchasedCommandHandler(
        IShoppingListItemRepository shoppingListItemRepository,
        ShoppingListBusinessRules businessRules,
        IMediator mediator)
    {
        _shoppingListItemRepository = shoppingListItemRepository;
        _businessRules = businessRules;
        _mediator = mediator;
    }

    public async Task<string> Handle(MarkAsPurchasedCommand command, CancellationToken cancellationToken)
    {
        var item = await _businessRules.GetItemOrThrow(command.ItemId, command.UserId, cancellationToken);

        // Domain davranışı — zaten satın alınmışsa BusinessException fırlatır
        item.MarkAsPurchased();

        await _shoppingListItemRepository.UpdateAsync(item, cancellationToken);

        // Cross-module event: PantryAdditionEventHandler dolaba otomatik ekleyecek
        await _mediator.Publish(ItemPurchasedEventFactory.Create(item), cancellationToken);

        return ShoppingListMessages.ItemMarkedAsPurchased;
    }
}
