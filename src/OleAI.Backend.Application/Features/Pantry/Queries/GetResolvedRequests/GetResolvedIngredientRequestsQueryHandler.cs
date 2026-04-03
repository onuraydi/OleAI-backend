using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Pantry.Queries.GetResolvedRequests
{
    public class GetResolvedIngredientRequestsQueryHandler : IRequestHandler<GetResolvedIngredientRequestsQuery, Paginate<ResolvedRequestsResponse>>
    {
        private readonly IIngredientRequestRepository _ingredientRequestRepository;
        private readonly IMapper _mapper;

        public GetResolvedIngredientRequestsQueryHandler(IIngredientRequestRepository ingredientRequestRepository, IMapper mapper)
        {
            _ingredientRequestRepository = ingredientRequestRepository;
            _mapper = mapper;
        }
        // TODO BURADA Çeşitli sorunlar olabilir
        public async Task<Paginate<ResolvedRequestsResponse>> Handle(GetResolvedIngredientRequestsQuery request, CancellationToken cancellationToken)
        {
            QueryOptions<IngredientRequest> query = new QueryOptions<IngredientRequest>
            {
                Predicate = r => r.Status != IngredientRequestStatus.Pending,
                OrderBy = r => r.OrderByDescending(x => x.CreatedDate),
                Index = request.Page,
                Size = request.Size,
                EnableTracking = false
            };

            var resolvedRequests = await _ingredientRequestRepository.GetListAsync(query, cancellationToken);

            var mappedRequest = _mapper.Map<List<ResolvedRequestsResponse>>(resolvedRequests.Items);

            return new Paginate<ResolvedRequestsResponse>
            {
                Items = mappedRequest,
                PageNumber = resolvedRequests.PageNumber,
                PageSize = resolvedRequests.PageSize,
                TotalItems = resolvedRequests.TotalItems,
                TotalPages = resolvedRequests.TotalPages,
            };
        }
    }
}
