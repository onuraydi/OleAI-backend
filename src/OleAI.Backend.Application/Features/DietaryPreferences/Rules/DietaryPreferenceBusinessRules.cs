using OleAI.Backend.Application.Features.DietaryPreferences.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Rules;

public class DietaryPreferenceBusinessRules
{
    private readonly IDietaryPreferenceRepository _dietaryPreferenceRepository;

    public DietaryPreferenceBusinessRules(IDietaryPreferenceRepository dietaryPreferenceRepository)
    {
        _dietaryPreferenceRepository = dietaryPreferenceRepository;
    }

    public async Task CheckIfDietaryPreferenceNameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        var exists = await _dietaryPreferenceRepository.AnyAsync(
            predicate: dp => dp.Name.ToLower() == name.ToLower(),
            cancellationToken: cancellationToken);

        if (exists)
            throw new BusinessException(DietaryPreferenceMessages.DietaryPreferenceNameAlreadyExists);
    }

    public async Task CheckIfDietaryPreferenceNameExistsWhenUpdatingAsync(string name, Guid currentId, CancellationToken cancellationToken = default)
    {
        var exists = await _dietaryPreferenceRepository.AnyAsync(
            predicate: dp => dp.Name.ToLower() == name.ToLower() && dp.Id != currentId,
            cancellationToken: cancellationToken);

        if (exists)
            throw new BusinessException(DietaryPreferenceMessages.DietaryPreferenceNameAlreadyExists);
    }

    public void CheckIfDietaryPreferenceExists(DietaryPreference? dietaryPreference)
    {
        if (dietaryPreference is null)
            throw new NotFoundException(DietaryPreferenceMessages.DietaryPreferenceNotFound);
    }
}
