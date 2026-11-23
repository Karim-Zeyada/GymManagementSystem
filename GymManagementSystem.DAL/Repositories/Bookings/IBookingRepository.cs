using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Generic;

namespace GymManagementSystem.DAL.Repositories.Bookings;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<IEnumerable<Booking>> GetAllWithDetailsAsync();
    Task<Booking?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Booking>> GetBookingsBySessionIdAsync(int sessionId);
    Task<IEnumerable<Booking>> GetBookingsByMemberIdAsync(int memberId);
    Task<bool> IsMemberBookedInSessionAsync(int memberId, int sessionId);
    Task<int> GetSessionBookingsCountAsync(int sessionId);
}
