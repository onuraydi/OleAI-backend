using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetPendingAdminRecipes
{
    public class GetPendingAdminRecipesQueryHandler : IRequestHandler<GetPendingAdminRecipesQuery, Paginate<PendingAdminRecipeListResponse>>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;

        public GetPendingAdminRecipesQueryHandler(IRecipeRepository recipeRepository, IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _mapper = mapper;
        }
        public async Task<Paginate<PendingAdminRecipeListResponse>> Handle(GetPendingAdminRecipesQuery request, CancellationToken cancellationToken)
        {
            QueryOptions<Recipe> query = new QueryOptions<Recipe>();

            query.Predicate = r => r.Status == RecipeStatus.PendingAdminApproval;

            query.Index = request.Page;
            query.Size = request.Size;
            query.EnableTracking = false;

            query.Include = r => r.Include(x => x.RecipeImages);

            var recipes = await _recipeRepository.GetListAsync(query, cancellationToken: cancellationToken);

            return _mapper.Map<Paginate<PendingAdminRecipeListResponse>>(recipes);
        }
    }
}
