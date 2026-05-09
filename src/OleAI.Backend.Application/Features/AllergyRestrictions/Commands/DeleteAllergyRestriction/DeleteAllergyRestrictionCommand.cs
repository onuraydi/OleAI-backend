using MediatR;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Commands.DeleteAllergyRestriction;

public class DeleteAllergyRestrictionCommand : IRequest<string>
{
    public Guid Id { get; set; }
}
