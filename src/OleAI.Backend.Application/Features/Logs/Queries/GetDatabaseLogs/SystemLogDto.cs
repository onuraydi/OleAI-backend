using System;

namespace OleAI.Backend.Application.Features.Logs.Queries.GetDatabaseLogs
{
    public class SystemLogDto
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public string? Level { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string? Exception { get; set; }
        public string? Properties { get; set; }
        public string? CorrelationId { get; set; }
    }
}
