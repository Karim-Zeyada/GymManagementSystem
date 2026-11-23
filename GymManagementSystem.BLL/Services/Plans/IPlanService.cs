using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Plans;

namespace GymManagementSystem.BLL.Services.Plans;

public interface IPlanService
{
    Task<IEnumerable<PlanViewModel>> GetAllPlansAsync();
    Task<PlanDetailsViewModel?> GetPlanDetailsAsync(int id);
    Task<EditPlanViewModel?> GetPlanForEditAsync(int id);
    Task<Result> UpdatePlanAsync(EditPlanViewModel model);
    Task<Result> ToggleStatusAsync(int id);
}
