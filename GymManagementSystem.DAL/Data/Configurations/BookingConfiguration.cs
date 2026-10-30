using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(x => x.IsAttended)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(b => new {
            b.MemberId,
            b.SessionId
        }).IsUnique();

        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint("CK_Booking_Date", "[Date] >= GETDATE()");
        });

        builder.HasOne(b => b.Member)
            .WithMany(m => m.Bookings)
            .HasForeignKey(b => b.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
