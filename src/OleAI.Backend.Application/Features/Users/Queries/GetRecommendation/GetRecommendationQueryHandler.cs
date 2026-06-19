using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;
using OleAI.Backend.Application.Features.Users.Rules;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Users.Queries.GetRecommendation
{
    public class GetRecommendationQueryHandler : IRequestHandler<GetRecommendationQuery, List<RecipeRecommendationDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAiRecommendationService _aiRecommendationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;
        private readonly IIngredientRepository _ingredientRepository;

        public GetRecommendationQueryHandler(
            IUserRepository userRepository,
            IAiRecommendationService aiRecommendationService,
            IHttpContextAccessor httpContextAccessor,
            UserBusinessRules userBusinessRules,
            IRecipeRepository recipeRepository,
            IMapper mapper,
            IIngredientRepository ingredientRepository)
        {
            _userRepository = userRepository;
            _aiRecommendationService = aiRecommendationService;
            _httpContextAccessor = httpContextAccessor;
            _userBusinessRules = userBusinessRules;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
            _ingredientRepository = ingredientRepository;
        }

        public async Task<List<RecipeRecommendationDto>> Handle(GetRecommendationQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(
                _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // N+1 problemi olmadan tek sorguda tüm ilişkisel grafiği Eager Loading ile çek
            var user = await _userRepository.GetAsync(
                predicate: u => u.Id == userId,
                include: q => q
                    .Include(u => u.UserDietaryPreferences)
                        .ThenInclude(udp => udp.DietaryPreference)
                            .ThenInclude(dp => dp.ExcludedIngredients)
                                .ThenInclude(ei => ei.Ingredient)
                    .Include(u => u.UserDietaryPreferences)
                        .ThenInclude(udp => udp.DietaryPreference)
                            .ThenInclude(dp => dp.ExcludedCategories)
                                .ThenInclude(ec => ec.Category)
                    .Include(u => u.UserAllergies)
                        .ThenInclude(ua => ua.AllergyRestriction)
                            .ThenInclude(ar => ar.ExcludedIngredients)
                                .ThenInclude(ei => ei.Ingredient)
                    .Include(u => u.UserAllergies)
                        .ThenInclude(ua => ua.AllergyRestriction)
                            .ThenInclude(ar => ar.ExcludedCategories)
                                .ThenInclude(ec => ec.Category)
                    .Include(u => u.UserDislikedIngredients)
                        .ThenInclude(udi => udi.Ingredient)
                    .Include(u => u.UserIngredients)
                        .ThenInclude(ui => ui.Ingredient)
                    .Include(u => u.Interactions),
                enableTracking: false,
                cancellationToken: cancellationToken);

            _userBusinessRules.CheckIfUserExists(user);

            // Temel Gıdaları (Staples) DB'den çek
            var staplesQuery = new QueryOptions<Domain.Entities.Ingredient>
            {
                Predicate = i => i.IsStaple == true,
                EnableTracking = false
            };
            var staplesResult = await _ingredientRepository.GetListAsync(staplesQuery, cancellationToken);
            var stapleIds = staplesResult.Items.Select(i => i.Id).ToList();

            // Entity -> AiRecommendationPayload mapping
            var payload = MapToPayload(userId, user!, request.Mode, request.UseZeroWaste, request.UseAllergyFilters, request.UseDietFilters, request.UseDislikedFilters, stapleIds);

            var aiResponse = await _aiRecommendationService.GetRecommendationsAsync(payload, cancellationToken);
            var recipeIds = aiResponse.Recipes.Select(r => r.RecipeId).ToList();

            // "Explore" (Keşfet) modundaysa, daha önce etkileşime girdiği (like, save, dislike) tarifleri AI sonuçlarından filtrele
            if (request.Mode == RecommendationMode.Explore)
            {
                var interactedRecipeIds = user.Interactions
                    .Where(i => i.IsLiked || i.IsSaved || i.IsDisliked)
                    .Select(i => i.RecipeId)
                    .ToHashSet();

                recipeIds = recipeIds.Where(id => !interactedRecipeIds.Contains(id)).ToList();
            }

            if (recipeIds.Count == 0)
            {
                // FALLBACK & COLD START: AI'dan sonuç gelmediyse (kullanıcının hiç etkileşimi ve malzemesi yoksa), global popüler tarifleri getir
                var fallbackQuery = new QueryOptions<Domain.Entities.Recipe>
                {
                    Index = 0,
                    Size = 20, // 20 popüler tarif
                    EnableTracking = false,
                    Include = q => q.Include(r => r.RecipeImages).Include(r => r.Comments).Include(r => r.Interactions).Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag),
                    Predicate = r => r.DeletedDate == null && r.Status == RecipeStatus.Published && r.Visibility == RecipeVisibility.Public,
                    OrderBy = q => q.OrderByDescending(r => r.Interactions.Count) // En çok etkileşim alanlar (Global Popularity)
                };

                var fallbackRecipes = await _recipeRepository.GetListAsync(fallbackQuery, cancellationToken);
                var mappedFallback = _mapper.Map<Paginate<RecipeListItemResponse>>(fallbackRecipes, opt => opt.Items["currentUserId"] = userId);

                return mappedFallback.Items.Select(r => new RecipeRecommendationDto
                {
                    Recipe = r,
                    MatchScore = 1.0, // Varsayılan skor
                    IsFallback = true
                }).ToList();
            }

            var query = new QueryOptions<Domain.Entities.Recipe>
            {
                Index = 0,
                Size = 100, // En fazla 100 öneri
                EnableTracking = false,
                Include = q => q.Include(r => r.RecipeImages).Include(r => r.Comments).Include(r => r.Interactions).Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag),
                Predicate = r => r.DeletedDate == null && r.Status == RecipeStatus.Published && r.Visibility == RecipeVisibility.Public && recipeIds.Contains(r.Id),
                OrderBy = null // Client-side (AI order) ile sıralayacağız
            };

            var recipes = await _recipeRepository.GetListAsync(query, cancellationToken);

            var mappedRecipes = _mapper.Map<Paginate<RecipeListItemResponse>>(recipes, opt => opt.Items["currentUserId"] = userId);

            // AI'ın döndüğü sıraya göre ve MatchScore ile birleştirerek listeyi oluştur
            var resultList = new List<RecipeRecommendationDto>();
            foreach (var aiRecipe in aiResponse.Recipes)
            {
                var mappedRecipe = mappedRecipes.Items.FirstOrDefault(r => r.Id == aiRecipe.RecipeId);
                if (mappedRecipe != null)
                {
                    resultList.Add(new RecipeRecommendationDto
                    {
                        Recipe = mappedRecipe,
                        MatchScore = aiRecipe.Score
                    });
                }
            }

            return resultList;
        }

        /// <summary>
        /// Kullanıcının diyet tercihleri ve alerjilerinden, AI motorunun beklediği düz listeye (flat list) dönüştürür.
        /// Hem diyet hem alerji kısıtlarından gelen dışlama listeleri birleştirilir ve tekrar eden üyeler elenir.
        /// </summary>
        private static AiRecommendationPayload MapToPayload(Guid userId, Domain.Entities.User user, RecommendationMode mode, bool useZeroWaste, bool useAllergyFilters, bool useDietFilters, bool useDislikedFilters, List<Guid> stapleIds)
        {
            var excludedIngredients = new HashSet<string>();
            var excludedCategories = new HashSet<string>();

            if (useDietFilters)
            {
                // Diyet tercihlerinden gelen dışlamalar
                foreach (var udp in user.UserDietaryPreferences)
                {
                    foreach (var ei in udp.DietaryPreference.ExcludedIngredients)
                        excludedIngredients.Add(ei.Ingredient.Name);

                    foreach (var ec in udp.DietaryPreference.ExcludedCategories)
                        excludedCategories.Add(ec.Category.Name);
                }
            }

            if (useAllergyFilters)
            {
                // Alerji kısıtlarından gelen dışlamalar
                foreach (var ua in user.UserAllergies)
                {
                    foreach (var ei in ua.AllergyRestriction.ExcludedIngredients)
                        excludedIngredients.Add(ei.Ingredient.Name);

                    foreach (var ec in ua.AllergyRestriction.ExcludedCategories)
                        excludedCategories.Add(ec.Category.Name);
                }
            }

            if (useDislikedFilters)
            {
                // Sevmediği malzemelerden (DislikedIngredients) gelen dışlamalar
                foreach (var udi in user.UserDislikedIngredients)
                {
                    excludedIngredients.Add(udi.Ingredient.Name);
                }
            }

            return new AiRecommendationPayload
            {
                UserId = userId,
                PantryItems = user.UserIngredients
                    .Where(ui => ui.Ingredient != null && ui.Amount > 0)
                    .Select(ui =>
                    {
                        var baseUnit = ui.Ingredient!.UnitFamily switch
                        {
                            Domain.Enums.MeasurementCategory.Weight => Domain.Enums.MeasurementUnit.Gram,
                            Domain.Enums.MeasurementCategory.Volume => Domain.Enums.MeasurementUnit.Milliliter,
                            _ => Domain.Enums.MeasurementUnit.Piece
                        };
                        return new PantryItemDto
                        {
                            Id = ui.Ingredient!.Id,
                            Name = ui.Ingredient.Name,
                            Quantity = OleAI.Backend.Domain.Services.MeasurementUnitHelper.ConvertAmount(ui.Amount, ui.Unit, baseUnit),
                            Unit = baseUnit.ToString(),
                            ExpirationDate = ui.ExpirationDate
                        };
                    })
                    .ToList(),
                Interactions = user.Interactions
                    .Where(i => i.IsLiked || i.IsDisliked || i.IsSaved)
                    .Select(i => new RecipeInteractionDto
                    {
                        RecipeId = i.RecipeId,
                        InteractionType = i.IsLiked ? "like" : (i.IsDisliked ? "dislike" : "save"),
                        InteractionDate = i.UpdatedDate ?? i.CreatedDate
                    })
                    .ToList(),
                DietaryPreferences = user.UserDietaryPreferences
                    .Select(udp => udp.DietaryPreference.Name)
                    .ToList(),
                ExcludedIngredients = excludedIngredients.ToList(),
                ExcludedCategories = excludedCategories.ToList(),
                Mode = mode.ToString().ToLower(),
                UseZeroWaste = useZeroWaste,
                ToleranceLevel = 0.8, // %20 eksik malzeme toleransı (fuzzy matching)
                StapleIngredientIds = stapleIds
            };
        }
    }
}
