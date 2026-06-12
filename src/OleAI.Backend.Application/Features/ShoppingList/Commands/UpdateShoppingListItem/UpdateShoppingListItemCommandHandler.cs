using MediatR;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.UpdateShoppingListItem;

using OleAI.Backend.Application.Features.Pantry.Dtos;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Domain.Services;

public class UpdateShoppingListItemCommandHandler : IRequestHandler<UpdateShoppingListItemCommand, string>
{
    private readonly IShoppingListItemRepository _shoppingListItemRepository;
    private readonly ShoppingListBusinessRules _businessRules;
    private readonly ICacheService _cacheService;
    private const string IngredientCacheKeyPrefix = "ingredient:";

    public UpdateShoppingListItemCommandHandler(
        IShoppingListItemRepository shoppingListItemRepository,
        ShoppingListBusinessRules businessRules,
        ICacheService cacheService)
    {
        _shoppingListItemRepository = shoppingListItemRepository;
        _businessRules = businessRules;
        _cacheService = cacheService;
    }

    public async Task<string> Handle(UpdateShoppingListItemCommand command, CancellationToken cancellationToken)
    {
        // Varlık + sahiplik kontrolü; satın alınmış ürünün miktarı domain içinde guard edilir
        var item = await _businessRules.GetItemOrThrow(command.ItemId, command.UserId, cancellationToken);

        if (command.NewQuantity <= 0)
        {
            await _shoppingListItemRepository.DeleteAsync(item, true, cancellationToken);
            return ShoppingListMessages.ItemRemovedFromList;
        }

        if (item.IngredientId.HasValue)
        {
            var cacheKey = $"{IngredientCacheKeyPrefix}{item.IngredientId}";
            var cachedIngredient = await _cacheService.GetAsync<IngredientCacheModel>(cacheKey);

            if (cachedIngredient != null && !MeasurementUnitHelper.IsCompatible(cachedIngredient.UnitFamily, command.NewUnit))
            {
                throw new Core.CrossCuttingConcerns.Exceptions.Types.BusinessException(
                    $"'{command.NewUnit}' birimi bu malzeme için geçerli değil. Uygun aile: {cachedIngredient.UnitFamily}.");
            }
        }

        // Domain davranışı: IsPurchased=true ise BusinessException fırlatır
        item.UpdateQuantity(command.NewQuantity, command.NewUnit);

        await _shoppingListItemRepository.UpdateAsync(item, cancellationToken);

        return ShoppingListMessages.ItemUpdated;
    }
}
