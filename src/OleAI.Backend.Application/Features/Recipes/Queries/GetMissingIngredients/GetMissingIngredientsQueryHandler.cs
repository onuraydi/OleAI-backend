using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetMissingIngredients
{
    public class GetMissingIngredientsQueryHandler : IRequestHandler<GetMissingIngredientsQuery, List<IngredientResponse>>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUserIngredientRepository _userIngredientRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public GetMissingIngredientsQueryHandler(
            IRecipeRepository recipeRepository,
            IUserIngredientRepository userIngredientRepository,
            IIngredientRepository ingredientRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _userIngredientRepository = userIngredientRepository;
            _ingredientRepository = ingredientRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<IngredientResponse>> Handle(GetMissingIngredientsQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) throw new UnauthorizedAccessException();
            var userId = Guid.Parse(userIdClaim);

            // Tarifi ve malzemelerini çek
            var recipe = await _recipeRepository.GetAsync(
                predicate: r => r.Id == request.RecipeId,
                include: q => q.Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient),
                enableTracking: false,
                cancellationToken: cancellationToken);

            if (recipe == null)
            {
                throw new Exception("Tarif bulunamadı.");
            }

            var recipeIngredientIds = recipe.RecipeIngredients
                .Where(ri => ri.IngredientId.HasValue && ri.Ingredient != null && !ri.Ingredient.IsStaple) // Staple (Temel gıda) olanları baştan eksik saymıyoruz
                .Select(ri => ri.IngredientId!.Value)
                .ToHashSet();

            var userIngredientsQuery = new QueryOptions<UserIngredient>
            {
                Predicate = ui => ui.UserId == userId && ui.IngredientId.HasValue && recipeIngredientIds.Contains(ui.IngredientId.Value),
                EnableTracking = false
            };
            var userIngredientsResult = await _userIngredientRepository.GetListAsync(userIngredientsQuery, cancellationToken);

            var userPantryIngredientIds = userIngredientsResult.Items
                .Select(ui => ui.IngredientId!.Value)
                .ToHashSet();

            // Dolapta olmayanlar
            var missingIngredientIds = recipeIngredientIds.Except(userPantryIngredientIds).ToList();

            if (!missingIngredientIds.Any())
            {
                return new List<IngredientResponse>();
            }

            // Eksik malzemelerin detaylarını al
            var missingIngredientsQuery = new QueryOptions<Ingredient>
            {
                Predicate = i => missingIngredientIds.Contains(i.Id),
                Include = q => q.Include(i => i.Category),
                EnableTracking = false
            };
            var missingIngredientsResult = await _ingredientRepository.GetListAsync(missingIngredientsQuery, cancellationToken);

            return _mapper.Map<List<IngredientResponse>>(missingIngredientsResult.Items);
        }
    }
}
