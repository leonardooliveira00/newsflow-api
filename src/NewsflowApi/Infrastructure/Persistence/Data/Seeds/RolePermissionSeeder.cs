using Microsoft.EntityFrameworkCore;
using NewsflowApi.Domain.Constants.Authorization;
using NewsflowApi.Domain.Entities.Authorization;
using NewsflowApi.Infrastructure.Persistence;

namespace NewsflowApi.Infrastructure.Persistence.Data.Seeds
{
    public class RolePermissionSeeder(NewsflowDbContext context)
    {
        private readonly NewsflowDbContext _context = context;

        public async Task SeedAsync()
        {
            var roles = await _context.Roles.ToDictionaryAsync(role => role.Name, role => role.Id);

            var permissions = await _context.Permissions.ToDictionaryAsync(permission => permission.Name, permission => permission.Id);

            var matrix = new Dictionary<string, string[]>
            {
                ["ADMIN"] = [.. permissions.Keys],

                ["MANAGER"] = [
                    PermissionConstants.ViewAnalytics
                    ],

                ["EDITOR_IN_CHIEF"] = [
                    PermissionConstants.CreateStaff,
                    PermissionConstants.CreateArticle,
                    PermissionConstants.EditOwnArticle,
                    PermissionConstants.EditAnyArticle,
                    PermissionConstants.SubmitArticle,
                    PermissionConstants.ReviewArticle,
                    PermissionConstants.RequestArticleChanges,
                    PermissionConstants.ApproveArticle,
                    PermissionConstants.PublishArticle,
                    PermissionConstants.UploadMedia,
                    PermissionConstants.ViewAnalytics
                    ],

                ["EDITOR"] = [
                    PermissionConstants.EditAnyArticle,
                    PermissionConstants.ReviewArticle,
                    PermissionConstants.RequestArticleChanges,
                    PermissionConstants.ApproveArticle,
                    ],

                ["REPORTER"] = [
                    PermissionConstants.CreateArticle,
                    PermissionConstants.EditOwnArticle,
                    PermissionConstants.SubmitArticle,
                    ],

                ["PHOTOGRAPHER"] = [
                    PermissionConstants.UploadMedia,
                    ]
            };

            var existingRelations = await _context.RolePermissions.Select(rolePermission => new
            {
                rolePermission.RoleId,
                rolePermission.PermissionId
            }).ToListAsync();

            var existingPairs = existingRelations.Select(relation => (
                relation.RoleId,
                relation.PermissionId
            )).ToHashSet();

            var missingRelations = new List<RolePermission>();

            foreach (var (roleName, permissionNames) in matrix)
            {
                var roleId = roles[roleName];

                foreach (var permissionName in permissionNames)
                {
                    var permissionId = permissions[permissionName];

                    if (existingPairs.Contains((roleId, permissionId))) continue;

                    missingRelations.Add(new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId,
                    });
                }
            }

            if (missingRelations.Count == 0) return;

            _context.RolePermissions.AddRange(missingRelations);

            await _context.SaveChangesAsync();
        }
    }
}
