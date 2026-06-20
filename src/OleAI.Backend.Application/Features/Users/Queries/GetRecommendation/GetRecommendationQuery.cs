using MediatR;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;
using OleAI.Backend.Application.Services.AiRecommendation;

namespace OleAI.Backend.Application.Features.Users.Queries.GetRecommendation
{
    public class GetRecommendationQuery : IRequest<List<RecipeRecommendationDto>>
    {
        /// <summary>
        /// Öneri motoru çalışma modu. (Default, Ultimate, Strict, ForYou, Quick, Explore, Healthy, MinimalShopping)
        /// Her mod, malzemelerin ve kullanıcı tercihlerinin ağırlığını farklı şekilde hesaplar.
        /// </summary>
        public RecommendationMode Mode { get; set; } = RecommendationMode.Default;

        /// <summary>
        /// İsraf önleyici (Zero Waste) modu. True ise son kullanma tarihi 3 günden az kalan ürünlere Aciliyet Bonusu eklenir.
        /// </summary>
        public bool UseZeroWaste { get; set; } = false;

        /// <summary>
        /// Alerjen malzemeleri filtrele. (Katı Filtre)
        /// </summary>
        public bool UseAllergyFilters { get; set; } = true;

        /// <summary>
        /// Diyet tercihlerini filtrele (Vegan vb.). (Katı Filtre)
        /// </summary>
        public bool UseDietFilters { get; set; } = true;

        /// <summary>
        /// Sevilmeyen malzemeleri filtrele. (Katı Filtre)
        /// </summary>
        public bool UseDislikedFilters { get; set; } = true;
    }
}
