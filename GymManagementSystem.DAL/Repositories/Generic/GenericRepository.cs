using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repositories.Generic;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly GymDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(GymDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
