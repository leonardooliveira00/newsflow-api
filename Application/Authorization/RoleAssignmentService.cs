using Microsoft.EntityFrameworkCore;
using NewsflowApi.Application.Common;
using NewsflowApi.Data;
using NewsflowApi.Domain.Authorization;

namespace NewsflowApi.Application.Authorization
{
    public class RoleAssignmentService(NewsflowDbContext context)
    {
        private readonly NewsflowDbContext _context = context;

        public async Task<ApplicationResult> AssignRoleAsync(Guid userId, string roleName)
        {
            var userExists = await _context.Users.AnyAsync(user => user.Id == userId);

            if (!userExists) return ApplicationResult.Failure(
                "user_not_found",
                "User not found.",
                ApplicationErrorType.NotFound
                );

            var role = await _context.Roles.FirstOrDefaultAsync(role => role.Name == roleName);

            if (role is null) return ApplicationResult.Failure(
                "role_not_found",
                "Role not found.",
                ApplicationErrorType.NotFound
                );

            var alreadyHasRole = await _context.UserRoles.AnyAsync(userRole =>
                userRole.UserId == userId &&
                userRole.RoleId == role.Id);

            if (alreadyHasRole) return ApplicationResult.Failure(
                "user_already_has_role",
                "User already has this role",
                ApplicationErrorType.Conflict
                );

            var userRole = new UserRole
            {
                RoleId = role.Id,
                UserId = userId,
                AssignedAt = DateTime.UtcNow
            };

            _context.UserRoles.Add(userRole);

            await _context.SaveChangesAsync();

            return ApplicationResult.Success();
        }
    }
}
