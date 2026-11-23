using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Generic;

namespace GymManagementSystem.DAL.Repositories.Trainers;

public interface ITrainerRepository : IGenericRepository<Trainer>
{
    Task<Trainer?> GetWithSessionsAsync(int id);
    Task<bool> IsEmailExistsAsync(string email, int? excludeTrainerId = null);
    Task<bool> IsPhoneExistsAsync(string phone, int? excludeTrainerId = null);
    Task<bool> HasActiveSessionsAsync(int trainerId);
    Task<int> GetTotalCountAsync();
}
