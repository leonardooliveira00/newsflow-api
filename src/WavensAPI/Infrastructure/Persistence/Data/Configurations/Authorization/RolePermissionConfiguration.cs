using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WavensApi.Domain.Entities.Authorization;

namespace WavensApi.Infrastructure.Persistence.Data.Configurations.Authorization
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

            builder.HasOne(rolePermission => rolePermission.Role).WithMany(role => role.RolePermissions).HasForeignKey(rolePermission => rolePermission.RoleId);

            builder.HasOne(rolePermission => rolePermission.Permission).WithMany(permission => permission.RolePermissions).HasForeignKey(rolePermission => rolePermission.PermissionId);
        }
    }
}
