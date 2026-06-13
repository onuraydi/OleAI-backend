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
            Predicate = GetInteractionPredicate(userId, request),
            Include = q => q.Include(i => i.Recipe).ThenInclude(r => r.RecipeImages)
                            .Include(i => i.Recipe).ThenInclude(r => r.Comments)
                            .Include(i => i.Recipe).ThenInclude(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
                            .Include(i => i.Recipe).ThenInclude(r => r.RecipeCategories).ThenInclude(rc => rc.RecipeCategory)
        };

        if (request is IFilterableRecipeQuery filterableRequest && !string.IsNullOrWhiteSpace(filterableRequest.SortBy))
        {
            switch (filterableRequest.SortBy.ToLower())
            {
                case "highest rated":
                    query.OrderBy = q => q.OrderByDescending(i => i.Recipe.Comments.Where(c => c.Rating != null).Average(c => (double?)c.Rating) ?? 0);
                    break;
                case "quickest":
                    query.OrderBy = q => q.OrderBy(i => i.Recipe.PrepTime + i.Recipe.CookTime);
                    break;
                case "popular":
                    query.OrderBy = q => q.OrderByDescending(i => i.Recipe.Interactions.Count(interaction => interaction.IsLiked))
                                          .ThenByDescending(i => i.Recipe.Comments.Where(c => c.Rating != null).Average(c => (double?)c.Rating) ?? 0);
                    break;
                case "newest":
                default:
                    query.OrderBy = q => q.OrderByDescending(i => i.Recipe.CreatedDate);
                    break;
            }
        }
        else
        {
            query.OrderBy = q => q.OrderByDescending(i => i.CreatedDate);
        }

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

    protected abstract Expression<Func<RecipeInteraction, bool>> GetInteractionPredicate(Guid userId, TQuery request);
}
