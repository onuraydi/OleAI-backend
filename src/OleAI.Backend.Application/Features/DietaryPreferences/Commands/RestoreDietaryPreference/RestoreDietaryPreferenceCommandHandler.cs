using MediatR;
using OleAI.Backend.Application.Features.DietaryPreferences.Constants;
using OleAI.Backend.Application.Features.DietaryPreferences.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Commands.RestoreDietaryPreference;

public class RestoreDietaryPreferenceCommandHandler : IRequestHandler<RestoreDietaryPreferenceCommand, string>
{
    private readonly IDietaryPreferenceRepository _dietaryPreferenceRepository;
    private readonly DietaryPreferenceBusinessRules _businessRules;

    public RestoreDietaryPreferenceCommandHandler(
        IDietaryPreferenceRepository dietaryPreferenceRepository,
        DietaryPreferenceBusinessRules businessRules)
    {
        _dietaryPreferenceRepository = dietaryPreferenceRepository;
        _businessRules = businessRules;
    }

    public async Task<string> Handle(RestoreDietaryPreferenceCommand request, CancellationToken cancellationToken)
    {
        var dietaryPreference = await _dietaryPreferenceRepository.GetAsync(
            predicate: dp => dp.Id == request.Id,
            withDeleted: true,
            cancellationToken: cancellationToken);

        _businessRules.CheckIfDietaryPreferenceExists(dietaryPreference);

        dietaryPreference!.DeletedDate = null;

        await _dietaryPreferenceRepository.UpdateAsync(dietaryPreference, cancellationToken);

        return "Diyet kısıtlaması başarıyla geri yüklendi.";
    }
}
