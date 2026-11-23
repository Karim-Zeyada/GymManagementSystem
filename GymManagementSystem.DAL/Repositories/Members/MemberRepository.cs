using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repositories.Members;

public class MemberRepository : GenericRepository<Member>, IMemberRepository
{
    public MemberRepository(GymDbContext context) : base(context)
    {
    }

    public async Task<Member?> GetWithHealthRecordAsync(int id)
    {
        return await _dbSet
            .Include(m => m.HealthRecord)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Member?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(m => m.HealthRecord)
            .Include(m => m.Memberships)
                .ThenInclude(ms => ms.Plan)
            .Include(m => m.Bookings)
                .ThenInclude(b => b.Session)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<bool> IsEmailExistsAsync(string email, int? excludeMemberId = null)
    {
        return await _context.GymUsers
            .AnyAsync(u => u.Email.ToLower() == email.Trim().ToLower() && (!excludeMemberId.HasValue || u.Id != excludeMemberId.Value));
    }

    public async Task<bool> IsPhoneExistsAsync(string phone, int? excludeMemberId = null)
    {
        return await _context.GymUsers
            .AnyAsync(u => u.Phone == phone.Trim() && (!excludeMemberId.HasValue || u.Id != excludeMemberId.Value));
    }

    public async Task<bool> HasActiveBookingsAsync(int memberId)
    {
        var now = DateTime.UtcNow;
        return await _context.Bookings
            .Include(b => b.Session)
            .AnyAsync(b => b.MemberId == memberId && b.Session != null && b.Session.EndDate >= now);
    }

    public async Task<int> GetActiveMembersCountAsync(DateTime now)
    {
        return await _context.Memberships
            .Where(m => m.EndDate >= now)
            .Select(m => m.MemberId)
            .Distinct()
            .CountAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _dbSet.CountAsync();
    }
}
