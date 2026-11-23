using GymManagementSystem.BLL.ViewModels.Analytics;
using GymManagementSystem.DAL.Repositories.UnitOfWork;

namespace GymManagementSystem.BLL.Services.Analytics;

public class AnalyticsService : IAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;

    public AnalyticsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AnalyticsViewModel> GetDashboardAnalyticsAsync()
    {
        var now = DateTime.Now;

        var totalMembers = await _unitOfWork.Members.GetTotalCountAsync();
        var activeMembers = await _unitOfWork.Members.GetActiveMembersCountAsync(now);
        var totalTrainers = await _unitOfWork.Trainers.GetTotalCountAsync();
        var upcomingSessions = await _unitOfWork.Sessions.GetUpcomingCountAsync(now);
        var ongoingSessions = await _unitOfWork.Sessions.GetOngoingCountAsync(now);
        var completedSessions = await _unitOfWork.Sessions.GetCompletedCountAsync(now);

        return new AnalyticsViewModel
        {
            TotalMembers = totalMembers,
            ActiveMembers = activeMembers,
            TotalTrainers = totalTrainers,
            UpcomingSessions = upcomingSessions,
            OngoingSessions = ongoingSessions,
            CompletedSessions = completedSessions
        };
    }
}
