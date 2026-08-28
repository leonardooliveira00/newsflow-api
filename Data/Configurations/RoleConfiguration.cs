using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsflowApi.Domain.Roles;

namespace NewsflowApi.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");

            builder.HasKey(role => role.RoleId);
            builder.Property(role => role.RoleId).HasColumnName("role_id");

            builder.Property(role => role.RoleName).HasColumnName("name").HasMaxLength(50).IsRequired();
            builder.HasIndex(role => role.RoleName).IsUnique();

            builder.Property(role => role.RoleDescription).HasColumnName("description").HasMaxLength(255);

            builder.Property(role => role.CreatedAt).HasColumnName("created_at");

            builder.Property(role => role.UpdatedAt).HasColumnName("updated_at");
        }
    }
}
