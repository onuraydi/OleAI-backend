using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.ShoppingList.Events;

public static class ItemPurchasedEventFactory
{
    public static ItemPurchasedEvent Create(ShoppingListItem item)
    {
        return new ItemPurchasedEvent
        {
            UserId = item.UserId,
            IngredientId = item.IngredientId,
            Quantity = item.Quantity,
            Unit = item.Unit
        };
    }
}
