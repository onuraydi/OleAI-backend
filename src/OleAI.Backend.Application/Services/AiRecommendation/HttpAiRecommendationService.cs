using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OleAI.Backend.Application.Services.AiRecommendation
{
    public class HttpAiRecommendationService : IAiRecommendationService
    {
        private readonly HttpClient _httpClient;
        private readonly AiServiceSettings _settings;
        private readonly ILogger<HttpAiRecommendationService> _logger;

        public HttpAiRecommendationService(
            HttpClient httpClient,
            IOptions<AiServiceSettings> settings,
            ILogger<HttpAiRecommendationService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;

            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey);
        }

        public async Task<AiRecommendationResponse> GetRecommendationsAsync(
            AiRecommendationPayload payload,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/v1/recommendations", payload, cancellationToken);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<AiRecommendationResponse>(cancellationToken: cancellationToken);
                return result ?? new AiRecommendationResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI Recommendation request failed.");
                return new AiRecommendationResponse();
            }
        }

        public async Task SyncRecipeAsync(
            Guid recipeId,
            string title,
            List<RecipeIngredientAiDto> ingredients,
            string? category = null,
            List<string>? tags = null,
            bool isPublished = true,
            int popularity = 0,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Syncing recipe {RecipeId} ({Title}) with AI engine...", recipeId, title);

                var syncData = new
                {
                    recipeId = recipeId,
                    title = title,
                    ingredients = ingredients,
                    category = category,
                    tags = tags ?? new List<string>(),
                    isPublished = isPublished,
                    popularity = popularity
                };

                var response = await _httpClient.PostAsJsonAsync("/api/v1/recipes/sync", syncData, cancellationToken);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Recipe {RecipeId} synced successfully.", recipeId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to sync recipe {RecipeId} with AI engine.", recipeId);
            }
        }
    }
}
