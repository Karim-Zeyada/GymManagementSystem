using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Generic;

namespace GymManagementSystem.DAL.Repositories.Sessions;

public interface ISessionRepository : IGenericRepository<Session>
{
    Task<IEnumerable<Session>> GetAllWithDetailsAsync();
    Task<Session?> GetWithDetailsAsync(int id);
    Task<bool> HasTrainerConflictAsync(int trainerId, DateTime startDate, DateTime endDate, int? excludeSessionId = null);
    Task<int> GetUpcomingCountAsync(DateTime now);
    Task<int> GetOngoingCountAsync(DateTime now);
    Task<int> GetCompletedCountAsync(DateTime now);
}
