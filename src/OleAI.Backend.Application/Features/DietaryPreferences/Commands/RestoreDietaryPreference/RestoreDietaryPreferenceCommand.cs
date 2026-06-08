using MediatR;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Commands.RestoreDietaryPreference;

public class RestoreDietaryPreferenceCommand : IRequest<string>
{
    public Guid Id { get; set; }
}
