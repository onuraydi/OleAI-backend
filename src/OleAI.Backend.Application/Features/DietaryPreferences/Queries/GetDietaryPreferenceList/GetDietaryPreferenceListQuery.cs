using MediatR;
using OleAI.Backend.Application.Features.DietaryPreferences.Dtos.Responses;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Queries.GetDietaryPreferenceList;

public class GetDietaryPreferenceListQuery : IRequest<List<DietaryPreferenceListResponse>>
{
}
