using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Features.Users.Constants;
using OleAI.Backend.Application.Features.Users.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Users.Commands.UpdateUserPreferences
{
    public class UpdateUserPreferencesCommandHandler : IRequestHandler<UpdateUserPreferencesCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserDietaryPreferenceRepository _preferenceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cacheService;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IUserProfileVectorService _vectorService;

        private const string ProfileCacheKeyPrefix = "user-profile:";

        public UpdateUserPreferencesCommandHandler(
            IUserRepository userRepository,
            IUserDietaryPreferenceRepository preferenceRepository,
            IHttpContextAccessor httpContextAccessor,
            ICacheService cacheService,
            UserBusinessRules userBusinessRules,
            IUserProfileVectorService vectorService)
        {
            _userRepository = userRepository;
            _preferenceRepository = preferenceRepository;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
            _userBusinessRules = userBusinessRules;
            _vectorService = vectorService;
        }

        public async Task<string> Handle(UpdateUserPreferencesCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(
                _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Kullanıcı varlık kontrolü
            var userExists = await _userRepository.AnyAsync(
                predicate: u => u.Id == userId,
                cancellationToken: cancellationToken);

            if (!userExists)
                throw new Core.CrossCuttingConcerns.Exceptions.Types.NotFoundException(UserMessages.UserNotFound);

            // Gönderilen ID'lerin geçerliliğini doğrula
            await _userBusinessRules.CheckIfDietaryPreferencesExistAsync(request.DietaryPreferenceIds, cancellationToken);

            // Hard Delete — mevcut tercihleri fiziksel olarak temizle
            await _preferenceRepository.ExecuteDeleteByUserIdAsync(userId, cancellationToken);

            // Yeni tercihleri ekle
            if (request.DietaryPreferenceIds.Count > 0)
            {
                var newPreferences = request.DietaryPreferenceIds
                    .Select(preferenceId => new UserDietaryPreference
                    {
                        UserId = userId,
                        DietaryPreferenceId = preferenceId
                    })
                    .ToList();

                await _preferenceRepository.AddRangeAsync(newPreferences, cancellationToken);
            }

            // Cache Invalidation
            await _cacheService.RemoveAsync($"{ProfileCacheKeyPrefix}{userId}");

            // AI Profil Vektör Senkronizasyonu (fire-and-forget)
            _ = _vectorService.UpdateDietaryProfileAsync(userId, CancellationToken.None);

            return UserMessages.DietaryPreferencesUpdatedSuccessfully;
        }
    }
}
