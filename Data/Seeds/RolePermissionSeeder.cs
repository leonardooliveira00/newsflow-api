using Microsoft.EntityFrameworkCore;
using NewsflowApi.Domain.Authorization;

namespace NewsflowApi.Data.Seeds
{
    public class RolePermissionSeeder
    {
        private readonly NewsflowDbContext _context;

        public RolePermissionSeeder(NewsflowDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            var roles = await _context.Roles.ToDictionaryAsync(role => role.Name, role => role.Id);

            var permissions = await _context.Permissions.ToDictionaryAsync(permission => permission.Name, permission => permission.Id);

            var matrix = new Dictionary<string, string[]>
            {
                ["ADMIN"] = [.. permissions.Keys],

                ["MANAGER"] = [
                    "ANALYTICS_VIEW"
                    ],

                ["EDITOR_IN_CHIEF"] = [
                    "STAFF_CREATE",
                    "ARTICLE_CREATE",
                    "ARTICLE_EDIT_OWN",
                    "ARTICLE_EDIT_ANY",
                    "ARTICLE_SUBMIT",
                    "ARTICLE_REVIEW",
                    "ARTICLE_REQUEST_CHANGES",
                    "ARTICLE_APPROVE",
                    "ARTICLE_PUBLISH",
                    "MEDIA_UPLOAD",
                    "ANALYTICS_VIEW"
                    ],

                ["EDITOR"] = [
                    "ARTICLE_EDIT_ANY",
                    "ARTICLE_REVIEW",
                    "ARTICLE_REQUEST_CHANGES",
                    "ARTICLE_APPROVE"
                    ],

                ["REPORTER"] = [
                    "ARTICLE_CREATE",
                    "ARTICLE_EDIT_OWN",
                    "ARTICLE_SUBMIT"
                    ],

                ["PHOTOGRAPHER"] = [
                    "MEDIA_UPLOAD"
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

            foreach (var (roleName, PermissionNames) in matrix)
            {
                var roleId = roles[roleName];

                foreach (var permissionName in PermissionNames)
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
