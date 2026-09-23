using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsflowApi.Application.Common;
using NewsflowApi.Domain.Entities.Authorization;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Infrastructure.Persistence;

namespace NewsflowApi.Application.Authorization
{
    public class UserRoleManagementService(
        NewsflowDbContext context,
        UserManager<User> userManager)
    {
        private readonly NewsflowDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;

        public async Task<ApplicationResult> AssignRoleAsync(Guid userId, Guid roleId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ApplicationResult.Failure(
                "user_not_found",
                "User not found.",
                ApplicationErrorType.NotFound
                );

            var role = await _context.Roles.FirstOrDefaultAsync(role => role.Id == roleId);

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

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.SaveChangesAsync();

                var updateSecurityStampResult = await _userManager.UpdateSecurityStampAsync(user);

                if (!updateSecurityStampResult.Succeeded)
                {
                    await transaction.RollbackAsync();

                    return ApplicationResult.Failure(
                        "update_security_stamp_failed",
                        "Failed to update the security stamp.",
                        ApplicationErrorType.Internal
                           );
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                return ApplicationResult.Failure(
                    "role_assignment_failed",
                    "Failed to assign the role.",
                    ApplicationErrorType.Internal
                    );
            }

            return ApplicationResult.Success();
        }

        public async Task<ApplicationResult> ResignRoleAsync(Guid userId, Guid roleId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ApplicationResult.Failure(
                "user_not_found",
                "User not found.",
                ApplicationErrorType.NotFound
                );

            var role = await _context.Roles.FirstOrDefaultAsync(role => role.Id == roleId);

            if (role is null) return ApplicationResult.Failure(
                "role_not_found",
                "Role not found.",
                ApplicationErrorType.NotFound
                );

            var userRole = await _context.UserRoles.FirstOrDefaultAsync(userRole => userRole.UserId == user.Id && userRole.RoleId == role.Id);

            if (userRole is null) return ApplicationResult.Failure(
                "role_not_assigned_to_user",
                "Role is not assigned to the user. ",
                ApplicationErrorType.NotFound
                );

            _context.UserRoles.Remove(userRole);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.SaveChangesAsync();

                var updateSecurityStampResult = await _userManager.UpdateSecurityStampAsync(user);

                if (!updateSecurityStampResult.Succeeded)
                {
                    await transaction.RollbackAsync();

                    return ApplicationResult.Failure(
                        "update_security_stamp_failed",
                        "Failed to update the security stamp.",
                        ApplicationErrorType.Internal
                           );
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                return ApplicationResult.Failure(
                        "role_removal_failed",
                        "Failed to remove the role.",
                        ApplicationErrorType.Internal
                        );
            }

            return ApplicationResult.Success();
        }
    }
}
