using System;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetMyDislikedRecipes
{
    public class GetMyDislikedRecipesQueryHandler : BaseUserInteractionRecipesQueryHandler<GetMyDislikedRecipesQuery>
    {
        public GetMyDislikedRecipesQueryHandler(IRecipeInteractionRepository recipeInteractionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(recipeInteractionRepository, mapper, httpContextAccessor)
        {
        }

        protected override Expression<Func<RecipeInteraction, bool>> GetInteractionPredicate(Guid userId, GetMyDislikedRecipesQuery request)
        {
            var searchTerm = request.SearchTerm?.ToLower();
            return i => i.UserId == userId && i.IsDisliked &&
                        (string.IsNullOrWhiteSpace(searchTerm) || i.Recipe.Title.ToLower().Contains(searchTerm) || i.Recipe.Description.ToLower().Contains(searchTerm));
        }
    }
}
