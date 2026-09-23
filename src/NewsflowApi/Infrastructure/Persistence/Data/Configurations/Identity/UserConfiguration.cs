using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsflowApi.Domain.Entities.Identity.Users;

namespace NewsflowApi.Infrastructure.Persistence.Data.Configurations.Identity
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(user => user.Id);

            builder.Property(user => user.StaffId);

            builder.Property(user => user.Email).HasMaxLength(255).IsRequired();
            builder.Property(user => user.NormalizedEmail).HasMaxLength(255).IsRequired();
            builder.HasIndex(user => user.NormalizedEmail).HasDatabaseName("EmailIndex").IsUnique();

            builder.Property(user => user.PasswordHash);

            builder.Property(user => user.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

            builder.Property(user => user.CreatedAt);

            builder.Property(user => user.UpdatedAt);

            builder.HasOne(user => user.Staff).WithOne(staff => staff.User).HasForeignKey<User>(user => user.StaffId);
        }
    }
}
