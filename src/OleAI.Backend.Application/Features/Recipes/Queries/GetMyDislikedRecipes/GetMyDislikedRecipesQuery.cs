using MediatR;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetMyDislikedRecipes
{
    public class GetMyDislikedRecipesQuery : IRequest<Paginate<RecipeListItemResponse>>
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 10;
    }
}
