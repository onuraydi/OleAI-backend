using MediatR;
using OleAI.Backend.Application.Features.DietaryPreferences.Constants;
using OleAI.Backend.Application.Features.DietaryPreferences.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Commands.DeleteDietaryPreference;

public class DeleteDietaryPreferenceCommandHandler : IRequestHandler<DeleteDietaryPreferenceCommand, string>
{
    private readonly IDietaryPreferenceRepository _dietaryPreferenceRepository;
    private readonly DietaryPreferenceBusinessRules _businessRules;

    public DeleteDietaryPreferenceCommandHandler(
        IDietaryPreferenceRepository dietaryPreferenceRepository,
        DietaryPreferenceBusinessRules businessRules)
    {
        _dietaryPreferenceRepository = dietaryPreferenceRepository;
        _businessRules = businessRules;
    }

    public async Task<string> Handle(DeleteDietaryPreferenceCommand request, CancellationToken cancellationToken)
    {
        var dietaryPreference = await _dietaryPreferenceRepository.GetAsync(
            predicate: dp => dp.Id == request.Id,
            cancellationToken: cancellationToken);

        _businessRules.CheckIfDietaryPreferenceExists(dietaryPreference);

        await _dietaryPreferenceRepository.DeleteAsync(dietaryPreference!, permanent: false, cancellationToken: cancellationToken);

        return DietaryPreferenceMessages.DietaryPreferenceDeletedSuccessfully;
    }
}
