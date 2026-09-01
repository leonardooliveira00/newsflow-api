using Microsoft.EntityFrameworkCore;
using NewsflowApi.Application.Common;
using NewsflowApi.Data;

namespace NewsflowApi.Application.Authorization
{
    public class AuthorizationService(NewsflowDbContext dbContext)
    {
        private readonly NewsflowDbContext _context = dbContext;

        public async Task<ApplicationResult<List<string>>> GetUserRolesAsync(Guid userId)
        {
            var userExists = await _context.Users.AnyAsync(user => user.Id == userId);

            if (!userExists) return ApplicationResult<List<string>>.Failure(
                "user_not_found",
                "User not found",
                ApplicationErrorType.NotFound
                );

            var roles = await _context.UserRoles.Where(userRole
                => userRole.UserId == userId)
                .Select(userRole => userRole.Role.Name)
                .ToListAsync();

            return ApplicationResult<List<string>>.Success(roles);
        }

        public async Task<ApplicationResult<List<String>>> GetUserPermissionsAsync(Guid userId)
        {
            var userExists = await _context.Users.AnyAsync(user => user.Id == userId);

            if (!userExists) return ApplicationResult<List<string>>.Failure(
                "user_not_found",
                "User not found",
                ApplicationErrorType.NotFound
                );

            var permissions = await _context.UserRoles.Where(userRole
                => userRole.UserId == userId)
                .SelectMany(userRole
                => userRole.Role.RolePermissions)
                .Select(rolePermission => rolePermission.Permission.Name)
                .Distinct().ToListAsync();

            return ApplicationResult<List<string>>.Success(permissions);
        }

        public async Task<ApplicationResult<UserAuthorizationContext>> SetUserAuthorizationAsync(Guid userId)
        {
            var userExists = await _context.Users.AnyAsync(user => user.Id == userId);

            if (!userExists) return ApplicationResult<UserAuthorizationContext>.Failure(
                "user_not_found",
                "User not found",
                ApplicationErrorType.NotFound
                );

            var userRoles = await _context.UserRoles.Where(userRole
                => userRole.UserId == userId)
                .Include(userRole => userRole.Role)
                .ThenInclude(role => role.RolePermissions)
                .ThenInclude(rolePermission => rolePermission.Permission)
                .AsNoTracking().ToListAsync();

            var roles = userRoles.Select(userRole
                => userRole.Role.Name)
                .Distinct().ToList();

            var permissions = userRoles.SelectMany(userRole
                => userRole.Role.RolePermissions)
                .Select(rolePermission => rolePermission.Permission.Name)
                .Distinct().ToList();

            var authorizationContext = new UserAuthorizationContext
            {
                Roles = roles,
                Permissions = permissions
            };

            return ApplicationResult<UserAuthorizationContext>.Success(authorizationContext);
        }
    }
}
