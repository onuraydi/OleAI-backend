using OleAI.Backend.Application.Features.ShoppingList.Dtos.Requests;

namespace OleAI.Backend.Application.Features.ShoppingList.Dtos.Requests;

public class AddMultipleShoppingListItemsRequest
{
    public Guid UserId { get; set; }
    public List<AddShoppingListItemRequest> Items { get; set; } = new();
}
