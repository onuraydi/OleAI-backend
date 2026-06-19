using System.Collections.Generic;
using MediatR;

namespace OleAI.Backend.Application.Features.Logs.Queries.GetLogFiles
{
    public class GetLogFilesQuery : IRequest<List<LogFileDto>>
    {
    }
}
