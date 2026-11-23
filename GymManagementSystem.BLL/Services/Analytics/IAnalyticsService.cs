using GymManagementSystem.BLL.ViewModels.Analytics;

namespace GymManagementSystem.BLL.Services.Analytics;

public interface IAnalyticsService
{
    Task<AnalyticsViewModel> GetDashboardAnalyticsAsync();
}
