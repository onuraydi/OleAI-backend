using MediatR;

namespace OleAI.Backend.Application.Features.Users.Commands.UpdateUserAllergies
{
    public class UpdateUserAllergiesCommand : IRequest<string>
    {
        public List<Guid> AllergyRestrictionIds { get; set; } = new();
    }
}
