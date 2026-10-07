using Microsoft.EntityFrameworkCore;
using WavensApi.Application.Common.Application;
using WavensApi.Application.Common.Errors;
using WavensApi.Infrastructure.Persistence;

namespace WavensApi.Application.Authorization
{
    public class AuthorizationService(WavensDbContext dbContext)
    {
        private readonly WavensDbContext _context = dbContext;

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
