using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsflowApi.Domain.UserRoles;

namespace NewsflowApi.Data.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("user_roles");

            builder.HasKey(userRole => new
            {
                userRole.UserId,
                userRole.RoleId
            });

            builder.Property(userRole => userRole.UserId).HasColumnName("user_id");

            builder.Property(userRole => userRole.RoleId).HasColumnName("role_id");

            builder.Property(userRole => userRole.AssignedAt).HasColumnName("assigned_at");

            builder.HasOne(userRole => userRole.User).WithMany().HasForeignKey(userRole => userRole.UserId);

            builder.HasOne(userRole => userRole.Role).WithMany().HasForeignKey(userRole => userRole.RoleId);
        }
    }
}
