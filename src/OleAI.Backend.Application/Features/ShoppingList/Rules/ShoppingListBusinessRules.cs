using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.ShoppingList.Rules;

/// <summary>
/// Alışveriş listesi modülüne özgü iş kuralları.
/// Bu sınıf DI ile inject edilir; her kural ihlali uygun exception fırlatır.
/// </summary>
public class ShoppingListBusinessRules
{
    private readonly IShoppingListItemRepository _shoppingListItemRepository;
    private readonly IIngredientRepository _ingredientRepository;

    public ShoppingListBusinessRules(
        IShoppingListItemRepository shoppingListItemRepository,
        IIngredientRepository ingredientRepository)
    {
        _shoppingListItemRepository = shoppingListItemRepository;
        _ingredientRepository = ingredientRepository;
    }

    /// <summary>
    /// Kullanıcının aktif (satın alınmamış) listesinde aynı malzeme zaten varsa
    /// eklemeyi engeller — idempotency garantisi sağlar.
    /// </summary>
    public async Task ItemMustNotAlreadyExistInActiveList(
        Guid userId,
        Guid ingredientId,
        CancellationToken cancellationToken = default)
    {
        var existing = await _shoppingListItemRepository.GetAsync(
            predicate: s => s.UserId == userId &&
                            s.IngredientId == ingredientId &&
                            !s.IsPurchased,
            enableTracking: false,
            cancellationToken: cancellationToken);

        if (existing != null)
            throw new BusinessException(ShoppingListMessages.ProductAlreadyInList);
    }

    /// <summary>
    /// Alışveriş listesi item'ını getirir. Bulunamazsa veya başka bir
    /// kullanıcıya aitse NotFoundException fırlatır.
    /// </summary>
    public async Task<ShoppingListItem> GetItemOrThrow(
        Guid itemId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var item = await _shoppingListItemRepository.GetAsync(
            predicate: s => s.Id == itemId && s.UserId == userId,
            enableTracking: true,
            cancellationToken: cancellationToken);

        if (item == null)
            throw new NotFoundException(ShoppingListMessages.ItemNotFoundOrUnauthorized);

        return item;
    }

    /// <summary>
    /// Malzeme sistemde kayıtlı değilse eklemeye izin vermez.
    /// </summary>
    public async Task IngredientMustExist(
        Guid ingredientId,
        CancellationToken cancellationToken = default)
    {
        var ingredient = await _ingredientRepository.GetAsync(
            predicate: i => i.Id == ingredientId,
            enableTracking: false,
            cancellationToken: cancellationToken);

        if (ingredient == null)
            throw new BusinessException(ShoppingListMessages.IngredientNotFound);
    }
}
