using MediatR;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Application.Features.Pantry.Queries.GetResolvedRequests
{
    public class GetResolvedIngredientRequestsQuery : IRequest<Paginate<ResolvedRequestsResponse>>
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 20;
    }
}
