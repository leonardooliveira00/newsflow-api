using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsflowApi.Domain.Identity.Staffs;

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

            builder.Property(staff => staff.CreatedAt);

            builder.Property(staff => staff.UpdatedAt);
        }
    }
}
