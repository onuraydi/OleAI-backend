using MediatR;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Commands.DeleteDietaryPreference;

public class DeleteDietaryPreferenceCommand : IRequest<string>
{
    public Guid Id { get; set; }
}
