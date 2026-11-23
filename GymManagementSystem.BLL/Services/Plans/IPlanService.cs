using GymManagementSystem.BLL.ViewModels.Plans;

namespace GymManagementSystem.BLL.Services.Plans;

public interface IPlanService
{
    Task<IEnumerable<PlanViewModel>> GetAllPlansAsync();
    Task<PlanDetailsViewModel?> GetPlanDetailsAsync(int id);
    Task<EditPlanViewModel?> GetPlanForEditAsync(int id);
    Task<(bool Success, string? ErrorMessage)> UpdatePlanAsync(EditPlanViewModel model);
    Task<(bool Success, string? ErrorMessage)> ToggleStatusAsync(int id);
}
