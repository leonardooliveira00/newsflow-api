using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsflowApi.Domain.Staffs;

namespace NewsflowApi.Data.Configurations
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.ToTable("staffs");

            builder.HasKey(staff => staff.Id);

            builder.Property(staff => staff.FirstName).HasColumnName("first_name").HasMaxLength(50).IsRequired();

            builder.Property(staff => staff.LastName).HasColumnName("last_name").HasMaxLength(50).IsRequired();

            builder.Property(staff => staff.CreatedAt).HasColumnName("created_at");

            builder.Property(staff => staff.UpdatedAt).HasColumnName("updated_at");
        }
    }
}
