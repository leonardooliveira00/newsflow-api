using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsflowApi.Domain.Authorization;

namespace NewsflowApi.Data.Configurations.Authorization
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(role => role.Id);

            builder.Property(role => role.RoleName).HasMaxLength(50).IsRequired();
            builder.HasIndex(role => role.RoleName).IsUnique();

            builder.Property(role => role.RoleDescription).HasMaxLength(255);

            builder.Property(role => role.CreatedAt);

            builder.Property(role => role.UpdatedAt);
        }
    }
}
