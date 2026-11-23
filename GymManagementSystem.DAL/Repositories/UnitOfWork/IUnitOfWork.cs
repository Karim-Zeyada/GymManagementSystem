using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Generic;
using GymManagementSystem.DAL.Repositories.Members;
using GymManagementSystem.DAL.Repositories.Plans;
using GymManagementSystem.DAL.Repositories.Sessions;
using GymManagementSystem.DAL.Repositories.Trainers;

namespace GymManagementSystem.DAL.Repositories.UnitOfWork;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IMemberRepository Members { get; }
    ITrainerRepository Trainers { get; }
    IPlanRepository Plans { get; }
    ISessionRepository Sessions { get; }
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<HealthRecord> HealthRecords { get; }
    IGenericRepository<Booking> Bookings { get; }
    IGenericRepository<Membership> Memberships { get; }

    Task<int> SaveChangesAsync();
    Task<int> CompleteAsync();
    IGenericRepository<T> Repository<T>() where T : BaseEntity;
}
