using MediatR;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;

namespace OleAI.Backend.Application.Features.Pantry.Queries.GetPendingRequests
{
    public class GetPendingIngredientRequestsQuery : IRequest<List<PendingRequestDto>>
    {
    }
}
