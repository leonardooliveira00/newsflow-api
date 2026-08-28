using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsflowApi.Domain.Staffs;
using NewsflowApi.Domain.Users;

namespace NewsflowApi.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(user => user.Id);

            builder.Property(user => user.StaffId).HasColumnName("staff_id");

            builder.Property(user => user.Email).HasMaxLength(255).IsRequired();
            builder.HasIndex(user => user.Email).IsUnique();

            builder.Property(user => user.PasswordHash).HasColumnName("password_hash");

            builder.Property(user => user.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();

            builder.Property(user => user.CreatedAt).HasColumnName("created_at");

            builder.Property(user => user.UpdatedAt).HasColumnName("updated_at");

            builder.HasOne(user => user.Staff).WithOne(staff => staff.User).HasForeignKey<User>(user => user.StaffId);
        }
    }
}
