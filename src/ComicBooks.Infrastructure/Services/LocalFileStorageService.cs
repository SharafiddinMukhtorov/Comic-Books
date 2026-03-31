using ComicBooks.Application.Common.Interfaces;

namespace ComicBooks.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadPath;

    public LocalFileStorageService()
    {
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(_uploadPath);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var uniqueFileName = $"{Guid.NewGuid():N}_{fileName}";
        var filePath = Path.Combine(_uploadPath, uniqueFileName);

        await using var fileOutput = File.Create(filePath);
        await fileStream.CopyToAsync(fileOutput, cancellationToken);

        return $"/uploads/{uniqueFileName}";
    }

    public Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) return Task.CompletedTask;
        var fileName = Path.GetFileName(fileUrl);
        var filePath = Path.Combine(_uploadPath, fileName);
        if (File.Exists(filePath)) File.Delete(filePath);
        return Task.CompletedTask;
    }
}
