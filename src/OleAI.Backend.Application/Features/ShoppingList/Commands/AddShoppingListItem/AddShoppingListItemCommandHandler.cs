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

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.AddShoppingListItem;

public class AddShoppingListItemCommandHandler
    : IRequestHandler<AddShoppingListItemCommand, ShoppingListItemResponse>
{
    private readonly IShoppingListItemRepository _shoppingListItemRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IIngredientRequestRepository _ingredientRequestRepository;
    private readonly ICacheService _cacheService;
    private readonly ShoppingListBusinessRules _businessRules;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    private const string IngredientCacheKeyPrefix = "ingredient:";

    public AddShoppingListItemCommandHandler(
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

    public async Task<ShoppingListItemResponse> Handle(
        AddShoppingListItemCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;

        if (!req.IngredientId.HasValue && string.IsNullOrWhiteSpace(req.RequestedName))
        {
            throw new Core.CrossCuttingConcerns.Exceptions.Types.BusinessException("Lütfen bir malzeme seçin veya yeni bir malzeme adı girin.");
        }

        if (req.IngredientId.HasValue && !string.IsNullOrWhiteSpace(req.RequestedName))
        {
            throw new Core.CrossCuttingConcerns.Exceptions.Types.BusinessException("Aynı anda hem malzeme ID'si hem de özel malzeme adı gönderilemez.");
        }

        ShoppingListItem newItem;
        string responseIngredientName;
        string responseCategoryName;

        if (req.IngredientId.HasValue)
        {
            await _businessRules.ItemMustNotAlreadyExistInActiveList(
                req.UserId, req.IngredientId.Value, cancellationToken);

            var cacheKey = $"{IngredientCacheKeyPrefix}{req.IngredientId}";
            var cachedIngredient = await _cacheService.GetAsync<IngredientCacheModel>(cacheKey);

            if (cachedIngredient == null)
            {
                var ingredient = await _ingredientRepository.GetAsync(
                    predicate: i => i.Id == req.IngredientId,
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

            if (!MeasurementUnitHelper.IsCompatible(cachedIngredient.UnitFamily, req.Unit))
                throw new Core.CrossCuttingConcerns.Exceptions.Types.BusinessException(
                    $"'{req.Unit}' birimi bu malzeme için geçerli değil. Uygun aile: {cachedIngredient.UnitFamily}.");

            newItem = ShoppingListItem.CreateStandard(
                userId: req.UserId,
                ingredientId: req.IngredientId.Value,
                quantity: req.Quantity,
                unit: req.Unit);

            responseIngredientName = cachedIngredient.Name;
            responseCategoryName = cachedIngredient.CategoryName;
        }
        else
        {
            // Custom ingredient request
            var ingredientRequest = new IngredientRequest(Guid.NewGuid(), req.UserId, req.RequestedName!, req.SuggestedCategoryId);

            newItem = ShoppingListItem.CreatePending(
                userId: req.UserId,
                ingredientRequest: ingredientRequest,
                quantity: req.Quantity,
                unit: req.Unit);

            responseIngredientName = ingredientRequest.RequestedName!;
            responseCategoryName = "DİĞER";
        }

        await _shoppingListItemRepository.AddAsync(newItem, cancellationToken);

        if (!req.IngredientId.HasValue && newItem.IngredientRequest != null)
        {
            await _mediator.Publish(new Features.Pantry.Events.IngredientRequestedEvent
            {
                RequestId = newItem.IngredientRequestId!.Value,
                RequestedName = newItem.IngredientRequest.RequestedName!
            }, cancellationToken);
        }

        return new ShoppingListItemResponse
        {
            Id = newItem.Id,
            IngredientId = newItem.IngredientId,
            IngredientRequestId = newItem.IngredientRequestId,
            IngredientName = responseIngredientName,
            CategoryName = responseCategoryName,
            Quantity = newItem.Quantity,
            Unit = newItem.Unit,
            IsPurchased = newItem.IsPurchased
        };
    }
}
