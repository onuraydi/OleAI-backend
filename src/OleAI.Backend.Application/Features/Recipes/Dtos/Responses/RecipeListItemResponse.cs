namespace OleAI.Backend.Application.Features.Recipes.Dtos.Responses;

public class RecipeListItemResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MainImageUrl { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
}
