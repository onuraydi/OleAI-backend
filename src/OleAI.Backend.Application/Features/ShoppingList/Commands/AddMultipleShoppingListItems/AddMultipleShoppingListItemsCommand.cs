using MediatR;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Requests;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Responses;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.AddMultipleShoppingListItems;

public class AddMultipleShoppingListItemsCommand : IRequest<List<ShoppingListItemResponse>>
{
    public AddMultipleShoppingListItemsRequest Request { get; set; } = default!;
}
