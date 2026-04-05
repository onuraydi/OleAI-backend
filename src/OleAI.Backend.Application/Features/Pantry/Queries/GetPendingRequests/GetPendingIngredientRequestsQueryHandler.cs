using MediatR;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Pantry.Queries.GetPendingRequests
{
    public class GetPendingIngredientRequestsQueryHandler : IRequestHandler<GetPendingIngredientRequestsQuery, List<PendingRequestDto>>
    {
        private readonly IIngredientRequestRepository _requestRepository;

        public GetPendingIngredientRequestsQueryHandler(IIngredientRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<List<PendingRequestDto>> Handle(GetPendingIngredientRequestsQuery request, CancellationToken cancellationToken)
        {
            QueryOptions<IngredientRequest> query = new QueryOptions<IngredientRequest>();
            query.Predicate = r => r.Status == IngredientRequestStatus.Pending;
            query.OrderBy = r => r.OrderBy(x => x.CreatedDate);
            query.EnableTracking = false;

            var pendingRequests = await _requestRepository.GetListAsync(query, cancellationToken);

            return pendingRequests.Items.Select(r => new PendingRequestDto
            {
                Id = r.Id,
                UserId = r.UserId,
                RequestedName = r.LlmAnalysisResult,
                CreatedDate = r.CreatedDate
            }).ToList();
        }
    }
}
