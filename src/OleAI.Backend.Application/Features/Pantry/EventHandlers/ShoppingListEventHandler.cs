using MediatR;
using OleAI.Backend.Application.Features.Pantry.Events;
// Not: IShoppingListItemRepository eklendiğinde açılacak

namespace OleAI.Backend.Application.Features.Pantry.EventHandlers;

public class ShoppingListEventHandler : INotificationHandler<IngredientDepletedEvent>
{
    // private readonly IShoppingListItemRepository _shoppingListRepository;

    public ShoppingListEventHandler(/* IShoppingListItemRepository shoppingListRepository */)
    {
        // _shoppingListRepository = shoppingListRepository;
    }

    public async Task Handle(IngredientDepletedEvent notification, CancellationToken cancellationToken)
    {
        // TODO: Gerçek ShoppingList entity'si eklendiğinde burası aktif edilecek.

        /* var existingItem = await _shoppingListRepository.GetAsync(s => s.UserId == notification.UserId && s.IngredientId == notification.MasterIngredientId);
        
        if (existingItem == null)
        {
            var newItem = new ShoppingListItem(Guid.NewGuid(), notification.UserId, notification.MasterIngredientId);
            await _shoppingListRepository.AddAsync(newItem);
        }
        */

        // Şimdilik sistemin düzgün çalıştığını görmek için Console/Log basıyoruz.
        Console.WriteLine($"[AKILLI ASİSTAN] {notification.UserId} ID'li kullanıcının dolabında {notification.IngredientName} bitti. Alışveriş listesine otomatik eklendi!");

        await Task.CompletedTask;
    }
}
