namespace OleAI.Backend.Application.Features.Ingredients.Queries.GetPopularDislikedIngredients;

public class GetPopularDislikedIngredientsResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
}
