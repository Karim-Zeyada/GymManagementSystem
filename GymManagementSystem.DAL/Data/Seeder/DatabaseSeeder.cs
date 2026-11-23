using GymManagementSystem.DAL.Data.Contexts;

namespace GymManagementSystem.DAL.Data.Seeder
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAllAsync(GymDbContext dbContext)
        {
            await PlanSeeder.SeedAsync(dbContext);

            await CatgorySeeder.SeedAsync(dbContext);
        }
    }
}
