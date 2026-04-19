using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.Pantry.Dtos;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Services;

namespace OleAI.Backend.Application.Features.Pantry.EventHandlers;

public class ShoppingListEventHandler : INotificationHandler<IngredientDepletedEvent>
{
    private readonly IShoppingListItemRepository _shoppingListItemRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    private const string IngredientCacheKeyPrefix = "ingredient:";

    public ShoppingListEventHandler(
        IShoppingListItemRepository shoppingListItemRepository,
        IIngredientRepository ingredientRepository,
        ICacheService cacheService,
        IMapper mapper)
    {
        _shoppingListItemRepository = shoppingListItemRepository;
        _ingredientRepository = ingredientRepository;
        _cacheService = cacheService;
        _mapper = mapper;
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
        var cachedIngredient = await _cacheService.GetAsync<IngredientCacheModel>(cacheKey);

        if (cachedIngredient == null)
        {
            var ingredient = await _ingredientRepository.GetAsync(
                predicate: i => i.Id == notification.MasterIngredientId,
                enableTracking: false,
                cancellationToken: cancellationToken);

            if (ingredient == null) return;

            cachedIngredient = _mapper.Map<IngredientCacheModel>(ingredient);

            await _cacheService.SetAsync(
                cacheKey,
                cachedIngredient,
                absoluteExpiration: TimeSpan.FromHours(24));
        }

        var defaultUnit = MeasurementUnitHelper.GetDefaultUnitForCategory(cachedIngredient.UnitFamily);

        var newItem = new ShoppingListItem(
            userId: notification.UserId,
            ingredientId: cachedIngredient.Id,
            quantity: 1,
            unit: defaultUnit);

        await _shoppingListItemRepository.AddAsync(newItem, cancellationToken);
    }
}
