using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Data.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint("CK_SessionCapacity", "[Capacity] between 1 AND 25");
            tb.HasCheckConstraint("CK_Session_DateRange", "[EndDate] > [StartDate]");

        });

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
