using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Features.Users.Constants;
using OleAI.Backend.Application.Features.Users.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Users.Commands.UpdateUserAllergies
{
    public class UpdateUserAllergiesCommandHandler : IRequestHandler<UpdateUserAllergiesCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserAllergyRepository _allergyRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cacheService;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IUserProfileVectorService _vectorService;

        private const string ProfileCacheKeyPrefix = "user-profile:";

        public UpdateUserAllergiesCommandHandler(
            IUserRepository userRepository,
            IUserAllergyRepository allergyRepository,
            IHttpContextAccessor httpContextAccessor,
            ICacheService cacheService,
            UserBusinessRules userBusinessRules,
            IUserProfileVectorService vectorService)
        {
            _userRepository = userRepository;
            _allergyRepository = allergyRepository;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
            _userBusinessRules = userBusinessRules;
            _vectorService = vectorService;
        }

        public async Task<string> Handle(UpdateUserAllergiesCommand request, CancellationToken cancellationToken)
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
            await _userBusinessRules.CheckIfAllergyRestrictionsExistAsync(request.AllergyRestrictionIds, cancellationToken);

            // Hard Delete — mevcut alerji kayıtlarını fiziksel olarak temizle
            await _allergyRepository.ExecuteDeleteByUserIdAsync(userId, cancellationToken);

            // Yeni alerji kayıtlarını ekle
            if (request.AllergyRestrictionIds.Count > 0)
            {
                var newAllergies = request.AllergyRestrictionIds
                    .Select(allergyId => new UserAllergy
                    {
                        UserId = userId,
                        AllergyRestrictionId = allergyId
                    })
                    .ToList();

                await _allergyRepository.AddRangeAsync(newAllergies, cancellationToken);
            }

            // Cache Invalidation
            await _cacheService.RemoveAsync($"{ProfileCacheKeyPrefix}{userId}");

            // AI Profil Vektör Senkronizasyonu (fire-and-forget)
            _ = _vectorService.UpdateDietaryProfileAsync(userId, CancellationToken.None);

            return UserMessages.AllergiesUpdatedSuccessfully;
        }
    }
}
