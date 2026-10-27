using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Data.Contexts
{
    public class GymDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=GymManagementSystem;Trusted_Connection=True; trustServerCertificate=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymDbContext).Assembly);
        }

        public DbSet<Plan> Plans { get; set; }
    }
}
