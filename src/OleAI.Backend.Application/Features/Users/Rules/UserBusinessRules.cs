using OleAI.Backend.Application.Features.Users.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Users.Rules
{
    public class UserBusinessRules
    {
        private readonly IDietaryPreferenceRepository _dietaryPreferenceRepository;
        private readonly IAllergyRestrictionRepository _allergyRestrictionRepository;

        public UserBusinessRules(
            IDietaryPreferenceRepository dietaryPreferenceRepository,
            IAllergyRestrictionRepository allergyRestrictionRepository)
        {
            _dietaryPreferenceRepository = dietaryPreferenceRepository;
            _allergyRestrictionRepository = allergyRestrictionRepository;
        }

        /// <summary>
        /// Kullanıcının veritabanında var olup olmadığını kontrol eder.
        /// </summary>
        public void CheckIfUserExists(User? user)
        {
            if (user is null)
                throw new NotFoundException(UserMessages.UserNotFound);
        }

        /// <summary>
        /// Gönderilen beslenme tercihi ID'lerinin tamamının sistemde aktif olarak kayıtlı olduğunu doğrular.
        /// </summary>
        public async Task CheckIfDietaryPreferencesExistAsync(List<Guid> preferenceIds, CancellationToken cancellationToken)
        {
            if (preferenceIds.Count == 0) return;

            var existingCount = await _dietaryPreferenceRepository.GetCountAsync(
                predicate: dp => preferenceIds.Contains(dp.Id) && dp.IsActive,
                cancellationToken: cancellationToken);

            if (existingCount != preferenceIds.Count)
                throw new BusinessException(UserMessages.InvalidDietaryPreferencesSelected);
        }

        /// <summary>
        /// Gönderilen alerji kısıtı ID'lerinin tamamının sistemde aktif olarak kayıtlı olduğunu doğrular.
        /// </summary>
        public async Task CheckIfAllergyRestrictionsExistAsync(List<Guid> allergyIds, CancellationToken cancellationToken)
        {
            if (allergyIds.Count == 0) return;

            var existingCount = await _allergyRestrictionRepository.GetCountAsync(
                predicate: ar => allergyIds.Contains(ar.Id) && ar.IsActive,
                cancellationToken: cancellationToken);

            if (existingCount != allergyIds.Count)
                throw new BusinessException(UserMessages.InvalidAllergyRestrictionsSelected);
        }
    }
}
