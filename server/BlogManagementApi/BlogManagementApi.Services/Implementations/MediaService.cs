using BlogManagementApi.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlogManagementApi.Services.Implementations
{
    public class MediaService : IMediaService
    {
        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        private readonly IHostEnvironment _env;
        private readonly ILogger<MediaService> _logger;

        public MediaService(IHostEnvironment env, ILogger<MediaService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<string> UploadAsync(Stream stream, string fileName)
        {
            if (stream is null || stream.Length == 0)
                throw new InvalidOperationException("No file provided.");

            if (stream.Length > MaxFileSizeBytes)
                throw new InvalidOperationException("File exceeds the 5MB size limit.");

            var extension = Path.GetExtension(fileName);
            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException("Invalid file type. Only jpg, png, gif, and webp are allowed.");

            var now = DateTime.UtcNow;
            var relativePath = Path.Combine("wwwroot", "uploads", now.Year.ToString(), now.Month.ToString("D2"));
            var fullDir = Path.Combine(_env.ContentRootPath, relativePath);

            Directory.CreateDirectory(fullDir);

            var savedFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(fullDir, savedFileName);

            await using var fileStream = new FileStream(fullPath, FileMode.Create);
            await stream.CopyToAsync(fileStream);

            var url = $"/uploads/{now.Year}/{now.Month:D2}/{savedFileName}";

            _logger.LogInformation("File uploaded: {FileName} -> {Url}", fileName, url);

            return url;
        }
    }
}
