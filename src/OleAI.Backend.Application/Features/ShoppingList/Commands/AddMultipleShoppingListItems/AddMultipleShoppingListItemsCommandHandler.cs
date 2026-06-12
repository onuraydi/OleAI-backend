using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Pantry.Dtos;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Responses;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Services;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.AddMultipleShoppingListItems;

public class AddMultipleShoppingListItemsCommandHandler
    : IRequestHandler<AddMultipleShoppingListItemsCommand, List<ShoppingListItemResponse>>
{
    private readonly IShoppingListItemRepository _shoppingListItemRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IIngredientRequestRepository _ingredientRequestRepository;
    private readonly ICacheService _cacheService;
    private readonly ShoppingListBusinessRules _businessRules;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    private const string IngredientCacheKeyPrefix = "ingredient:";

    public AddMultipleShoppingListItemsCommandHandler(
        IShoppingListItemRepository shoppingListItemRepository,
        IIngredientRepository ingredientRepository,
        IIngredientRequestRepository ingredientRequestRepository,
        ICacheService cacheService,
        ShoppingListBusinessRules businessRules,
        IMapper mapper,
        IMediator mediator)
    {
        _shoppingListItemRepository = shoppingListItemRepository;
        _ingredientRepository = ingredientRepository;
        _ingredientRequestRepository = ingredientRequestRepository;
        _cacheService = cacheService;
        _businessRules = businessRules;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<List<ShoppingListItemResponse>> Handle(
        AddMultipleShoppingListItemsCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;
        var newItems = new List<ShoppingListItem>();
        var updatedItems = new List<ShoppingListItem>();
        var responses = new List<ShoppingListItemResponse>();
        var itemsToPublish = new List<ShoppingListItem>();

        foreach (var itemReq in req.Items)
        {
            if (!itemReq.IngredientId.HasValue && string.IsNullOrWhiteSpace(itemReq.RequestedName))
            {
                throw new Core.CrossCuttingConcerns.Exceptions.Types.BusinessException("Lütfen bir malzeme seçin veya yeni bir malzeme adı girin.");
            }

            if (itemReq.IngredientId.HasValue && !string.IsNullOrWhiteSpace(itemReq.RequestedName))
            {
                throw new Core.CrossCuttingConcerns.Exceptions.Types.BusinessException("Aynı anda hem malzeme ID'si hem de özel malzeme adı gönderilemez.");
            }

            ShoppingListItem newItem;
            string responseIngredientName;
            string responseCategoryName;
            string responseCategoryIconName;

            if (itemReq.IngredientId.HasValue)
            {
                var cacheKey = $"{IngredientCacheKeyPrefix}{itemReq.IngredientId}";
                var cachedIngredient = await _cacheService.GetAsync<IngredientCacheModel>(cacheKey);

                if (cachedIngredient == null)
                {
                    var ingredient = await _ingredientRepository.GetAsync(
                        predicate: i => i.Id == itemReq.IngredientId,
                        include: q => q.Include(i => i.Category),
                        enableTracking: false,
                        cancellationToken: cancellationToken);

                    if (ingredient == null)
                    {
                        throw new Core.CrossCuttingConcerns.Exceptions.Types.BusinessException(
                            Features.ShoppingList.Constants.ShoppingListMessages.IngredientNotFound);
                    }

                    cachedIngredient = _mapper.Map<IngredientCacheModel>(ingredient);
                    await _cacheService.SetAsync(cacheKey, cachedIngredient, absoluteExpiration: TimeSpan.FromHours(24));
                }

                if (!MeasurementUnitHelper.IsCompatible(cachedIngredient.UnitFamily, itemReq.Unit))
                    throw new Core.CrossCuttingConcerns.Exceptions.Types.BusinessException(
                        $"'{itemReq.Unit}' birimi bu malzeme için geçerli değil. Uygun aile: {cachedIngredient.UnitFamily}.");

                var existingItem = await _shoppingListItemRepository.GetAsync(
                    predicate: s => s.UserId == req.UserId && s.IngredientId == itemReq.IngredientId.Value && !s.IsPurchased,
                    enableTracking: true,
                    cancellationToken: cancellationToken);

                if (existingItem != null)
                {
                    // Existing items are found. Convert amount and add.
                    var convertedAmount = MeasurementUnitHelper.ConvertAmount(itemReq.Quantity, itemReq.Unit, existingItem.Unit);
                    existingItem.UpdateQuantity(existingItem.Quantity + convertedAmount, existingItem.Unit);
                    newItem = existingItem;

                    if (!updatedItems.Contains(existingItem))
                        updatedItems.Add(existingItem);
                }
                else
                {
                    newItem = ShoppingListItem.CreateStandard(
                        userId: req.UserId,
                        ingredientId: itemReq.IngredientId.Value,
                        quantity: itemReq.Quantity,
                        unit: itemReq.Unit);
                    newItems.Add(newItem);
                }

                responseIngredientName = cachedIngredient.Name;
                responseCategoryName = cachedIngredient.CategoryName;
                responseCategoryIconName = cachedIngredient.CategoryIconName;
            }
            else
            {
                var ingredientRequest = new IngredientRequest(Guid.NewGuid(), req.UserId, itemReq.RequestedName!, itemReq.SuggestedCategoryId);

                newItem = ShoppingListItem.CreatePending(
                    userId: req.UserId,
                    ingredientRequest: ingredientRequest,
                    quantity: itemReq.Quantity,
                    unit: itemReq.Unit);

                responseIngredientName = ingredientRequest.RequestedName!;
                responseCategoryName = "DİĞER";
                responseCategoryIconName = "fastfood";

                itemsToPublish.Add(newItem);
                newItems.Add(newItem);
            }

            responses.Add(new ShoppingListItemResponse
            {
                Id = newItem.Id,
                IngredientId = newItem.IngredientId,
                IngredientRequestId = newItem.IngredientRequestId,
                IngredientName = responseIngredientName,
                CategoryName = responseCategoryName,
                CategoryIconName = responseCategoryIconName,
                Quantity = newItem.Quantity,
                Unit = newItem.Unit,
                IsPurchased = newItem.IsPurchased
            });
        }

        if (newItems.Any())
        {
            await _shoppingListItemRepository.AddRangeAsync(newItems, cancellationToken);
        }

        if (updatedItems.Any())
        {
            await _shoppingListItemRepository.UpdateRangeAsync(updatedItems, cancellationToken);
        }

        foreach (var item in itemsToPublish)
        {
            await _mediator.Publish(new Features.Pantry.Events.IngredientRequestedEvent
            {
                RequestId = item.IngredientRequestId!.Value,
                RequestedName = item.IngredientRequest!.RequestedName!
            }, cancellationToken);
        }

        return responses;
    }
}
