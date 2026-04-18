using MediatR;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Responses;

namespace OleAI.Backend.Application.Features.ShoppingList.Queries.GetMyShoppingList;

public class GetMyShoppingListQuery : IRequest<List<ShoppingListItemResponse>>
{
    public Guid UserId { get; set; }
}
