using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsflowApi.Domain.Entities.Authorization;

namespace NewsflowApi.Infrastructure.Persistence.Data.Configurations.Authorization
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {

        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(permission => permission.Id);

            builder.Property(permission => permission.Name).HasMaxLength(50).IsRequired();
            builder.HasIndex(permission => permission.Name).IsUnique();

            builder.Property(permission => permission.Description).HasMaxLength(255);

            builder.Property(permission => permission.CreatedAt);

            builder.Property(permission => permission.UpdatedAt);
        }
    }
}
