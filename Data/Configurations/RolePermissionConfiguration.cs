using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsflowApi.Domain.RolePermissions;

using NewsflowApi.Domain.Roles;
using NewsflowApi.Domain.Permissions;

namespace NewsflowApi.Data.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("role_permissions");

            builder.HasKey(rolePermission => new
            {
                rolePermission.RoleId,
                rolePermission.PermissionId
            });

            builder.Property(rolePermission => rolePermission.RoleId).HasColumnName("role_id");

            builder.Property(rolePermission => rolePermission.PermissionId).HasColumnName("permission_id");

            builder.HasOne(rolePermission => rolePermission.Role).WithMany().HasForeignKey(rolePermission => rolePermission.RoleId);

            builder.HasOne(rolePermission => rolePermission.Permission).WithMany().HasForeignKey(rolePermission => rolePermission.PermissionId);
        }
    }
}
