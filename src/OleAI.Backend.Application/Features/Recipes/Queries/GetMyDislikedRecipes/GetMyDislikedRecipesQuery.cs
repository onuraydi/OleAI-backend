using MediatR;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetMyDislikedRecipes
{
    public class GetMyDislikedRecipesQuery : IRequest<Paginate<RecipeListItemResponse>>, IPageableRequest
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 10;
        public string? SearchTerm { get; set; }
    }
}
