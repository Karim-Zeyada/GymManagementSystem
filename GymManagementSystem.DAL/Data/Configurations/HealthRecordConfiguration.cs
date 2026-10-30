using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Data.Configurations;

public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
{
    public void Configure(EntityTypeBuilder<HealthRecord> builder)
    {
        builder.Property(h => h.Height)
            .HasPrecision(5, 2);

        builder.Property(h => h.Weight)
            .HasPrecision(5, 2);

        builder.Property(h => h.BloodType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(h => h.MedicalConditions)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(h => h.Notes)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint("HealthRecordHeight", "Height > 0");
            tb.HasCheckConstraint("HealthRecordWeight", "Weight > 0");
        });

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
