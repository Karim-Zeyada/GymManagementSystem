using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Data.Seeder;

public static class CatgorySeeder
{
    public static async Task SeedAsync(GymDbContext dbContext)
    {
        if (await dbContext.Categories.AnyAsync())
            return;
       
            var categories = new List<Category>
            {
                new Category { Name = "Strength Training" },
                new Category { Name = "Cardio" },
                new Category { Name = "Flexibility" },
                new Category { Name = "Balance" },
                new Category { Name = "Endurance" }
            };
            dbContext.Categories.AddRange(categories);
            await dbContext.SaveChangesAsync();
    }
}
