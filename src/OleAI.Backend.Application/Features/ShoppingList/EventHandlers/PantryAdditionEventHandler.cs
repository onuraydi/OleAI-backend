using MediatR;
using OleAI.Backend.Application.Features.ShoppingList.Events;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.ShoppingList.EventHandlers;

/// <summary>
/// ItemPurchasedEvent'i dinler; satın alınan malzemeyi kullanıcının dolabına
/// (UserIngredient) otomatik olarak ekler ya da mevcutsa miktarını artırır.
/// </summary>
public class PantryAdditionEventHandler : INotificationHandler<ItemPurchasedEvent>
{
    private readonly IUserIngredientRepository _userIngredientRepository;

    public PantryAdditionEventHandler(IUserIngredientRepository userIngredientRepository)
    {
        _userIngredientRepository = userIngredientRepository;
    }

    public async Task Handle(ItemPurchasedEvent notification, CancellationToken cancellationToken)
    {
        // Dolapta zaten aynı malzeme var mı kontrol et
        var existing = await _userIngredientRepository.GetAsync(
            predicate: ui => ui.UserId == notification.UserId &&
                             ui.IngredientId == notification.IngredientId &&
                             ui.IngredientRequestId == null,   // sadece onaylı kayıtları eşleştir
            enableTracking: true,
            cancellationToken: cancellationToken);

        if (existing != null)
        {
            // Mevcut girişte miktarı artır (AddQuantity domain davranışı)
            existing.AddQuantity(notification.Quantity);
            await _userIngredientRepository.UpdateAsync(existing, cancellationToken);
        }
        else
        {
            // Yeni pantry girişi oluştur
            var newEntry = UserIngredient.CreateStandard(
                id: Guid.NewGuid(),
                userId: notification.UserId,
                ingredientId: notification.IngredientId,
                amount: notification.Quantity,
                unit: notification.Unit);

            await _userIngredientRepository.AddAsync(newEntry, cancellationToken);
        }
    }
}
