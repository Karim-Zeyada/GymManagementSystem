using GymManagementSystem.BLL.ViewModels.Members;

namespace GymManagementSystem.BLL.Services.Members;

public interface IMemberService
{
    Task<IEnumerable<MemberViewModel>> GetAllMembersAsync();
    Task<MemberDetailsViewModel?> GetMemberDetailsAsync(int id);
    Task<HealthRecordDetailsViewModel?> GetHealthRecordDetailsAsync(int memberId);
    Task<EditMemberViewModel?> GetMemberForEditAsync(int id);
    Task<DeleteMemberViewModel?> GetMemberForDeleteAsync(int id);
    Task<(bool Success, string? ErrorMessage)> CreateMemberAsync(CreateMemberViewModel model);
    Task<(bool Success, string? ErrorMessage)> UpdateMemberAsync(EditMemberViewModel model);
    Task<(bool Success, string? ErrorMessage)> DeleteMemberAsync(int id);
}
