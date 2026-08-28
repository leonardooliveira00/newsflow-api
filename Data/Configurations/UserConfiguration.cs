using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NewsflowApi.Domain.Users;

namespace NewsflowApi.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(user => user.UserId);
            builder.Property(user => user.UserId).HasColumnName("user_id");

            builder.Property(user => user.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            builder.HasIndex(user => user.Email).IsUnique();

            builder.Property(user => user.PasswordHash).HasColumnName("password_hash");

            builder.Property(user => user.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();

            builder.Property(user => user.CreatedAt).HasColumnName("created_at");

            builder.Property(user => user.UpdatedAt).HasColumnName("updated_at");
        }
    }
}
