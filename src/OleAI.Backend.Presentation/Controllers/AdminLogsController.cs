using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Logs.Queries.DownloadLogFile;
using OleAI.Backend.Application.Features.Logs.Queries.GetDatabaseLogs;
using OleAI.Backend.Application.Features.Logs.Queries.GetLogFiles;

namespace OleAI.Backend.Presentation.Controllers
{
    [Route("api/admin/logs")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminLogsController : BaseController
    {
        /// <summary>
        /// Veritabanına (SQL Server) yazılmış olan yapısal Serilog kayıtlarını sayfalı şekilde getirir.
        /// </summary>
        [HttpGet("db")]
        public async Task<IActionResult> GetDatabaseLogs([FromQuery] GetDatabaseLogsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Dosya sistemine (logs klasörü) yazılmış .txt uzantılı log dosyalarının listesini döndürür.
        /// </summary>
        [HttpGet("files")]
        public async Task<IActionResult> GetLogFiles()
        {
            var result = await Mediator.Send(new GetLogFilesQuery());
            return Ok(result);
        }

        /// <summary>
        /// Dosya sistemindeki belirtilen log dosyasını (örn: oleai-log-2026-06-14.txt) indirir veya okur.
        /// </summary>
        [HttpGet("files/{fileName}")]
        public async Task<IActionResult> DownloadLogFile([FromRoute] string fileName)
        {
            var query = new DownloadLogFileQuery { FileName = fileName };
            var fileBytes = await Mediator.Send(query);

            return File(fileBytes, "text/plain", fileName);
        }
    }
}
