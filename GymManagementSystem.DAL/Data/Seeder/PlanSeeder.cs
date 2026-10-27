using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Data.Seeder
{
    public static class PlanSeeder
    {
        public static async Task SeedAsync()
        {
            using var dbContext = new GymDbContext();
            bool hasAnyPlans = await dbContext.Plans.AnyAsync();

            if (hasAnyPlans)
            {
                return;
            }

            List<Plan> plans = new List<Plan>
            {
                new Plan {
                    Name = "Basic",
                    Description = "Access to gym equipment and facilities.",
                    Price = 300,
                    DurationDays = 30, // Added duration
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow },

                new Plan {
                    Name = "Standard",
                    Description = "Includes Basic plan benefits plus group classes.",
                    Price = 500,
                    DurationDays = 30, // Added duration
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow },

                new Plan {
                    Name = "Premium",
                    Description = "Includes Standard plan benefits plus personal training sessions.",
                    Price = 900,
                    DurationDays = 30, // Added duration
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow }
            };

            await dbContext.Plans.AddRangeAsync(plans);
            await dbContext.SaveChangesAsync();
        }
    }
}