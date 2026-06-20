using MediatR;

namespace OleAI.Backend.Application.Features.Logs.Queries.DownloadLogFile
{
    public class DownloadLogFileQuery : IRequest<byte[]>
    {
        public string FileName { get; set; } = string.Empty;
    }
}
