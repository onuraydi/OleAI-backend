using OleAI.Backend.Application.Features.AllergyRestrictions.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Rules;

public class AllergyRestrictionBusinessRules
{
    private readonly IAllergyRestrictionRepository _allergyRestrictionRepository;

    public AllergyRestrictionBusinessRules(IAllergyRestrictionRepository allergyRestrictionRepository)
    {
        _allergyRestrictionRepository = allergyRestrictionRepository;
    }

    public async Task CheckIfAllergyRestrictionNameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        var exists = await _allergyRestrictionRepository.AnyAsync(
            predicate: ar => ar.Name.ToLower() == name.ToLower(),
            cancellationToken: cancellationToken);

        if (exists)
            throw new BusinessException(AllergyRestrictionMessages.AllergyRestrictionNameAlreadyExists);
    }

    public async Task CheckIfAllergyRestrictionNameExistsWhenUpdatingAsync(string name, Guid currentId, CancellationToken cancellationToken = default)
    {
        var exists = await _allergyRestrictionRepository.AnyAsync(
            predicate: ar => ar.Name.ToLower() == name.ToLower() && ar.Id != currentId,
            cancellationToken: cancellationToken);

        if (exists)
            throw new BusinessException(AllergyRestrictionMessages.AllergyRestrictionNameAlreadyExists);
    }

    public void CheckIfAllergyRestrictionExists(AllergyRestriction? allergyRestriction)
    {
        if (allergyRestriction is null)
            throw new NotFoundException(AllergyRestrictionMessages.AllergyRestrictionNotFound);
    }
}
