using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.ShoppingList.Dtos.Responses;

public class ShoppingListItemResponse
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public MeasurementUnit Unit { get; set; }
    public bool IsPurchased { get; set; }
}
