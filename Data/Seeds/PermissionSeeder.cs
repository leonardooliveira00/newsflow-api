using Microsoft.EntityFrameworkCore;
using NewsflowApi.Domain.Authorization;

namespace NewsflowApi.Data.Seeds
{
    public class PermissionSeeder
    {
        private readonly NewsflowDbContext _context;

        public PermissionSeeder(NewsflowDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            var permissions = new[]
        {
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "USER_CREATE",
                Description = "Create CMS users."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "USER_UPDATE",
                Description = "Update CMS users."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "USER_SUSPEND",
                Description = "Suspend CMS users."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "ARTICLE_CREATE",
                Description = "Create articles."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "ARTICLE_EDIT_OWN",
                Description = "Edit own articles."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "ARTICLE_EDIT_ANY",
                Description = "Edit any article."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "ARTICLE_SUBMIT",
                Description = "Submit articles for review."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "ARTICLE_REVIEW",
                Description = "Review submitted articles."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "ARTICLE_REQUEST_CHANGES",
                Description = "Request changes to articles."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "ARTICLE_APPROVE",
                Description = "Approve articles."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "ARTICLE_PUBLISH",
                Description = "Publish articles."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "MEDIA_UPLOAD",
                Description = "Upload media assets."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "ANALYTICS_VIEW",
                Description = "View analytics."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "ROLE_MANAGE",
                Description = "Manage roles and permissions."
            }
        };

            var existingPermissions = await _context.Permissions.Select(permission => permission.Name).ToListAsync();

            var missingPermissions = permissions.Where(permission => !existingPermissions.Contains(permission.Name)).ToList();

            if (missingPermissions.Count == 0) return;

            _context.Permissions.AddRange(missingPermissions);

            await _context.SaveChangesAsync();
        }
    }
}
