using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Memberships;

namespace GymManagementSystem.BLL.Services.Memberships;

public interface IMembershipService
{
    Task<IEnumerable<MembershipViewModel>> GetAllMembershipsAsync(string? filter = null);
    Task<MembershipDetailsViewModel?> GetMembershipDetailsAsync(int id);
    Task<CreateMembershipViewModel> GetCreateMembershipViewModelAsync(int? memberId = null);
    Task<Result> CreateMembershipAsync(CreateMembershipViewModel model);
    Task<Result> CancelMembershipAsync(int id);
}
