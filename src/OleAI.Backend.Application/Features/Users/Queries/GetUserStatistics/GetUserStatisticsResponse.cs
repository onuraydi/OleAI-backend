namespace OleAI.Backend.Application.Features.Users.Queries.GetUserStatistics;

public class GetUserStatisticsResponse
{
    public int TotalRecipesCreated { get; set; }
    public int TotalRecipesSaved { get; set; }
    public int TotalPantryItems { get; set; }
    public int TotalRecipesLiked { get; set; }
}
