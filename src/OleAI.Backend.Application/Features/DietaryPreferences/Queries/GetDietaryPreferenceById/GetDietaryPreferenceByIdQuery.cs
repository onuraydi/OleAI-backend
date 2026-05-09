using MediatR;
using OleAI.Backend.Application.Features.DietaryPreferences.Dtos.Responses;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Queries.GetDietaryPreferenceById;

public class GetDietaryPreferenceByIdQuery : IRequest<DietaryPreferenceDetailResponse>
{
    public Guid Id { get; set; }
}
