using System.Linq.Expressions;
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

namespace OleAI.Backend.Application.Features.Recipes.Queries;

public abstract class BaseUserInteractionRecipesQueryHandler<TQuery> : IRequestHandler<TQuery, Paginate<RecipeListItemResponse>>
    where TQuery : IRequest<Paginate<RecipeListItemResponse>>, IPageableRequest
{
    protected readonly IRecipeInteractionRepository RecipeInteractionRepository;
    protected readonly IMapper Mapper;
    protected readonly IHttpContextAccessor HttpContextAccessor;

    protected BaseUserInteractionRecipesQueryHandler(
        IRecipeInteractionRepository recipeInteractionRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        RecipeInteractionRepository = recipeInteractionRepository;
        Mapper = mapper;
        HttpContextAccessor = httpContextAccessor;
    }

    public async Task<Paginate<RecipeListItemResponse>> Handle(TQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var query = new QueryOptions<RecipeInteraction>
        {
            Index = request.Page,
            Size = request.Size,
            EnableTracking = false,
            Predicate = GetInteractionPredicate(userId),
            Include = q => q.Include(i => i.Recipe).ThenInclude(r => r.RecipeImages)
                            .Include(i => i.Recipe).ThenInclude(r => r.Comments)
        };

        var interactions = await RecipeInteractionRepository.GetListAsync(query, cancellationToken);

        var recipes = interactions.Items.Select(i => i.Recipe).ToList();
        var mappedRecipes = Mapper.Map<IList<RecipeListItemResponse>>(recipes);

        return new Paginate<RecipeListItemResponse>
        {
            Items = mappedRecipes,
            PageSize = interactions.PageSize,
            PageNumber = interactions.PageNumber,
            TotalItems = interactions.TotalItems,
            TotalPages = interactions.TotalPages
        };
    }

    protected abstract Expression<Func<RecipeInteraction, bool>> GetInteractionPredicate(Guid userId);
}
