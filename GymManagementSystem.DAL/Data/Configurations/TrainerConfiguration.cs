using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Data.Configurations;

public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
{
    public void Configure(EntityTypeBuilder<Trainer> builder)
    {
        builder.Property(p => p.Speciality)
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}
