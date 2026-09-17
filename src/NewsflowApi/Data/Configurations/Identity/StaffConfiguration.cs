using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsflowApi.Domain.Entities.Staffs;

namespace NewsflowApi.Data.Configurations.Identity
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.ToTable("Staffs");

            builder.HasKey(staff => staff.Id);

            builder.Property(staff => staff.FirstName).HasMaxLength(50).IsRequired();

            builder.Property(staff => staff.LastName).HasMaxLength(50).IsRequired();

            builder.Property(staff => staff.Email).HasMaxLength(255).IsRequired();
            builder.HasIndex(staff => staff.Email).IsUnique();

            builder.Property(staff => staff.ContactPhone).HasMaxLength(20).IsRequired();

            builder.Property(staff => staff.Bio).HasMaxLength(1000);
        }
    }
}
