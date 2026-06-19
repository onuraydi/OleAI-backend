using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.Restock;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.ShoppingLists.Commands.SyncDepletedIngredients
{
    public class SyncDepletedIngredientsCommandHandler : IRequestHandler<SyncDepletedIngredientsCommand, string>
    {
        private readonly IShoppingListItemRepository _shoppingListItemRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISmartRestockService _smartRestockService;
        private readonly IIngredientRepository _ingredientRepository;

        public SyncDepletedIngredientsCommandHandler(
            IShoppingListItemRepository shoppingListItemRepository,
            IHttpContextAccessor httpContextAccessor,
            ISmartRestockService smartRestockService,
            IIngredientRepository ingredientRepository)
        {
            _shoppingListItemRepository = shoppingListItemRepository;
            _httpContextAccessor = httpContextAccessor;
            _smartRestockService = smartRestockService;
            _ingredientRepository = ingredientRepository;
        }

        public async Task<string> Handle(SyncDepletedIngredientsCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) throw new UnauthorizedAccessException();
            var userId = Guid.Parse(userIdClaim);

            if (!request.ItemsToRestock.Any())
                return "Hiçbir ürün eklenmedi.";

            // Mevcut listeyi çek
            QueryOptions<ShoppingListItem> shoppingListQuery = new QueryOptions<ShoppingListItem>();
            shoppingListQuery.Predicate = s => s.UserId == userId && !s.IsPurchased;
            shoppingListQuery.Size = 1000;

            var currentShoppingListResult = await _shoppingListItemRepository.GetListAsync(shoppingListQuery, cancellationToken);
            var currentShoppingListIds = currentShoppingListResult.Items.Select(s => s.IngredientId).ToList();

            var addedCount = 0;

            foreach (var item in request.ItemsToRestock)
            {
                if (!currentShoppingListIds.Contains(item.IngredientId))
                {
                    if (item.Quantity > 0)
                    {
                        await _shoppingListItemRepository.AddAsync(ShoppingListItem.CreateStandard(
                            userId: userId,
                            ingredientId: item.IngredientId,
                            quantity: item.Quantity,
                            unit: (Domain.Enums.MeasurementUnit)item.Unit
                        ), cancellationToken);
                        addedCount++;
                    }
                }
            }

            return $"{addedCount} adet biten ürün alışveriş listesine başarıyla eklendi.";
        }
    }
}
