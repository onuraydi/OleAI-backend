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

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetMyLikedRecipes
{
    public class GetMyLikedRecipesQueryHandler : IRequestHandler<GetMyLikedRecipesQuery, Paginate<RecipeListItemResponse>>
    {
        private readonly IRecipeInteractionRepository _recipeInteractionRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetMyLikedRecipesQueryHandler(IRecipeInteractionRepository recipeInteractionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _recipeInteractionRepository = recipeInteractionRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Paginate<RecipeListItemResponse>> Handle(GetMyLikedRecipesQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var query = new QueryOptions<RecipeInteraction>
            {
                Index = request.Page,
                Size = request.Size,
                EnableTracking = false,
                Predicate = i => i.UserId == userId && i.IsLiked,
                Include = q => q.Include(i => i.Recipe).ThenInclude(r => r.RecipeImages)
            };

            var interactions = await _recipeInteractionRepository.GetListAsync(query, cancellationToken);

            var recipes = interactions.Items.Select(i => i.Recipe).ToList();
            var mappedRecipes = _mapper.Map<IList<RecipeListItemResponse>>(recipes);

            return new Paginate<RecipeListItemResponse>
            {
                Items = mappedRecipes,
                PageSize = interactions.PageSize,
                PageNumber = interactions.PageNumber,
                TotalItems = interactions.TotalItems,
                TotalPages = interactions.TotalPages
            };
        }
    }
}
