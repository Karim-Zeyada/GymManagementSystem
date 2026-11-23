using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Data.Configurations;

public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint("CK_Membership_DateRange", "[EndDate] > [StartDate]");
        });

        builder.HasQueryFilter(m => !m.IsDeleted);

        //builder.HasIndex(x => new {
        //    x.MemberId,
        //    x.PlanId,
        //    x.StartDate
        //}).IsUnique();
    }
}
 