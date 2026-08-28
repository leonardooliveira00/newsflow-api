using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NewsflowApi.Domain.Authorization;

namespace NewsflowApi.Data.Configurations.Authorization
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermissions");

            builder.HasKey(rolePermission => new
            {
                rolePermission.RoleId,
                rolePermission.PermissionId
            });

            builder.Property(rolePermission => rolePermission.RoleId);

            builder.Property(rolePermission => rolePermission.PermissionId);

            builder.HasOne(rolePermission => rolePermission.Role).WithMany().HasForeignKey(rolePermission => rolePermission.RoleId);

            builder.HasOne(rolePermission => rolePermission.Permission).WithMany().HasForeignKey(rolePermission => rolePermission.PermissionId);
        }
    }
}
