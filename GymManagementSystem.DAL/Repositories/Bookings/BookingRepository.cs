using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repositories.Bookings;

public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    public BookingRepository(GymDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Booking>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(b => b.Member)
            .Include(b => b.Session)
                .ThenInclude(s => s.Category)
            .Include(b => b.Session)
                .ThenInclude(s => s.Trainer)
            .OrderByDescending(b => b.Date)
            .ToListAsync();
    }

    public async Task<Booking?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.Member)
            .Include(b => b.Session)
                .ThenInclude(s => s.Category)
            .Include(b => b.Session)
                .ThenInclude(s => s.Trainer)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetBookingsBySessionIdAsync(int sessionId)
    {
        return await _dbSet
            .Include(b => b.Member)
            .Where(b => b.SessionId == sessionId)
            .OrderBy(b => b.Member.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetBookingsByMemberIdAsync(int memberId)
    {
        return await _dbSet
            .Include(b => b.Session)
                .ThenInclude(s => s.Category)
            .Include(b => b.Session)
                .ThenInclude(s => s.Trainer)
            .Where(b => b.MemberId == memberId)
            .OrderByDescending(b => b.Session.StartDate)
            .ToListAsync();
    }

    public async Task<bool> IsMemberBookedInSessionAsync(int memberId, int sessionId)
    {
        return await _dbSet
            .AnyAsync(b => b.MemberId == memberId && b.SessionId == sessionId);
    }

    public async Task<int> GetSessionBookingsCountAsync(int sessionId)
    {
        return await _dbSet.CountAsync(b => b.SessionId == sessionId);
    }
}
