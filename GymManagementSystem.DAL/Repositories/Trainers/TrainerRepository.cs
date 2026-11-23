using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repositories.Trainers;

public class TrainerRepository : GenericRepository<Trainer>, ITrainerRepository
{
    public TrainerRepository(GymDbContext context) : base(context)
    {
    }

    public async Task<Trainer?> GetWithSessionsAsync(int id)
    {
        return await _dbSet
            .Include(t => t.Sessions)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<bool> IsEmailExistsAsync(string email, int? excludeTrainerId = null)
    {
        return await _context.GymUsers
            .AnyAsync(u => u.Email.ToLower() == email.Trim().ToLower() && (!excludeTrainerId.HasValue || u.Id != excludeTrainerId.Value));
    }

    public async Task<bool> IsPhoneExistsAsync(string phone, int? excludeTrainerId = null)
    {
        return await _context.GymUsers
            .AnyAsync(u => u.Phone == phone.Trim() && (!excludeTrainerId.HasValue || u.Id != excludeTrainerId.Value));
    }

    public async Task<bool> HasActiveSessionsAsync(int trainerId)
    {
        var now = DateTime.UtcNow;
        return await _context.Sessions
            .AnyAsync(s => s.TrainerId == trainerId && s.EndDate >= now);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _dbSet.CountAsync();
    }
}
