using MediatR;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Features.ShoppingList.Events;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.BulkMarkAsPurchased;

public class BulkMarkAsPurchasedCommandHandler : IRequestHandler<BulkMarkAsPurchasedCommand, string>
{
    private readonly IShoppingListItemRepository _shoppingListItemRepository;
    private readonly IMediator _mediator;

    public BulkMarkAsPurchasedCommandHandler(
        IShoppingListItemRepository shoppingListItemRepository,
        IMediator mediator)
    {
        _shoppingListItemRepository = shoppingListItemRepository;
        _mediator = mediator;
    }

    public async Task<string> Handle(BulkMarkAsPurchasedCommand command, CancellationToken cancellationToken)
    {
        if (command.ItemIds.Count == 0)
            throw new BusinessException("En az bir ürün seçmelisiniz.");

        var options = new Core.Repositories.Options.QueryOptions<Domain.Entities.ShoppingListItem>
        {
            Predicate = s => command.ItemIds.Contains(s.Id) &&
                             s.UserId == command.UserId &&
                             !s.IsPurchased,
            EnableTracking = true
        };

        var paginate = await _shoppingListItemRepository.GetListAsync(options, cancellationToken);
        var items = paginate.Items;

        if (items.Count == 0)
            throw new NotFoundException(ShoppingListMessages.ItemNotFoundOrUnauthorized);

        foreach (var item in items)
        {
            item.MarkAsPurchased();
        }

        await _shoppingListItemRepository.UpdateRangeAsync(items, cancellationToken);

        foreach (var item in items)
        {
            await _mediator.Publish(ItemPurchasedEventFactory.Create(item), cancellationToken);
        }

        return string.Format(ShoppingListMessages.BulkMarkedAsPurchased, items.Count);
    }
}
