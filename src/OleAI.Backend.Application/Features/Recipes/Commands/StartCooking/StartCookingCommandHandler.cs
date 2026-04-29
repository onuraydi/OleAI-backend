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
                    await _userIngredientRepository.UpdateAsync(updatedIngredient);
                }
            }

            if (depletedIngredientIds.Any())
            {
                QueryOptions<ShoppingListItem> shoppingListQuery = new QueryOptions<ShoppingListItem>();
                query.Predicate = s => s.UserId == userId;

                var currentShoppingListResult = await _shoppingListItemRepository.GetListAsync(shoppingListQuery, cancellationToken);

                var currentShoppingListIds = currentShoppingListResult.Items.Select(s => s.IngredientId).ToList();

                foreach (var ingredientId in depletedIngredientIds)
                {
                    if (!currentShoppingListIds.Contains(ingredientId))
                    {
                        await _shoppingListItemRepository.AddAsync(new ShoppingListItem
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            IngredientId = ingredientId,
                            IsPurchased = false
                        });
                    }
                }
            }

            return depletedIngredientIds.Any()
            ? RecipeMessages.EnjoyAndInventoryUpdatedWithShoppingList
            : RecipeMessages.EnjoyAndIngredientsRemovedFromInventory;
        }
    }
}
