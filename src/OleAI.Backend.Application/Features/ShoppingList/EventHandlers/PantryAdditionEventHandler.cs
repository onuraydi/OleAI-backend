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
        if (notification.IngredientId.HasValue)
        {
            // Standard ingredient
            var existing = await _userIngredientRepository.GetAsync(
                predicate: ui => ui.UserId == notification.UserId &&
                                 ui.IngredientId == notification.IngredientId.Value,
                withDeleted: true,
                enableTracking: true,
                cancellationToken: cancellationToken);

            if (existing != null)
            {
                if (existing.DeletedDate != null)
                {
                    existing.DeletedDate = null;
                    existing.UpdateQuantity(notification.Quantity, notification.Unit);
                }
                else
                {
                    existing.AddQuantity(notification.Quantity);
                }

                await _userIngredientRepository.UpdateAsync(existing, cancellationToken);
            }
            else
            {
                // Yeni pantry girişi oluştur
                var newEntry = UserIngredient.CreateStandard(
                    id: Guid.NewGuid(),
                    userId: notification.UserId,
                    ingredientId: notification.IngredientId.Value,
                    amount: notification.Quantity,
                    unit: notification.Unit);

                await _userIngredientRepository.AddAsync(newEntry, cancellationToken);
            }
        }
        else if (notification.IngredientRequestId.HasValue)
        {
            // Pending ingredient request
            var existingPending = await _userIngredientRepository.GetAsync(
                predicate: ui => ui.UserId == notification.UserId &&
                                 ui.IngredientRequestId == notification.IngredientRequestId.Value,
                withDeleted: true,
                enableTracking: true,
                cancellationToken: cancellationToken);

            if (existingPending != null)
            {
                if (existingPending.DeletedDate != null)
                {
                    existingPending.DeletedDate = null;
                    existingPending.UpdateQuantity(notification.Quantity, notification.Unit);
                }
                else
                {
                    existingPending.AddQuantity(notification.Quantity);
                }

                await _userIngredientRepository.UpdateAsync(existingPending, cancellationToken);
            }
            else
            {
                // Yeni pending pantry girişi oluştur
                var newEntry = UserIngredient.CreatePending(
                    id: Guid.NewGuid(),
                    userId: notification.UserId,
                    ingredientRequestId: notification.IngredientRequestId.Value,
                    amount: notification.Quantity,
                    unit: notification.Unit);

                await _userIngredientRepository.AddAsync(newEntry, cancellationToken);
            }
        }
    }
}
