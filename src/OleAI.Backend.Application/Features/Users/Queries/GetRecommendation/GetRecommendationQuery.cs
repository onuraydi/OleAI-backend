using MediatR;
using OleAI.Backend.Application.Services.AiRecommendation;

namespace OleAI.Backend.Application.Features.Users.Queries.GetRecommendation
{
    public class GetRecommendationQuery : IRequest<AiRecommendationResponse>
    {
        /// <summary>
        /// Öneri motoru çalışma modu. (Default, Ultimate, Strict, ForYou, Quick, Explore, Healthy, MinimalShopping)
        /// Her mod, malzemelerin ve kullanıcı tercihlerinin ağırlığını farklı şekilde hesaplar.
        /// </summary>
        public RecommendationMode Mode { get; set; } = RecommendationMode.Default;
    }
}
