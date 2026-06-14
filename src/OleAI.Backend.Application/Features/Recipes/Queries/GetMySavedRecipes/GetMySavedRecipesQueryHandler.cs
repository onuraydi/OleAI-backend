using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetMySavedRecipes
{
    public class GetMySavedRecipesQueryHandler : BaseUserInteractionRecipesQueryHandler<GetMySavedRecipesQuery>
    {
        public GetMySavedRecipesQueryHandler(IRecipeInteractionRepository recipeInteractionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(recipeInteractionRepository, mapper, httpContextAccessor)
        {
        }

        protected override Expression<Func<RecipeInteraction, bool>> GetInteractionPredicate(Guid userId, GetMySavedRecipesQuery request)
        {
            var searchTerm = request.SearchTerm?.ToLower();
            return i => i.UserId == userId && i.IsSaved &&
                        (string.IsNullOrWhiteSpace(searchTerm) || i.Recipe.Title.ToLower().Contains(searchTerm) || i.Recipe.Description.ToLower().Contains(searchTerm)) &&
                        (!request.CategoryId.HasValue || i.Recipe.RecipeCategories.Any(c => c.RecipeCategoryId == request.CategoryId.Value)) &&
                        (!request.Difficulty.HasValue || i.Recipe.Difficulty == (OleAI.Backend.Domain.Enums.RecipeDifficulty)request.Difficulty.Value) &&
                        (!request.MaxTotalTime.HasValue || (i.Recipe.PrepTime + i.Recipe.CookTime) <= request.MaxTotalTime.Value) &&
                        (!request.TagId.HasValue || i.Recipe.RecipeTags.Any(t => t.TagId == request.TagId.Value)) &&
                        (!request.MinRating.HasValue || (i.Recipe.Comments.Where(c => c.Rating != null).Average(c => (double?)c.Rating) ?? 0) >= request.MinRating.Value);
        }
    }
}
