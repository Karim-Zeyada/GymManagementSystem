using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.DAL.Data.Seeder
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAllAsync(
            GymDbContext dbContext,
            RoleManager<IdentityRole>? roleManager = null,
            UserManager<ApplicationUser>? userManager = null)
        {
            await PlanSeeder.SeedAsync(dbContext);
            await CatgorySeeder.SeedAsync(dbContext);
            await GymDemoDataSeeder.SeedDemoDataAsync(dbContext);

            if (roleManager != null && userManager != null)
            {
                await IdentitySeeder.SeedRolesAndAdminAsync(roleManager, userManager);
            }
        }
    }
}
