using MediatR;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.UpdateShoppingListItem;

public class UpdateShoppingListItemCommand : IRequest<string>
{
    public Guid ItemId { get; set; }
    public Guid UserId { get; set; }        // Controller'da token'dan set edilir
    public double NewQuantity { get; set; }
    public MeasurementUnit NewUnit { get; set; }
}
