using System;

namespace OleAI.Backend.Application.Features.Logs.Queries.GetLogFiles
{
    public class LogFileDto
    {
        public string Name { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public DateTime LastModified { get; set; }
    }
}
