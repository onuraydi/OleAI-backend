using System.IO;
using MediatR;

namespace OleAI.Backend.Application.Features.Logs.Queries.GetLogFiles
{
    public class GetLogFilesQueryHandler : IRequestHandler<GetLogFilesQuery, List<LogFileDto>>
    {
        public Task<List<LogFileDto>> Handle(GetLogFilesQuery request, CancellationToken cancellationToken)
        {
            var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            var result = new List<LogFileDto>();

            if (Directory.Exists(logsDirectory))
            {
                var files = Directory.GetFiles(logsDirectory, "*.txt");
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    result.Add(new LogFileDto
                    {
                        Name = fileInfo.Name,
                        SizeBytes = fileInfo.Length,
                        LastModified = fileInfo.LastWriteTime
                    });
                }
            }

            // En yeniler en üstte
            return Task.FromResult(result.OrderByDescending(x => x.LastModified).ToList());
        }
    }
}
