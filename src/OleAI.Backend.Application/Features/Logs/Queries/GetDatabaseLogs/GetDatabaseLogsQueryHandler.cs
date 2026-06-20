using MediatR;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Logs.Queries.GetDatabaseLogs
{
    public class GetDatabaseLogsQueryHandler : IRequestHandler<GetDatabaseLogsQuery, Paginate<SystemLogDto>>
    {
        private readonly ISystemLogRepository _systemLogRepository;

        public GetDatabaseLogsQueryHandler(ISystemLogRepository systemLogRepository)
        {
            _systemLogRepository = systemLogRepository;
        }

        public async Task<Paginate<SystemLogDto>> Handle(GetDatabaseLogsQuery request, CancellationToken cancellationToken)
        {
            QueryOptions<SystemLog> query = new QueryOptions<SystemLog>();
            query.Index = request.Page;
            query.Size = request.Size;
            query.EnableTracking = false;
            query.OrderBy = q => q.OrderByDescending(x => x.TimeStamp);

            query.Predicate = log =>
                (string.IsNullOrEmpty(request.Level) || log.Level == request.Level) &&
                (string.IsNullOrEmpty(request.CorrelationId) || log.CorrelationId == request.CorrelationId) &&
                (string.IsNullOrEmpty(request.Keyword) || (log.Message != null && log.Message.Contains(request.Keyword)));

            var logs = await _systemLogRepository.GetListAsync(query, cancellationToken: cancellationToken);

            var items = logs.Items.Select(log => new SystemLogDto
            {
                Id = log.Id,
                Message = log.Message,
                Level = log.Level,
                TimeStamp = log.TimeStamp,
                Exception = log.Exception,
                Properties = log.Properties,
                CorrelationId = log.CorrelationId
            }).ToList();

            return new Paginate<SystemLogDto>
            {
                PageNumber = logs.PageNumber,
                PageSize = logs.PageSize,
                TotalItems = logs.TotalItems,
                TotalPages = logs.TotalPages,
                Items = items
            };
        }
    }
}
