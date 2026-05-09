namespace OleAI.Backend.Application.Services.AiRecommendation
{
    /// <summary>
    /// AI öneri motorundan dönen tarif önerisi yanıtı.
    /// </summary>
    public class AiRecommendationResponse
    {
        public List<RecommendedRecipeItem> Recipes { get; set; } = new();
    }

    public class RecommendedRecipeItem
    {
        public Guid RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public double Score { get; set; }
    }
}
