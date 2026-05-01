using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetMyLikedRecipes
{
    public class GetMyLikedRecipesQueryHandler : BaseUserInteractionRecipesQueryHandler<GetMyLikedRecipesQuery>
    {
        public GetMyLikedRecipesQueryHandler(IRecipeInteractionRepository recipeInteractionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(recipeInteractionRepository, mapper, httpContextAccessor)
        {
        }

        protected override Expression<Func<RecipeInteraction, bool>> GetInteractionPredicate(Guid userId)
        {
            return i => i.UserId == userId && i.IsLiked;
        }
    }
}
