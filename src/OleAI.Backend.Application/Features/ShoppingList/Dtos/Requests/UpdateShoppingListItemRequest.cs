using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.ShoppingList.Dtos.Requests;

public class UpdateShoppingListItemRequest
{
    public double NewQuantity { get; set; }
    public MeasurementUnit NewUnit { get; set; }
}
