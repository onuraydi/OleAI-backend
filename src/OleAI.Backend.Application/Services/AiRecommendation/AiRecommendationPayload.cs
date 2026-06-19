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

        [JsonPropertyName("useZeroWaste")]
        public bool UseZeroWaste { get; set; } = false;

        [JsonPropertyName("toleranceLevel")]
        public double ToleranceLevel { get; set; } = 0.8;

        [JsonPropertyName("stapleIngredientIds")]
        public List<Guid> StapleIngredientIds { get; set; } = new();
    }

    public class PantryItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public double Quantity { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; } = string.Empty;

        [JsonPropertyName("expirationDate")]
        public DateTime? ExpirationDate { get; set; }
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

    public class RecipeIngredientAiDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string Unit { get; set; } = string.Empty;
        public List<string> Aliases { get; set; } = new();
    }
}
