using MediatR;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.RemoveShoppingListItem;

public class RemoveShoppingListItemCommand : IRequest<string>
{
    public Guid ItemId { get; set; }
    public Guid UserId { get; set; }    // Controller'da token'dan set edilir
}
