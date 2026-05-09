using MediatR;

namespace OleAI.Backend.Application.Features.Users.Commands.UpdateUserPreferences
{
    public class UpdateUserPreferencesCommand : IRequest<string>
    {
        public List<Guid> DietaryPreferenceIds { get; set; } = new();
    }
}
