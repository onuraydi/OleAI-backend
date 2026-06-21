using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace OleAI.Backend.Application.Services.ImageStorage
{
    public class CloudinaryImageManager : IImageService
    {
        private readonly Cloudinary _cloudinary = null!;
        private readonly bool _isConfigured;

        public CloudinaryImageManager(IOptions<CloudinarySettings> cloudinarySettings)
        {
            var settings = cloudinarySettings.Value;

            if (string.IsNullOrWhiteSpace(settings.CloudName) ||
                string.IsNullOrWhiteSpace(settings.ApiKey) ||
                string.IsNullOrWhiteSpace(settings.ApiSecret))
            {
                _isConfigured = false;
                return;
            }

            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
            _isConfigured = true;
        }

        public async Task<bool> DeleteAsync(string publicId, CancellationToken cancellationToken = default)
        {
            if (!_isConfigured) return false;

            if (string.IsNullOrEmpty(publicId))
                return false;

            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok";
        }

        public async Task<(string Url, string PublicId)> UploadAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default)
        {
            if (!_isConfigured)
                throw new InvalidOperationException("Resim yükleme servisi (Cloudinary) yapılandırılmamış. Lütfen sunucu ayarlarını (appsettings/env) kontrol edin.");

            if (file == null || file.Length == 0)
                throw new ArgumentException("Yüklenmek istenen dosya boş olamaz.");

            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folderName,
                Format = "webp",
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

            if (uploadResult.Error != null)
                throw new Exception($"cloudinary yükleme hatası: {uploadResult.Error.Message}");

            return (uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
        }
    }
}
