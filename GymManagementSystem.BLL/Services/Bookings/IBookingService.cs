using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Bookings;

namespace GymManagementSystem.BLL.Services.Bookings;

public interface IBookingService
{
    Task<SessionScheduleViewModel> GetScheduleAndBookingsAsync();
    Task<CreateBookingViewModel> GetCreateBookingViewModelAsync(int? sessionId = null);
    Task<Result> CreateBookingAsync(CreateBookingViewModel model);
    Task<Result> ToggleAttendanceAsync(int bookingId);
    Task<Result> CancelBookingAsync(int bookingId);
    Task<IEnumerable<BookingViewModel>> GetBookingsForSessionAsync(int sessionId);
}
