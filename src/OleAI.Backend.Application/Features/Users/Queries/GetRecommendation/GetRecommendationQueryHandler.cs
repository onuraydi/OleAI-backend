using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Users.Rules;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Users.Queries.GetRecommendation
{
    public class GetRecommendationQueryHandler : IRequestHandler<GetRecommendationQuery, AiRecommendationResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAiRecommendationService _aiRecommendationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserBusinessRules _userBusinessRules;

        public GetRecommendationQueryHandler(
            IUserRepository userRepository,
            IAiRecommendationService aiRecommendationService,
            IHttpContextAccessor httpContextAccessor,
            UserBusinessRules userBusinessRules)
        {
            _userRepository = userRepository;
            _aiRecommendationService = aiRecommendationService;
            _httpContextAccessor = httpContextAccessor;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<AiRecommendationResponse> Handle(GetRecommendationQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(
                _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // N+1 problemi olmadan tek sorguda tüm ilişkisel grafiği Eager Loading ile çek
            var user = await _userRepository.GetAsync(
                predicate: u => u.Id == userId,
                include: q => q
                    .Include(u => u.UserDietaryPreferences)
                        .ThenInclude(udp => udp.DietaryPreference)
                            .ThenInclude(dp => dp.ExcludedIngredients)
                                .ThenInclude(ei => ei.Ingredient)
                    .Include(u => u.UserDietaryPreferences)
                        .ThenInclude(udp => udp.DietaryPreference)
                            .ThenInclude(dp => dp.ExcludedCategories)
                                .ThenInclude(ec => ec.Category)
                    .Include(u => u.UserAllergies)
                        .ThenInclude(ua => ua.AllergyRestriction)
                            .ThenInclude(ar => ar.ExcludedIngredients)
                                .ThenInclude(ei => ei.Ingredient)
                    .Include(u => u.UserAllergies)
                        .ThenInclude(ua => ua.AllergyRestriction)
                            .ThenInclude(ar => ar.ExcludedCategories)
                                .ThenInclude(ec => ec.Category)
                    .Include(u => u.UserDislikedIngredients)
                        .ThenInclude(udi => udi.Ingredient),
                enableTracking: false,
                cancellationToken: cancellationToken);

            _userBusinessRules.CheckIfUserExists(user);

            // Entity → AiRecommendationPayload mapping
            var payload = MapToPayload(userId, user!);

            return await _aiRecommendationService.GetRecommendationsAsync(payload, cancellationToken);
        }

        /// <summary>
        /// Kullanıcının diyet tercihleri ve alerjilerinden, AI motorunun beklediği düz listeye (flat list) dönüştürür.
        /// Hem diyet hem alerji kısıtlarından gelen dışlama listeleri birleştirilir ve tekrar eden öğeler elenir.
        /// </summary>
        private static AiRecommendationPayload MapToPayload(Guid userId, Domain.Entities.User user)
        {
            var excludedIngredients = new HashSet<string>();
            var excludedCategories = new HashSet<string>();

            // Diyet tercihlerinden gelen dışlamalar
            foreach (var udp in user.UserDietaryPreferences)
            {
                foreach (var ei in udp.DietaryPreference.ExcludedIngredients)
                    excludedIngredients.Add(ei.Ingredient.Name);

                foreach (var ec in udp.DietaryPreference.ExcludedCategories)
                    excludedCategories.Add(ec.Category.Name);
            }

            // Alerji kısıtlarından gelen dışlamalar
            foreach (var ua in user.UserAllergies)
            {
                foreach (var ei in ua.AllergyRestriction.ExcludedIngredients)
                    excludedIngredients.Add(ei.Ingredient.Name);

                foreach (var ec in ua.AllergyRestriction.ExcludedCategories)
                    excludedCategories.Add(ec.Category.Name);
            }

            // Sevmediği malzemelerden (DislikedIngredients) gelen dışlamalar
            foreach (var udi in user.UserDislikedIngredients)
            {
                excludedIngredients.Add(udi.Ingredient.Name);
            }

            return new AiRecommendationPayload
            {
                UserId = userId,
                DietaryPreferences = user.UserDietaryPreferences
                    .Select(udp => udp.DietaryPreference.Name)
                    .ToList(),
                Allergies = user.UserAllergies
                    .Select(ua => ua.AllergyRestriction.Name)
                    .ToList(),
                ExcludedIngredients = excludedIngredients.ToList(),
                ExcludedCategories = excludedCategories.ToList()
            };
        }
    }
}
