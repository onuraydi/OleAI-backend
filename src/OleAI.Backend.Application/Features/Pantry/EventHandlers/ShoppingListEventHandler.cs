using MediatR;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Services;

namespace OleAI.Backend.Application.Features.Pantry.EventHandlers;

/// <summary>
/// IngredientDepletedEvent'i yakalar; stoku biten malzemeyi kullanıcının
/// alışveriş listesine otomatik olarak ekler. Aynı malzeme zaten listede
/// (satın alınmamış) varsa tekrar eklenmez (idempotent).
/// </summary>
public class ShoppingListEventHandler : INotificationHandler<IngredientDepletedEvent>
{
    private readonly IShoppingListItemRepository _shoppingListItemRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ICacheService _cacheService;

    private const string IngredientCacheKeyPrefix = "ingredient:";

    public ShoppingListEventHandler(
        IShoppingListItemRepository shoppingListItemRepository,
        IIngredientRepository ingredientRepository,
        ICacheService cacheService)
    {
        _shoppingListItemRepository = shoppingListItemRepository;
        _ingredientRepository = ingredientRepository;
        _cacheService = cacheService;
    }

    public async Task Handle(IngredientDepletedEvent notification, CancellationToken cancellationToken)
    {
        var exists = await _shoppingListItemRepository.GetAsync(
            predicate: s => s.UserId == notification.UserId &&
                            s.IngredientId == notification.MasterIngredientId &&
                            !s.IsPurchased,
            enableTracking: false,
            cancellationToken: cancellationToken);

        if (exists != null) return;

        var cacheKey = $"{IngredientCacheKeyPrefix}{notification.MasterIngredientId}";
        var ingredient = await _cacheService.GetAsync<Ingredient>(cacheKey);

        if (ingredient == null)
        {
            ingredient = await _ingredientRepository.GetAsync(
                predicate: i => i.Id == notification.MasterIngredientId,
                enableTracking: false,
                cancellationToken: cancellationToken);

            if (ingredient == null) return;

            await _cacheService.SetAsync(
                cacheKey,
                ingredient,
                absoluteExpiration: TimeSpan.FromHours(24));
        }

        var defaultUnit = MeasurementUnitHelper.GetDefaultUnitForCategory(ingredient.UnitFamily);

        var newItem = new ShoppingListItem(
            userId: notification.UserId,
            ingredientId: ingredient.Id,
            quantity: 1,
            unit: defaultUnit);

        await _shoppingListItemRepository.AddAsync(newItem, cancellationToken);
    }
}
