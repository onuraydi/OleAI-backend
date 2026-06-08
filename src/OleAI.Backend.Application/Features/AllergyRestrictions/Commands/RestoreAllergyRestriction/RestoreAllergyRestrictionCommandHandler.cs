using MediatR;
using OleAI.Backend.Application.Features.AllergyRestrictions.Constants;
using OleAI.Backend.Application.Features.AllergyRestrictions.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Commands.RestoreAllergyRestriction;

public class RestoreAllergyRestrictionCommandHandler : IRequestHandler<RestoreAllergyRestrictionCommand, string>
{
    private readonly IAllergyRestrictionRepository _allergyRestrictionRepository;
    private readonly AllergyRestrictionBusinessRules _businessRules;

    public RestoreAllergyRestrictionCommandHandler(
        IAllergyRestrictionRepository allergyRestrictionRepository,
        AllergyRestrictionBusinessRules businessRules)
    {
        _allergyRestrictionRepository = allergyRestrictionRepository;
        _businessRules = businessRules;
    }

    public async Task<string> Handle(RestoreAllergyRestrictionCommand request, CancellationToken cancellationToken)
    {
        var restriction = await _allergyRestrictionRepository.GetAsync(
            predicate: ar => ar.Id == request.Id,
            withDeleted: true,
            cancellationToken: cancellationToken);

        _businessRules.CheckIfAllergyRestrictionExists(restriction);

        restriction!.DeletedDate = null;
        await _allergyRestrictionRepository.UpdateAsync(restriction, cancellationToken: cancellationToken);

        return "Alerji kısıtlaması başarıyla geri yüklendi.";
    }
}
