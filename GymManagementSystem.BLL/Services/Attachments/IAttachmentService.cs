using GymManagementSystem.BLL.Common;
using Microsoft.AspNetCore.Http;

namespace GymManagementSystem.BLL.Services.Attachments;

public interface IAttachmentService
{
    Task<Result<string>> SaveAsync(IFormFile file, string folderName);
    Result Delete(string? fileName, string folderName);
}
