using Microsoft.EntityFrameworkCore;
using NewsflowApi.Domain.Constants.Authorization;
using NewsflowApi.Domain.Entities.Authorization;
using NewsflowApi.Infrastructure.Persistence;

namespace NewsflowApi.Infrastructure.Persistence.Data.Seeds
{
    public class PermissionSeeder(NewsflowDbContext context)
    {
        private readonly NewsflowDbContext _context = context;

        public async Task SeedAsync()
        {
            var permissions = new[]
        {

            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.CreateStaff,
                Description = "Create staff members."
            },

            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.ViewStaff,
                Description = "List and view staff."
            },

            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.UpdateStaff,
                Description = "Update staff members data"
            },

            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.DeactivateStaff,
                Description = "Deactivate a staff member"
            },

            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.CreateUser,
                Description = "Create CMS users."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.UpdateUser,
                Description = "Update CMS users."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.SuspendUser,
                Description = "Suspend CMS users."
            },

            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.CreateArticle,
                Description = "Create articles."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.EditOwnArticle,
                Description = "Edit own articles."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.EditAnyArticle,
                Description = "Edit any article."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.SubmitArticle,
                Description = "Submit articles for review."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.ReviewArticle,
                Description = "Review submitted articles."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.RequestArticleChanges,
                Description = "Request changes to articles."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.ApproveArticle,
                Description = "Approve articles."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.PublishArticle,
                Description = "Publish articles."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.UploadMedia,
                Description = "Upload media assets."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.ViewAnalytics,
                Description = "View analytics."
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = PermissionConstants.ManageRole,
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
