using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Inventory;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Recipes.Commands.StartCooking
{
    public class StartCookingCommandHandler : IRequestHandler<StartCookingCommand, List<IngredientResponse>>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUserIngredientRepository _userIngredientRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RecipeBusinessRules _recipeBusinessRules;
        private readonly IMapper _mapper;

        public StartCookingCommandHandler(
            IRecipeRepository recipeRepository,
            IUserIngredientRepository userIngredientRepository,
            IIngredientRepository ingredientRepository,
            IHttpContextAccessor httpContextAccessor,
            RecipeBusinessRules recipeBusinessRules,
            IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _userIngredientRepository = userIngredientRepository;
            _ingredientRepository = ingredientRepository;
            _httpContextAccessor = httpContextAccessor;
            _recipeBusinessRules = recipeBusinessRules;
            _mapper = mapper;
        }

        public async Task<List<IngredientResponse>> Handle(StartCookingCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) throw new UnauthorizedAccessException();
            var userId = Guid.Parse(userIdClaim);

            var recipe = await _recipeRepository.GetAsync(
                predicate: r => r.Id == request.RecipeId,
                include: q => q.Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient),
                cancellationToken: cancellationToken);

            _recipeBusinessRules.CheckIfRecipeExists(recipe);

            QueryOptions<UserIngredient> query = new QueryOptions<UserIngredient>();
            query.Predicate = ui => ui.UserId == userId;
            query.Size = 1000;

            var userIngredientsResult = await _userIngredientRepository.GetListAsync(query, cancellationToken);

            // Tüketilecek malzemeleri filtrele:
            // 1. Temel Gıda (Staple) DEĞİLSE
            // 2. Kullanıcı tarafından (Pre-Cook aşamasında) Yoksayılmamışsa (Ignored DEĞİLSE)
            var ingredientsToConsume = recipe!.RecipeIngredients
                .Where(ri => ri.Ingredient != null
                          && !ri.Ingredient.IsStaple
                          && ri.IngredientId.HasValue
                          && !request.IgnoredIngredientIds.Contains(ri.IngredientId.Value))
                .ToList();

            var inventoryService = new InventoryService();
            var depletedIngredientIds = inventoryService.DeductIngredientsFromPantry(userIngredientsResult.Items, ingredientsToConsume);

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

            // Eskiden burada otomatik Alışveriş Listesine ekleme vardı. Artık bunu Frontend'e (Post-Cook) bırakıyoruz.
            // Sadece biten ürünlerin detaylarını dönüyoruz.
            if (!depletedIngredientIds.Any())
            {
                return new List<IngredientResponse>();
            }

            var queryOptions = new QueryOptions<Ingredient>
            {
                Predicate = i => depletedIngredientIds.Contains(i.Id),
                Include = q => q.Include(i => i.Category),
                EnableTracking = false
            };

            var depletedIngredientsResult = await _ingredientRepository.GetListAsync(queryOptions, cancellationToken);

            return _mapper.Map<List<IngredientResponse>>(depletedIngredientsResult.Items);
        }
    }
}
