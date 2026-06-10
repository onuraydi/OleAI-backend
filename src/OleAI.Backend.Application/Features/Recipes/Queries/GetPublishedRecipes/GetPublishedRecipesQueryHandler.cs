using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetPublishedRecipes;

public class GetPublishedRecipesQueryHandler : IRequestHandler<GetPublishedRecipesQuery, Paginate<RecipeListItemResponse>>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IMapper _mapper;

    public GetPublishedRecipesQueryHandler(IRecipeRepository recipeRepository, IMapper mapper)
    {
        _recipeRepository = recipeRepository;
        _mapper = mapper;
    }

    public async Task<Paginate<RecipeListItemResponse>> Handle(GetPublishedRecipesQuery request, CancellationToken cancellationToken)
    {
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
            r.Status == RecipeStatus.Published &&
            r.Visibility == RecipeVisibility.Public &&
            (string.IsNullOrWhiteSpace(searchTerm) || r.Title.ToLower().Contains(searchTerm) || r.Description.ToLower().Contains(searchTerm)) &&
            (!request.CategoryId.HasValue || r.RecipeCategories.Any(c => c.RecipeCategoryId == request.CategoryId.Value)) &&
            (!request.Difficulty.HasValue || r.Difficulty == request.Difficulty.Value) &&
            (!request.MaxTotalTime.HasValue || (r.PrepTime + r.CookTime) <= request.MaxTotalTime.Value) &&
            (!request.TagId.HasValue || r.RecipeTags.Any(t => t.TagId == request.TagId.Value));

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
                case "popular":
                default:
                    query.OrderBy = q => q.OrderByDescending(r => r.Comments.Count(c => c.Rating != null))
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
        return _mapper.Map<Paginate<RecipeListItemResponse>>(recipes);
    }
}
