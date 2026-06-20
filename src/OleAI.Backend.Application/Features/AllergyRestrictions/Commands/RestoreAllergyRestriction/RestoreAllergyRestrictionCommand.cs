using MediatR;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Commands.RestoreAllergyRestriction;

public class RestoreAllergyRestrictionCommand : IRequest<string>
{
    public Guid Id { get; set; }
}
