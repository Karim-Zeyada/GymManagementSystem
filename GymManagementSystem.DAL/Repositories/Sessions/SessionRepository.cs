using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repositories.Sessions;

public class SessionRepository : GenericRepository<Session>, ISessionRepository
{
    public SessionRepository(GymDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Session>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(s => s.Category)
            .Include(s => s.Trainer)
            .Include(s => s.Bookings)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync();
    }

    public async Task<Session?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Category)
            .Include(s => s.Trainer)
            .Include(s => s.Bookings)
                .ThenInclude(b => b.Member)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> HasTrainerConflictAsync(int trainerId, DateTime startDate, DateTime endDate, int? excludeSessionId = null)
    {
        return await _dbSet
            .AnyAsync(s => s.TrainerId == trainerId 
                && (!excludeSessionId.HasValue || s.Id != excludeSessionId.Value)
                && s.StartDate < endDate 
                && s.EndDate > startDate);
    }

    public async Task<int> GetUpcomingCountAsync(DateTime now)
    {
        return await _dbSet.CountAsync(s => s.StartDate > now);
    }

    public async Task<int> GetOngoingCountAsync(DateTime now)
    {
        return await _dbSet.CountAsync(s => s.StartDate <= now && s.EndDate >= now);
    }

    public async Task<int> GetCompletedCountAsync(DateTime now)
    {
        return await _dbSet.CountAsync(s => s.EndDate < now);
    }
}
