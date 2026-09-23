using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsflowApi.Domain.Entities.Authorization;

namespace NewsflowApi.Infrastructure.Persistence.Data.Configurations.Authorization
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            builder.HasKey(userRole => new
            {
                userRole.UserId,
                userRole.RoleId
            });

            builder.Property(userRole => userRole.UserId);

            builder.Property(userRole => userRole.RoleId);

            builder.Property(userRole => userRole.AssignedAt);

            builder.HasOne(userRole => userRole.User).WithMany().HasForeignKey(userRole => userRole.UserId);

            builder.HasOne(userRole => userRole.Role).WithMany().HasForeignKey(userRole => userRole.RoleId);
        }
    }
}
