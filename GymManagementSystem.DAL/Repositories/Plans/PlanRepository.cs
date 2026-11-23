using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repositories.Plans;

public class PlanRepository : GenericRepository<Plan>, IPlanRepository
{
    public PlanRepository(GymDbContext context) : base(context)
    {
    }

    public async Task<bool> HasActiveMembershipsAsync(int planId)
    {
        var now = DateTime.UtcNow;
        return await _context.Memberships
            .AnyAsync(m => m.PlanId == planId && m.EndDate >= now);
    }
}
