namespace OleAI.Backend.Application.Features.Recipes.Dtos.Responses;

public class RecipeListItemResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MainImageUrl { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public int Visibility { get; set; }
    public string VisibilityName { get; set; } = string.Empty;
    public bool IsLiked { get; set; }
    public bool IsSaved { get; set; }
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public int Difficulty { get; set; }
}
