using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Inventory;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Services;

namespace OleAI.Backend.Application.Features.Recipes.Commands.StartCooking
{
    public class StartCookingCommandHandler : IRequestHandler<StartCookingCommand, string>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUserIngredientRepository _userIngredientRepository;
        private readonly IShoppingListItemRepository _shoppingListItemRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RecipeBusinessRules _recipeBusinessRules;

        public StartCookingCommandHandler(IRecipeRepository recipeRepository, IUserIngredientRepository userIngredientRepository, IShoppingListItemRepository shoppingListItemRepository, IHttpContextAccessor httpContextAccessor, RecipeBusinessRules recipeBusinessRules)
        {
            _recipeRepository = recipeRepository;
            _userIngredientRepository = userIngredientRepository;
            _shoppingListItemRepository = shoppingListItemRepository;
            _httpContextAccessor = httpContextAccessor;
            _recipeBusinessRules = recipeBusinessRules;
        }

        public async Task<string> Handle(StartCookingCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) throw new UnauthorizedAccessException();
            var userId = Guid.Parse(userIdClaim);

            var recipe = await _recipeRepository.GetAsync(
                predicate: r => r.Id == request.RecipeId,
                include: q => q.Include(r => r.RecipeIngredients),
                cancellationToken: cancellationToken);

            _recipeBusinessRules.CheckIfRecipeExists(recipe);

            QueryOptions<UserIngredient> query = new QueryOptions<UserIngredient>();
            query.Predicate = ui => ui.UserId == userId;

            var userIngredientsResult = await _userIngredientRepository.GetListAsync(query, cancellationToken);

            var inventorySercive = new InventoryService();
            var depletedIngredientIds = inventorySercive.DeductIngredientsFromPantry(userIngredientsResult.Items, recipe!.RecipeIngredients);

            if (userIngredientsResult.Items.Any())
            {
                foreach (var updatedIngredient in userIngredientsResult.Items)
                {
                    if (updatedIngredient.IsDepleted)
                    {
                        await _userIngredientRepository.DeleteAsync(updatedIngredient, permanent: true, cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await _userIngredientRepository.UpdateAsync(updatedIngredient, cancellationToken: cancellationToken);
                    }
                }
            }

            if (depletedIngredientIds.Any())
            {
                QueryOptions<ShoppingListItem> shoppingListQuery = new QueryOptions<ShoppingListItem>();
                shoppingListQuery.Predicate = s => s.UserId == userId;

                var currentShoppingListResult = await _shoppingListItemRepository.GetListAsync(shoppingListQuery, cancellationToken);

                var currentShoppingListIds = currentShoppingListResult.Items.Select(s => s.IngredientId).ToList();

                foreach (var ingredientId in depletedIngredientIds)
                {
                    if (!currentShoppingListIds.Contains(ingredientId))
                    {
                        var targetUnit = OleAI.Backend.Domain.Enums.MeasurementUnit.Piece;
                        double targetQty = 1;
                        var userIngredient = userIngredientsResult.Items.FirstOrDefault(ui => ui.IngredientId == ingredientId);
                        var recipeIngredient = recipe.RecipeIngredients.FirstOrDefault(ri => ri.IngredientId == ingredientId);

                        if (recipeIngredient != null)
                        {
                            targetQty = (double)recipeIngredient.Amount;
                        }

                        if (userIngredient != null)
                        {
                            targetUnit = userIngredient.Unit;

                            if (recipeIngredient != null && targetUnit != (OleAI.Backend.Domain.Enums.MeasurementUnit)recipeIngredient.UnitId)
                            {
                                targetQty = MeasurementUnitHelper.ConvertAmount(
                                    (double)recipeIngredient.Amount,
                                    (OleAI.Backend.Domain.Enums.MeasurementUnit)recipeIngredient.UnitId,
                                    targetUnit);
                            }
                        }
                        else if (recipeIngredient != null)
                        {
                            targetUnit = (OleAI.Backend.Domain.Enums.MeasurementUnit)recipeIngredient.UnitId;
                        }

                        // Sürdürülebilir alışveriş miktarları (Çok küçük miktarlar yerine mantıklı paket/alım boyutları)
                        if (targetUnit == OleAI.Backend.Domain.Enums.MeasurementUnit.Gram && targetQty < 250)
                        {
                            targetQty = 250;
                        }
                        else if (targetUnit == OleAI.Backend.Domain.Enums.MeasurementUnit.Milliliter && targetQty < 250)
                        {
                            targetQty = 250;
                        }
                        else if (targetUnit == OleAI.Backend.Domain.Enums.MeasurementUnit.Kilogram && targetQty < 0.25)
                        {
                            targetQty = 0.5;
                        }
                        else if (targetUnit == OleAI.Backend.Domain.Enums.MeasurementUnit.Liter && targetQty < 0.25)
                        {
                            targetQty = 0.5;
                        }

                        await _shoppingListItemRepository.AddAsync(ShoppingListItem.CreateStandard(
                            userId: userId,
                            ingredientId: ingredientId,
                            quantity: targetQty,
                            unit: targetUnit
                        ), cancellationToken);
                    }
                }
            }

            return depletedIngredientIds.Any()
            ? RecipeMessages.EnjoyAndInventoryUpdatedWithShoppingList
            : RecipeMessages.EnjoyAndIngredientsRemovedFromInventory;
        }
    }
}
