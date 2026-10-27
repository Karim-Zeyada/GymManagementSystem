
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Models;

namespace GymManagementSystem.DAL.Repositories.Plans;

public class PlanRepository : IPlanRepository
{
    public GymDbContext dbContext = new GymDbContext();
    public void Add(Plan plan) => dbContext.Add(plan);

    public void Delete(Plan plan)
        => dbContext.Remove(plan);


    public async Task<IEnumerable<Plan>> GetAllAsync()
        => await dbContext.Plans.ToListAsync();


    public async Task<Plan?> GetByIdAsync(int id)
        => await dbContext.Plans.FirstOrDefaultAsync(p => p.Id == id);

    public void Update(Plan plan) => dbContext.Plans.Update(plan);

    public async Task<int> SaveChangesAsync() => await dbContext.SaveChangesAsync();


}
