using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repositories.Memberships;

public class MembershipRepository : GenericRepository<Membership>, IMembershipRepository
{
    public MembershipRepository(GymDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Membership>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(m => m.Member)
            .Include(m => m.Plan)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<Membership?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(m => m.Member)
            .Include(m => m.Plan)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Membership?> GetActiveMembershipForMemberAsync(int memberId, DateTime targetDate)
    {
        return await _dbSet
            .Include(m => m.Plan)
            .Where(m => m.MemberId == memberId && m.StartDate <= targetDate && m.EndDate >= targetDate)
            .OrderByDescending(m => m.EndDate)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasActiveMembershipAsync(int memberId, DateTime targetDate)
    {
        return await _dbSet
            .AnyAsync(m => m.MemberId == memberId && m.StartDate <= targetDate && m.EndDate >= targetDate);
    }

    public async Task<IEnumerable<Membership>> GetMembershipsByMemberIdAsync(int memberId)
    {
        return await _dbSet
            .Include(m => m.Plan)
            .Where(m => m.MemberId == memberId)
            .OrderByDescending(m => m.EndDate)
            .ToListAsync();
    }
}
