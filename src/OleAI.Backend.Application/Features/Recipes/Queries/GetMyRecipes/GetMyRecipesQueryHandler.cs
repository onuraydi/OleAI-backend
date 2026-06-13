using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetMyRecipes;

public class GetMyRecipesQueryHandler : IRequestHandler<GetMyRecipesQuery, Paginate<RecipeListItemResponse>>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyRecipesQueryHandler(IRecipeRepository recipeRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _recipeRepository = recipeRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Paginate<RecipeListItemResponse>> Handle(GetMyRecipesQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var query = new QueryOptions<Recipe>
        {
            Index = request.Page,
            Size = request.Size,
            EnableTracking = false,
            Include = q => q.Include(r => r.RecipeImages).Include(r => r.Comments)
        };

        var searchTerm = request.SearchTerm?.ToLower();
        query.Predicate = r =>
            r.DeletedDate == null &&
            r.UserId == userId &&
            (string.IsNullOrWhiteSpace(searchTerm) || r.Title.ToLower().Contains(searchTerm) || r.Description.ToLower().Contains(searchTerm)) &&
            (!request.CategoryId.HasValue || r.RecipeCategories.Any(c => c.RecipeCategoryId == request.CategoryId.Value)) &&
            (!request.Difficulty.HasValue || r.Difficulty == request.Difficulty.Value) &&
            (!request.MaxTotalTime.HasValue || (r.PrepTime + r.CookTime) <= request.MaxTotalTime.Value) &&
            (!request.TagId.HasValue || r.RecipeTags.Any(t => t.TagId == request.TagId.Value)) &&
            (!request.Visibility.HasValue || r.Visibility == request.Visibility.Value);

        // Apply Sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "highest rated":
                    query.OrderBy = q => q.OrderByDescending(r => r.Comments.Where(c => c.Rating != null).Average(c => (double?)c.Rating) ?? 0);
                    break;
                case "quickest":
                    query.OrderBy = q => q.OrderBy(r => r.PrepTime + r.CookTime);
                    break;
                case "popular":
                    query.OrderBy = q => q.OrderByDescending(r => r.Interactions.Count(i => i.IsLiked))
                                          .ThenByDescending(r => r.Comments.Where(c => c.Rating != null).Average(c => (double?)c.Rating) ?? 0);
                    break;
                case "newest":
                default:
                    query.OrderBy = q => q.OrderByDescending(r => r.CreatedDate);
                    break;
            }
        }
        else
        {
            query.OrderBy = q => q.OrderByDescending(r => r.CreatedDate);
        }

        var recipes = await _recipeRepository.GetListAsync(query, cancellationToken);
        return _mapper.Map<Paginate<RecipeListItemResponse>>(recipes);
    }
}
