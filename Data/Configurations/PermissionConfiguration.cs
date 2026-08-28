using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NewsflowApi.Domain.Permissions;

namespace NewsflowApi.Data.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {

        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("permissions");

            builder.HasKey(permission => permission.PermissionId);
            builder.Property(permission => permission.PermissionId).HasColumnName("permission_id");

            builder.Property(permission => permission.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
            builder.HasIndex(permission => permission.Name).IsUnique();

            builder.Property(permission => permission.Description).HasColumnName("description").HasMaxLength(255);

            builder.Property(permission => permission.CreatedAt).HasColumnName("created_at");

            builder.Property(permission => permission.UpdatedAt).HasColumnName("updated_at");
        }
    }
}
