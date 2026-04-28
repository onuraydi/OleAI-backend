using Microsoft.AspNetCore.Http;

namespace OleAI.Backend.Application.Services.ImageStorage;

public interface IImageService
{
    Task<(string Url, string PublicId)> UploadAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string publicId, CancellationToken cancellationToken = default);
}
