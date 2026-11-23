using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GymManagementSystem.DAL.Data.Seeder;

public static class PlanSeeder
{
    public static async Task SeedAsync(GymDbContext dbContext)
    {
        // 1. Idempotency check: only seed if the table is empty
        if (await dbContext.Plans.AnyAsync())
        {
            return;
        }

        // 2. Locate plans.json
        var candidatePaths = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Data", "Seeder", "plans.json"),
            Path.Combine(AppContext.BaseDirectory, "plans.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "GymManagementSystem.DAL", "Data", "Seeder", "plans.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seeder", "plans.json")
        };

        string? filePath = candidatePaths.FirstOrDefault(File.Exists);

        if (filePath == null)
        {
            return;
        }

        // 3. Read & Deserialize
        var json = await File.ReadAllTextAsync(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var plans = JsonSerializer.Deserialize<List<Plan>>(json, options);

        if (plans != null && plans.Any())
        {
            var now = DateTime.UtcNow;
            foreach (var plan in plans)
            {
                plan.CreatedAt = now;
            }

            // 4. Insert & Save
            await dbContext.Plans.AddRangeAsync(plans);
            await dbContext.SaveChangesAsync();
        }
    }
}
