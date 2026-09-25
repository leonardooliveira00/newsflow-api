using Microsoft.EntityFrameworkCore;
using NewsflowApi.Application.Common.Application;
using NewsflowApi.Application.Common.Errors;
using NewsflowApi.Infrastructure.Persistence;

namespace NewsflowApi.Application.Authorization
{
    public class AuthorizationService(NewsflowDbContext dbContext)
    {
        private readonly NewsflowDbContext _context = dbContext;

        public async Task<ApplicationResult<UserAuthorizationContext>> GetUserAuthorizationAsync(Guid userId)
        {
            var userExists = await _context.Users.AnyAsync(user => user.Id == userId);

            if (!userExists) return ApplicationResult<UserAuthorizationContext>.Failure(UserErrors.NotFound);

            var userRoles = await _context.UserRoles
                .AsNoTracking()
                .Where(userRole
                => userRole.UserId == userId)
                .Include(userRole => userRole.Role)
                .ThenInclude(role => role.RolePermissions)
                .ThenInclude(rolePermission => rolePermission.Permission)
                .ToListAsync();

            var roles = userRoles
                .Select(userRole
                => userRole.Role.Name)
                .Distinct()
                .ToList();

            var permissions = userRoles
                .SelectMany(userRole
                => userRole.Role.RolePermissions)
                .Select(rolePermission => rolePermission.Permission.Name)
                .Distinct()
                .ToList();

            var authorizationContext = new UserAuthorizationContext
            {
                Roles = roles,
                Permissions = permissions
            };

            return ApplicationResult<UserAuthorizationContext>.Success(authorizationContext);
        }
    }
}
