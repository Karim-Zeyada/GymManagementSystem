using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasDiscriminator<string>("UserType")
            .HasValue<Member>("Member")
            .HasValue<Trainer>("Trainer");
        
        builder.Property(u => u.Name)
            .HasMaxLength(100);
        builder.Property(u => u.Email)
            .HasMaxLength(100);
        builder.Property(u => u.Phone)
            .HasMaxLength(20);

        builder.OwnsOne(u => u.Address, address =>
        {
            address.Property(a => a.Street)
                .HasMaxLength(100)
                .HasColumnName("Street");
            address.Property(a => a.City)
                .HasMaxLength(50)
                .HasColumnName("City");
            address.Property(a => a.BuildingNumber)
                .HasColumnName("BuildingNumber");
        });

        builder.HasIndex(u => u.Email)
            .IsUnique();
        builder.HasIndex(u => u.Phone)
            .IsUnique();

        // phone format validation
        builder.ToTable("Users", t =>
        {
            t.HasCheckConstraint(
                "User_Phone_Ck",
                "LEN([Phone]) = 11 AND [Phone] LIKE '01[0125]%'"
             ); 
        });

        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
