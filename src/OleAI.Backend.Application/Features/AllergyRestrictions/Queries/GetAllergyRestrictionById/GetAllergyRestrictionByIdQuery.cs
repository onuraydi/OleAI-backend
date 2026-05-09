using MediatR;
using OleAI.Backend.Application.Features.AllergyRestrictions.Dtos.Responses;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Queries.GetAllergyRestrictionById;

public class GetAllergyRestrictionByIdQuery : IRequest<AllergyRestrictionDetailResponse>
{
    public Guid Id { get; set; }
}
