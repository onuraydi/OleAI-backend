using MediatR;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Requests;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Responses;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.AddShoppingListItem;

public class AddShoppingListItemCommand : IRequest<ShoppingListItemResponse>
{
    public AddShoppingListItemRequest Request { get; set; } = default!;
}
