using GymManagementSystem.BLL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace GymManagementSystem.BLL.Services.Attachments;

public class AttachmentService : IAttachmentService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<AttachmentService> _logger;

    private static readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png"];
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

    public AttachmentService(IWebHostEnvironment webHostEnvironment, ILogger<AttachmentService> logger)
    {
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    public async Task<Result<string>> SaveAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("Attachment upload failed: File is null or empty.");
            return Result.Fail<string>("No file was provided.");
        }

        if (file.Length > MaxFileSizeInBytes)
        {
            _logger.LogWarning("Attachment upload failed: File size ({Size} bytes) exceeds 5 MB limit.", file.Length);
            return Result.Fail<string>("File size cannot exceed 5 MB.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            _logger.LogWarning("Attachment upload failed: Invalid extension '{Extension}'.", extension);
            return Result.Fail<string>("Only JPG, JPEG, and PNG image files are allowed.");
        }

        // SkiaSharp validation: verify true image content to prevent disguised scripts
        using var stream = file.OpenReadStream();
        using var codec = SKCodec.Create(stream);
        if (codec is null)
        {
            _logger.LogWarning("Attachment upload failed: File binary header is not a valid image format.");
            return Result.Fail<string>("Invalid image file.");
        }

        var fileName = $"{Guid.NewGuid()}{extension}";
        var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", folderName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var filePath = Path.Combine(folderPath, fileName);
        await using var outputStream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(outputStream);

        _logger.LogInformation("Attachment saved successfully to '{FilePath}'.", filePath);
        return Result.Ok(fileName);
    }

    public Result Delete(string? fileName, string folderName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Result.Ok();
        }

        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", folderName, fileName);
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                _logger.LogInformation("Attachment deleted successfully from '{FilePath}'.", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete attachment at '{FilePath}'.", filePath);
                return Result.Fail($"Failed to delete attachment: {ex.Message}");
            }
        }

        return Result.Ok();
    }
}
