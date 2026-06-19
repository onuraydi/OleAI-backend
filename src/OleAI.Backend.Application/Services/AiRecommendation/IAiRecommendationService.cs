namespace OleAI.Backend.Application.Services.AiRecommendation
{
    /// <summary>
    /// AI öneri motoru ile iletişim kuran servis arayüzü.
    /// Gerçek implementasyon, Python/Scikit-learn tabanlı microservice ile HTTP üzerinden çalışacaktır.
    /// </summary>
    public interface IAiRecommendationService
    {
        /// <summary>
        /// Kullanıcının diyet tercihleri, alerjileri ve kısıtlarını içeren payload'ı
        /// AI motoruna ileterek kişiselleştirilmiş tarif önerileri alır.
        /// Fındık alerjisi gibi kısıtlar hard-filter olarak uygulanır.
        /// </summary>
        Task<AiRecommendationResponse> GetRecommendationsAsync(
            AiRecommendationPayload payload,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yeni bir tarif eklendiğinde veya güncellendiğinde AI modelini 
        /// incremental olarak güncellemek için webhook tetikler.
        /// </summary>
        Task SyncRecipeAsync(
            Guid recipeId,
            string title,
            List<RecipeIngredientAiDto> ingredients,
            string? category = null,
            List<string>? tags = null,
            bool isPublished = true,
            int popularity = 0,
            CancellationToken cancellationToken = default);
    }
}
