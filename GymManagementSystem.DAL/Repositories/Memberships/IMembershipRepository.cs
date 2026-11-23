using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Generic;

namespace GymManagementSystem.DAL.Repositories.Memberships;

public interface IMembershipRepository : IGenericRepository<Membership>
{
    Task<IEnumerable<Membership>> GetAllWithDetailsAsync();
    Task<Membership?> GetWithDetailsAsync(int id);
    Task<Membership?> GetActiveMembershipForMemberAsync(int memberId, DateTime targetDate);
    Task<bool> HasActiveMembershipAsync(int memberId, DateTime targetDate);
    Task<IEnumerable<Membership>> GetMembershipsByMemberIdAsync(int memberId);
}
