using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.ShoppingList.Dtos.Requests;

public class AddShoppingListItemRequest
{
    public Guid UserId { get; set; }            // Controller'da token'dan set edilir
    public Guid IngredientId { get; set; }
    public double Quantity { get; set; }
    public MeasurementUnit Unit { get; set; }
}
