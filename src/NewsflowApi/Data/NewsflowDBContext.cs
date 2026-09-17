using Microsoft.EntityFrameworkCore;

namespace NewsflowApi.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NewsflowApi.Domain.Common;
using NewsflowApi.Domain.Entities.Authorization;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Entities.Staffs;

public class NewsflowDbContext : IdentityUserContext<User, Guid>
{
    public NewsflowDbContext(DbContextOptions<NewsflowDbContext> options) : base(options)
    {
    }
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Staff> Staffs => Set<Staff>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(NewsflowDbContext).Assembly
            );
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Property(entity => entity.CreatedAt).IsModified = false;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}