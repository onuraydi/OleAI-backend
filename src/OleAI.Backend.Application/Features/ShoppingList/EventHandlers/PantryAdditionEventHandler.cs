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
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IIngredientRequestRepository _ingredientRequestRepository;

    public PantryAdditionEventHandler(
        IUserIngredientRepository userIngredientRepository,
        IIngredientRepository ingredientRepository,
        IIngredientRequestRepository ingredientRequestRepository)
    {
        _userIngredientRepository = userIngredientRepository;
        _ingredientRepository = ingredientRepository;
        _ingredientRequestRepository = ingredientRequestRepository;
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

            var ingredient = await _ingredientRepository.GetAsync(i => i.Id == notification.IngredientId.Value, cancellationToken: cancellationToken);
            DateTime? expirationDate = null;
            if (ingredient?.EstimatedShelfLifeInDays.HasValue == true)
            {
                expirationDate = DateTime.UtcNow.AddDays(ingredient.EstimatedShelfLifeInDays.Value);
            }

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

                // Eğer ürün sepete eklendiğinde rafta mevcut değilse (sıfırdan eklendi veya yeni tarih gerekiyorsa) süresini uzatabiliriz ama en kolayı yeni alımlarda tarihi yenilemek veya dokunmamaktır.
                // İstenen özellik: Yeni eklendiğinde tarih set edilmesi. Eğer existing var ve süresi dolmuşsa veya yoksa yenileyelim:
                if (!existing.ExpirationDate.HasValue || existing.ExpirationDate < DateTime.UtcNow)
                {
                    existing.UpdateExpirationDate(expirationDate);
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
                    unit: notification.Unit,
                    expirationDate: expirationDate);

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

            var request = await _ingredientRequestRepository.GetAsync(r => r.Id == notification.IngredientRequestId.Value, cancellationToken: cancellationToken);
            DateTime? expirationDate = null;
            if (request?.EstimatedShelfLifeInDays.HasValue == true)
            {
                expirationDate = DateTime.UtcNow.AddDays(request.EstimatedShelfLifeInDays.Value);
            }

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

                if (!existingPending.ExpirationDate.HasValue || existingPending.ExpirationDate < DateTime.UtcNow)
                {
                    existingPending.UpdateExpirationDate(expirationDate);
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
                    unit: notification.Unit,
                    expirationDate: expirationDate);

                await _userIngredientRepository.AddAsync(newEntry, cancellationToken);
            }
        }
    }
}
