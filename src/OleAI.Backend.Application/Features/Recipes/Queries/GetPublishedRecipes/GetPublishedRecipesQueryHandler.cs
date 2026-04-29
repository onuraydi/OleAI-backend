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
            Include = q => q.Include(r => r.RecipeImages),

            Predicate = r => r.Status == RecipeStatus.Published && r.Visibility == RecipeVisibility.Public
        };

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query.Predicate = r =>
                r.Status == RecipeStatus.Published &&
                r.Visibility == RecipeVisibility.Public &&
                (r.Title.ToLower().Contains(searchTerm) || r.Description.ToLower().Contains(searchTerm));
        }

        var recipes = await _recipeRepository.GetListAsync(query, cancellationToken);
        return _mapper.Map<Paginate<RecipeListItemResponse>>(recipes);
    }
}
