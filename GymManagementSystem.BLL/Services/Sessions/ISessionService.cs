using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Sessions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.BLL.Services.Sessions;

public interface ISessionService
{
    Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync();
    Task<SessionDetailsViewModel?> GetSessionDetailsAsync(int id);
    Task<CreateSessionViewModel> GetCreateSessionViewModelAsync();
    Task<Result<int>> CreateSessionAsync(CreateSessionViewModel model);
    Task<EditSessionViewModel?> GetSessionForEditAsync(int id);
    Task<Result> UpdateSessionAsync(EditSessionViewModel model);
    Task<DeleteSessionViewModel?> GetSessionForDeleteAsync(int id);
    Task<Result> DeleteSessionAsync(int id);

    // Lookup helpers for dropdowns ("Some Methods")
    Task<IEnumerable<SelectListItem>> GetCategoriesSelectListAsync();
    Task<IEnumerable<SelectListItem>> GetTrainersSelectListAsync();
}
