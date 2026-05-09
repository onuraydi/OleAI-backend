using MediatR;
using OleAI.Backend.Application.Features.AllergyRestrictions.Constants;
using OleAI.Backend.Application.Features.AllergyRestrictions.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Commands.DeleteAllergyRestriction;

public class DeleteAllergyRestrictionCommandHandler : IRequestHandler<DeleteAllergyRestrictionCommand, string>
{
    private readonly IAllergyRestrictionRepository _allergyRestrictionRepository;
    private readonly AllergyRestrictionBusinessRules _businessRules;

    public DeleteAllergyRestrictionCommandHandler(
        IAllergyRestrictionRepository allergyRestrictionRepository,
        AllergyRestrictionBusinessRules businessRules)
    {
        _allergyRestrictionRepository = allergyRestrictionRepository;
        _businessRules = businessRules;
    }

    public async Task<string> Handle(DeleteAllergyRestrictionCommand request, CancellationToken cancellationToken)
    {
        var restriction = await _allergyRestrictionRepository.GetAsync(
            predicate: ar => ar.Id == request.Id,
            cancellationToken: cancellationToken);

        _businessRules.CheckIfAllergyRestrictionExists(restriction);

        await _allergyRestrictionRepository.DeleteAsync(restriction!, permanent: false, cancellationToken: cancellationToken);

        return AllergyRestrictionMessages.AllergyRestrictionDeletedSuccessfully;
    }
}
