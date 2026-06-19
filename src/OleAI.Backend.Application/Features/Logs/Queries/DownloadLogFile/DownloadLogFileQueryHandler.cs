using System;
using System.IO;
using MediatR;

namespace OleAI.Backend.Application.Features.Logs.Queries.DownloadLogFile
{
    public class DownloadLogFileQueryHandler : IRequestHandler<DownloadLogFileQuery, byte[]>
    {
        public async Task<byte[]> Handle(DownloadLogFileQuery request, CancellationToken cancellationToken)
        {
            var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");

            // Path traversal koruması
            if (request.FileName.Contains("..") || request.FileName.Contains("/") || request.FileName.Contains("\\"))
            {
                throw new Exception("Geçersiz dosya adı.");
            }

            var filePath = Path.Combine(logsDirectory, request.FileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Log dosyası bulunamadı.");
            }

            // Dosya loglama esnasında başka bir process (Serilog) tarafından kullanımda olabileceği için
            // FileShare.ReadWrite ile açıyoruz ki kilitlenme olmasın.
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream, cancellationToken);
                return memoryStream.ToArray();
            }
        }
    }
}
