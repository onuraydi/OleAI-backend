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
    }
}
