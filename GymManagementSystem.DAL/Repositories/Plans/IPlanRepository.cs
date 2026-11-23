using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Generic;

namespace GymManagementSystem.DAL.Repositories.Plans;

public interface IPlanRepository : IGenericRepository<Plan>
{
    Task<bool> HasActiveMembershipsAsync(int planId);
}
