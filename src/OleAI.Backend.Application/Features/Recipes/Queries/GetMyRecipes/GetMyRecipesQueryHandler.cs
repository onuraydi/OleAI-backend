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
            Include = q => q.Include(r => r.RecipeImages),

            Predicate = r => r.DeletedDate == null && r.UserId == userId
        };

        var recipes = await _recipeRepository.GetListAsync(query, cancellationToken);
        return _mapper.Map<Paginate<RecipeListItemResponse>>(recipes);
    }
}
