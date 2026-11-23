using GymManagementSystem.BLL.ViewModels.Sessions;

namespace GymManagementSystem.BLL.ViewModels.Bookings;

public class SessionScheduleViewModel
{
    public IEnumerable<SessionViewModel> UpcomingSessions { get; set; } = new List<SessionViewModel>();
    public IEnumerable<BookingViewModel> RecentBookings { get; set; } = new List<BookingViewModel>();
}
