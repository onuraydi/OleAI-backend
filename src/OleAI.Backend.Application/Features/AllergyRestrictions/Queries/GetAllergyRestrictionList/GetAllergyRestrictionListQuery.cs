using MediatR;
using OleAI.Backend.Application.Features.AllergyRestrictions.Dtos.Responses;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Queries.GetAllergyRestrictionList;

public class GetAllergyRestrictionListQuery : IRequest<List<AllergyRestrictionListResponse>>
{
}
