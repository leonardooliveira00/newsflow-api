using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WavensApi.Application.Common.Application;
using WavensApi.Application.Common.Errors;
using WavensApi.Domain.Entities.Authorization;
using WavensApi.Domain.Entities.Identity.Users;
using WavensApi.Infrastructure.Persistence;

namespace WavensApi.Application.Authorization
{
    public class UserRoleManagementService(
        WavensDbContext context,
        UserManager<User> userManager)
    {
        private readonly WavensDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;

        public async Task<ApplicationResult> AssignRoleAsync(Guid userId, Guid roleId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ApplicationResult.Failure(UserErrors.NotFound);

            var role = await _context.Roles.FirstOrDefaultAsync(role => role.Id == roleId);

            if (role is null) return ApplicationResult.Failure(AuthorizationErrors.RoleNotFound);

            var alreadyHasRole = await _context.UserRoles.AnyAsync(userRole =>
                userRole.UserId == userId &&
                userRole.RoleId == role.Id);

            if (alreadyHasRole) return ApplicationResult.Failure(AuthorizationErrors.RoleAlreadyAssignedToUser);

            var userRole = new UserRole
            {
                RoleId = role.Id,
                UserId = userId,
                AssignedAt = DateTime.UtcNow
            };

            _context.UserRoles.Add(userRole);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.SaveChangesAsync();

            var updateSecurityStampResult = await _userManager.UpdateSecurityStampAsync(user);

            if (!updateSecurityStampResult.Succeeded)
            {
                throw new InvalidOperationException();
            }

            await transaction.CommitAsync();

            return ApplicationResult.Success();
        }

        public async Task<ApplicationResult> ResignRoleAsync(Guid userId, Guid roleId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ApplicationResult.Failure(UserErrors.NotFound);

            var role = await _context.Roles.FirstOrDefaultAsync(role => role.Id == roleId);

            if (role is null) return ApplicationResult.Failure(AuthorizationErrors.RoleNotFound);

            var userRole = await _context.UserRoles.FirstOrDefaultAsync(userRole => userRole.UserId == user.Id && userRole.RoleId == role.Id);

            if (userRole is null) return ApplicationResult.Failure(AuthorizationErrors.RoleNotAssignedToUser);

            _context.UserRoles.Remove(userRole);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.SaveChangesAsync();

            var updateSecurityStampResult = await _userManager.UpdateSecurityStampAsync(user);

            if (!updateSecurityStampResult.Succeeded)
            {
                throw new InvalidOperationException();
            }

            await transaction.CommitAsync();

            return ApplicationResult.Success();
        }
    }
}
