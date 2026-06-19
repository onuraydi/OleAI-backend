using MediatR;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Application.Features.Logs.Queries.GetDatabaseLogs
{
    public class GetDatabaseLogsQuery : IRequest<Paginate<SystemLogDto>>
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 20;
        public string? Level { get; set; }
        public string? CorrelationId { get; set; }
        public string? Keyword { get; set; }
    }
}
