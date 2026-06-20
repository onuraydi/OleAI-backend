using MediatR;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetMyLikedRecipes
{
    public class GetMyLikedRecipesQuery : IRequest<Paginate<RecipeListItemResponse>>, IPageableRequest, IFilterableRecipeQuery
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 10;
        public string? SearchTerm { get; set; }

        public int? CategoryId { get; set; }
        public int? Difficulty { get; set; }
        public int? MaxTotalTime { get; set; }
        public int? TagId { get; set; }
        public double? MinRating { get; set; }
        public string? SortBy { get; set; }
    }
}
