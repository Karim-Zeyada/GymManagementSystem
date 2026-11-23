using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Members;

namespace GymManagementSystem.BLL.Services.Members;

public interface IMemberService
{
    Task<IEnumerable<MemberViewModel>> GetAllMembersAsync();
    Task<MemberDetailsViewModel?> GetMemberDetailsAsync(int id);
    Task<HealthRecordDetailsViewModel?> GetHealthRecordDetailsAsync(int memberId);
    Task<EditMemberViewModel?> GetMemberForEditAsync(int id);
    Task<DeleteMemberViewModel?> GetMemberForDeleteAsync(int id);
    Task<Result> CreateMemberAsync(CreateMemberViewModel model);
    Task<Result> UpdateMemberAsync(EditMemberViewModel model);
    Task<Result> DeleteMemberAsync(int id);
}
