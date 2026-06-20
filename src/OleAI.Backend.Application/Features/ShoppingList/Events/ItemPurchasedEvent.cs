using MediatR;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.ShoppingList.Events;

/// <summary>
/// Bir alışveriş listesi item'ı satın alındı olarak işaretlendiğinde yayınlanır.
/// PantryAdditionEventHandler bu event'i dinleyerek malzemeyi kullanıcının
/// dolabına (UserIngredient) otomatik olarak ekler.
/// </summary>
public class ItemPurchasedEvent : INotification
{
    public Guid UserId { get; init; }
    public Guid? IngredientId { get; init; }
    public Guid? IngredientRequestId { get; init; }
    public double Quantity { get; init; }
    public MeasurementUnit Unit { get; init; }
}
