using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
    private readonly ICacheService _cacheService;
    private readonly ShoppingListBusinessRules _businessRules;
    private readonly IMapper _mapper;

    private const string IngredientCacheKeyPrefix = "ingredient:";

    public AddShoppingListItemCommandHandler(
        IShoppingListItemRepository shoppingListItemRepository,
        IIngredientRepository ingredientRepository,
        ICacheService cacheService,
        ShoppingListBusinessRules businessRules,
        IMapper mapper)
    {
        _shoppingListItemRepository = shoppingListItemRepository;
        _ingredientRepository = ingredientRepository;
        _cacheService = cacheService;
        _businessRules = businessRules;
        _mapper = mapper;
    }

    public async Task<ShoppingListItemResponse> Handle(
        AddShoppingListItemCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;

        await _businessRules.IngredientMustExist(req.IngredientId, cancellationToken);

        await _businessRules.ItemMustNotAlreadyExistInActiveList(
            req.UserId, req.IngredientId, cancellationToken);

        var cacheKey = $"{IngredientCacheKeyPrefix}{req.IngredientId}";
        var ingredient = await _cacheService.GetAsync<Ingredient>(cacheKey);

        if (ingredient == null)
        {
            ingredient = await _ingredientRepository.GetAsync(
                predicate: i => i.Id == req.IngredientId,
                include: q => q.Include(i => i.Category),
                enableTracking: false,
                cancellationToken: cancellationToken);

            if (ingredient != null)
                await _cacheService.SetAsync(cacheKey, ingredient, absoluteExpiration: TimeSpan.FromHours(24));
        }

        if (ingredient != null && !MeasurementUnitHelper.IsCompatible(ingredient.UnitFamily, req.Unit))
            throw new Core.CrossCuttingConcerns.Exceptions.Types.BusinessException(
                $"'{req.Unit}' birimi bu malzeme için geçerli değil. Uygun aile: {ingredient.UnitFamily}.");

        var newItem = new ShoppingListItem(
            userId: req.UserId,
            ingredientId: req.IngredientId,
            quantity: req.Quantity,
            unit: req.Unit);

        await _shoppingListItemRepository.AddAsync(newItem, cancellationToken);

        newItem = await _shoppingListItemRepository.GetAsync(
            predicate: s => s.Id == newItem.Id,
            include: q => q.Include(s => s.Ingredient).ThenInclude(i => i!.Category),
            enableTracking: false,
            cancellationToken: cancellationToken);

        return _mapper.Map<ShoppingListItemResponse>(newItem);
    }
}
