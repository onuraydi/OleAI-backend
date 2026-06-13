using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetPublishedRecipes;

public class GetPublishedRecipesQueryHandler : IRequestHandler<GetPublishedRecipesQuery, Paginate<RecipeListItemResponse>>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private const string DailyTrendCacheKey = "daily_trend_recipe_ids";

    public GetPublishedRecipesQueryHandler(
        IRecipeRepository recipeRepository,
        IUserRepository userRepository,
        ICacheService cacheService,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _recipeRepository = recipeRepository;
        _userRepository = userRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Paginate<RecipeListItemResponse>> Handle(GetPublishedRecipesQuery request, CancellationToken cancellationToken)
    {
        var query = new QueryOptions<Recipe>
        {
            Index = request.Page,
            Size = request.Size,
            EnableTracking = false,
            Include = q => q.Include(r => r.RecipeImages).Include(r => r.Comments).Include(r => r.Interactions)
        };

        var searchTerm = request.SearchTerm?.ToLower();

        var excludedIngredientIds = new List<Guid>();
        var excludedCategoryIds = new List<Guid>();
        var currentUserIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid? currentUserId = !string.IsNullOrWhiteSpace(currentUserIdClaim) ? Guid.Parse(currentUserIdClaim) : null;

        if (request.IsForYou && currentUserId.HasValue)
        {
            var user = await _userRepository.GetAsync(
                predicate: u => u.Id == currentUserId.Value,
                include: q => q
                    .Include(u => u.UserDietaryPreferences).ThenInclude(udp => udp.DietaryPreference).ThenInclude(dp => dp.ExcludedIngredients)
                    .Include(u => u.UserDietaryPreferences).ThenInclude(udp => udp.DietaryPreference).ThenInclude(dp => dp.ExcludedCategories)
                    .Include(u => u.UserAllergies).ThenInclude(ua => ua.AllergyRestriction).ThenInclude(ar => ar.ExcludedIngredients)
                    .Include(u => u.UserAllergies).ThenInclude(ua => ua.AllergyRestriction).ThenInclude(ar => ar.ExcludedCategories)
                    .Include(u => u.UserDislikedIngredients),
                enableTracking: false,
                cancellationToken: cancellationToken);

            if (user != null)
            {
                foreach (var udp in user.UserDietaryPreferences)
                {
                    foreach (var ei in udp.DietaryPreference.ExcludedIngredients) excludedIngredientIds.Add(ei.IngredientId);
                    foreach (var ec in udp.DietaryPreference.ExcludedCategories) excludedCategoryIds.Add(ec.CategoryId);
                }

                foreach (var ua in user.UserAllergies)
                {
                    foreach (var ei in ua.AllergyRestriction.ExcludedIngredients) excludedIngredientIds.Add(ei.IngredientId);
                    foreach (var ec in ua.AllergyRestriction.ExcludedCategories) excludedCategoryIds.Add(ec.CategoryId);
                }

                foreach (var udi in user.UserDislikedIngredients)
                {
                    excludedIngredientIds.Add(udi.IngredientId);
                }
            }
        }

        query.Predicate = r =>
            r.DeletedDate == null &&
            r.Status == RecipeStatus.Published &&
            r.Visibility == RecipeVisibility.Public &&
            (string.IsNullOrWhiteSpace(searchTerm) || r.Title.ToLower().Contains(searchTerm) || r.Description.ToLower().Contains(searchTerm)) &&
            (!request.CategoryId.HasValue || r.RecipeCategories.Any(c => c.RecipeCategoryId == request.CategoryId.Value)) &&
            (!request.Difficulty.HasValue || r.Difficulty == request.Difficulty.Value) &&
            (!request.MaxTotalTime.HasValue || (r.PrepTime + r.CookTime) <= request.MaxTotalTime.Value) &&
            (!request.TagId.HasValue || r.RecipeTags.Any(t => t.TagId == request.TagId.Value)) &&
            (!request.IsForYou || (
                !r.RecipeIngredients.Any(ri => ri.IngredientId.HasValue && excludedIngredientIds.Contains(ri.IngredientId.Value)) &&
                !r.RecipeIngredients.Any(ri => ri.Ingredient != null && excludedCategoryIds.Contains(ri.Ingredient.CategoryId))
            ));

        // Apply Sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "newest":
                    query.OrderBy = q => q.OrderByDescending(r => r.CreatedDate);
                    break;
                case "highest rated":
                    query.OrderBy = q => q.OrderByDescending(r => r.Comments.Where(c => c.Rating != null).Average(c => (double?)c.Rating) ?? 0);
                    break;
                case "quickest":
                    query.OrderBy = q => q.OrderBy(r => r.PrepTime + r.CookTime);
                    break;
                case "daily_trend":
                    return await HandleDailyTrend(query, request.Size, currentUserId, cancellationToken);
                case "popular":
                default:
                    // En beğenilen tarifler
                    query.OrderBy = q => q.OrderByDescending(r => r.Interactions.Count(i => i.IsLiked))
                                          .ThenByDescending(r => r.Comments.Where(c => c.Rating != null).Average(c => (double?)c.Rating) ?? 0);
                    break;
            }
        }
        else
        {
            // Default sort
            query.OrderBy = q => q.OrderByDescending(r => r.CreatedDate);
        }

        var recipes = await _recipeRepository.GetListAsync(query, cancellationToken);

        if (currentUserId.HasValue)
        {
            return _mapper.Map<Paginate<RecipeListItemResponse>>(recipes, opt => opt.Items["currentUserId"] = currentUserId.Value);
        }

        return _mapper.Map<Paginate<RecipeListItemResponse>>(recipes);
    }

    /// <summary>
    /// Günlük trend akışını tamamen kendi içinde yönetir.
    /// 1. Önce Redis cache'e bakar.
    /// 2. Cache varsa: ID'lere göre DB'den çeker, bellekte (client-side) sıralar.
    /// 3. Cache yoksa: Dünkü beğeni sayısına göre DB sorgular, cache'ler ve döner.
    /// EF Core'un IndexOf çevirememe sorununu client-side sıralama ile çözer.
    /// </summary>
    private async Task<Paginate<RecipeListItemResponse>> HandleDailyTrend(
        QueryOptions<Recipe> baseQuery,
        int size,
        Guid? currentUserId,
        CancellationToken cancellationToken)
    {
        // 1. Önce Redis cache'e bak
        var cachedTrendIds = await _cacheService.GetAsync<List<Guid>>(DailyTrendCacheKey);

        if (cachedTrendIds != null && cachedTrendIds.Count > 0)
        {
            // Cache hit — ID'lere göre DB'den çek (sıralama olmadan)
            var cachedQuery = new QueryOptions<Recipe>
            {
                Index = 0,
                Size = cachedTrendIds.Count,
                EnableTracking = false,
                Include = q => q.Include(r => r.RecipeImages).Include(r => r.Comments).Include(r => r.Interactions),
                Predicate = r =>
                    r.DeletedDate == null &&
                    r.Status == RecipeStatus.Published &&
                    r.Visibility == RecipeVisibility.Public &&
                    cachedTrendIds.Contains(r.Id),
                // Sıralama yok — client-side yapacağız
                OrderBy = q => q.OrderBy(r => r.CreatedDate)
            };

            var cachedRecipes = await _recipeRepository.GetListAsync(cachedQuery, cancellationToken);

            // Client-side sıralama: cache'teki ID sırasına göre
            var orderedItems = cachedTrendIds
                .Select(id => cachedRecipes.Items.FirstOrDefault(r => r.Id == id))
                .Where(r => r != null)
                .ToList();

            cachedRecipes.Items = orderedItems!;

            if (currentUserId.HasValue)
                return _mapper.Map<Paginate<RecipeListItemResponse>>(cachedRecipes, opt => opt.Items["currentUserId"] = currentUserId.Value);
            return _mapper.Map<Paginate<RecipeListItemResponse>>(cachedRecipes);
        }

        // 2. Cache miss — dünün beğenilerini hesapla
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var today = DateTime.UtcNow.Date;

        var trendQuery = new QueryOptions<Recipe>
        {
            Index = 0,
            Size = size,
            EnableTracking = false,
            Include = q => q.Include(r => r.RecipeImages).Include(r => r.Comments).Include(r => r.Interactions),
            Predicate = baseQuery.Predicate,
            OrderBy = q => q
                .OrderByDescending(r => r.Interactions.Count(i => i.IsLiked && i.CreatedDate >= yesterday && i.CreatedDate < today))
                .ThenByDescending(r => r.Interactions.Count(i => i.IsLiked))
                .ThenByDescending(r => r.Comments.Where(c => c.Rating != null).Average(c => (double?)c.Rating) ?? 0)
        };

        var trendResults = await _recipeRepository.GetListAsync(trendQuery, cancellationToken);
        var trendIds = trendResults.Items.Select(r => r.Id).ToList();

        // 3. Gece yarısına kadar (UTC) Redis'e cache'le
        if (trendIds.Count > 0)
        {
            var timeUntilMidnight = DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow;
            await _cacheService.SetAsync(DailyTrendCacheKey, trendIds, absoluteExpiration: timeUntilMidnight);
        }

        if (currentUserId.HasValue)
            return _mapper.Map<Paginate<RecipeListItemResponse>>(trendResults, opt => opt.Items["currentUserId"] = currentUserId.Value);
        return _mapper.Map<Paginate<RecipeListItemResponse>>(trendResults);
    }
}

