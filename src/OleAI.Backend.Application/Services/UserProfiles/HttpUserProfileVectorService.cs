using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OleAI.Backend.Application.Services.AiRecommendation;

namespace OleAI.Backend.Application.Services.UserProfiles
{
    public class HttpUserProfileVectorService : IUserProfileVectorService
    {
        private readonly HttpClient _httpClient;
        private readonly AiServiceSettings _settings;
        private readonly ILogger<HttpUserProfileVectorService> _logger;

        public HttpUserProfileVectorService(
            HttpClient httpClient,
            IOptions<AiServiceSettings> settings,
            ILogger<HttpUserProfileVectorService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey);
        }

        public async Task UpdateProfileVectorAsync(Guid userId, Guid recipeId, string interactionType, CancellationToken cancellationToken)
        {
            _logger.LogDebug("User {UserId} interacted with {RecipeId} ({Type}).", userId, recipeId, interactionType);
            // AI tarafında interaction'lar recommendation isteği sırasında işleniyor.
        }

        public Task UpdateDietaryProfileAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
