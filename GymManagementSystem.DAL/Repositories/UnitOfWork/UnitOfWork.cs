using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Bookings;
using GymManagementSystem.DAL.Repositories.Generic;
using GymManagementSystem.DAL.Repositories.Members;
using GymManagementSystem.DAL.Repositories.Memberships;
using GymManagementSystem.DAL.Repositories.Plans;
using GymManagementSystem.DAL.Repositories.Sessions;
using GymManagementSystem.DAL.Repositories.Trainers;

namespace GymManagementSystem.DAL.Repositories.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly GymDbContext _context;
    private IMemberRepository? _members;
    private ITrainerRepository? _trainers;
    private IPlanRepository? _plans;
    private ISessionRepository? _sessions;
    private IMembershipRepository? _memberships;
    private IBookingRepository? _bookings;
    private IGenericRepository<Category>? _categories;
    private IGenericRepository<HealthRecord>? _healthRecords;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(GymDbContext context)
    {
        _context = context;
    }

    public IMemberRepository Members => _members ??= new MemberRepository(_context);
    public ITrainerRepository Trainers => _trainers ??= new TrainerRepository(_context);
    public IPlanRepository Plans => _plans ??= new PlanRepository(_context);
    public ISessionRepository Sessions => _sessions ??= new SessionRepository(_context);
    public IMembershipRepository Memberships => _memberships ??= new MembershipRepository(_context);
    public IBookingRepository Bookings => _bookings ??= new BookingRepository(_context);
    public IGenericRepository<Category> Categories => _categories ??= new GenericRepository<Category>(_context);
    public IGenericRepository<HealthRecord> HealthRecords => _healthRecords ??= new GenericRepository<HealthRecord>(_context);

    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.TryGetValue(type, out var repository))
        {
            repository = new GenericRepository<T>(_context);
            _repositories[type] = repository;
        }
        return (IGenericRepository<T>)repository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<int> CompleteAsync()
    {
        return await SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
