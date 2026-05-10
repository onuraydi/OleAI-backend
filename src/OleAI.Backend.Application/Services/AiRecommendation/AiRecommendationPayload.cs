using System.Text.Json.Serialization;

namespace OleAI.Backend.Application.Services.AiRecommendation
{
    public class AiRecommendationPayload
    {
        [JsonPropertyName("id")]
        public Guid UserId { get; set; }

        [JsonPropertyName("pantryItems")]
        public List<PantryItemDto> PantryItems { get; set; } = new();

        [JsonPropertyName("interactions")]
        public List<RecipeInteractionDto> Interactions { get; set; } = new();

        [JsonPropertyName("preferences")]
        public List<string> DietaryPreferences { get; set; } = new();

        [JsonPropertyName("excludedIngredients")]
        public List<string> ExcludedIngredients { get; set; } = new();

        [JsonPropertyName("excludedCategories")]
        public List<string> ExcludedCategories { get; set; } = new();

        [JsonPropertyName("mode")]
        public string Mode { get; set; } = "default";
    }

    public class PantryItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class RecipeInteractionDto
    {
        [JsonPropertyName("recipeId")]
        public Guid RecipeId { get; set; }

        [JsonPropertyName("type")]
        public string InteractionType { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public DateTime InteractionDate { get; set; }
    }
}
