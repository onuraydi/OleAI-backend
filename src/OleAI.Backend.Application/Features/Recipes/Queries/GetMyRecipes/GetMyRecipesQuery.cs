using MediatR;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetMyRecipes;

public class GetMyRecipesQuery : IRequest<Paginate<RecipeListItemResponse>>
{
    public int Page { get; set; } = 0;
    public int Size { get; set; } = 10;
    public string? SearchTerm { get; set; }

    // Filters
    public int? CategoryId { get; set; }
    public OleAI.Backend.Domain.Enums.RecipeDifficulty? Difficulty { get; set; }
    public int? MaxTotalTime { get; set; }
    public int? TagId { get; set; }
    public OleAI.Backend.Domain.Enums.RecipeVisibility? Visibility { get; set; }

    // Sorting
    public string? SortBy { get; set; }
}
