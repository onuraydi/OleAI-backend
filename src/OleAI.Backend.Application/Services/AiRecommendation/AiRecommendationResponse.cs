using System.Text.Json.Serialization;

namespace OleAI.Backend.Application.Services.AiRecommendation
{
    /// <summary>
    /// AI öneri motorundan dönen tarif önerisi yanıtı.
    /// </summary>
    public class AiRecommendationResponse
    {
        [JsonPropertyName("recipes")]
        public List<RecommendedRecipeItem> Recipes { get; set; } = new();
    }

    public class RecommendedRecipeItem
    {
        [JsonPropertyName("recipeId")]
        public Guid RecipeId { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }
    }
}
