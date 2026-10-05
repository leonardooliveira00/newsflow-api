using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsflowApi.Application.Common.Application;
using NewsflowApi.Application.Common.Errors;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Enums.Identity.Users;
using NewsflowApi.Infrastructure.Persistence;

namespace NewsflowApi.Application.Users
{
    public class UserService(
        UserManager<User> userManager,
        NewsflowDbContext context
        )
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly NewsflowDbContext _context = context;

        public async Task<ApplicationResult<User>> CreateUserForStaffAsync(Guid staffId, string email)
        {
            var staff = await _context.Staffs.Include(staff => staff.User).FirstOrDefaultAsync(staff => staff.Id == staffId);

            if (staff is null) return ApplicationResult<User>.Failure(StaffErrors.NotFound);

            if (staff.User is not null) return ApplicationResult<User>.Failure(UserErrors.UserAlreadyExists);

            var existingEmail = await _userManager.FindByEmailAsync(email);

            if (existingEmail is not null) return ApplicationResult<User>.Failure(UserErrors.EmailAlreadyInUse);

            var user = new User
            {
                Id = Guid.NewGuid(),
                StaffId = staff.Id,
                Email = email,
                UserName = email,
                EmailConfirmed = false,
                Status = UserStatus.Pending
            };

            var identityResult = await _userManager.CreateAsync(user);

            if (!identityResult.Succeeded)
            {
                var errorMessage = string.Join(
                    " ",
                    identityResult.Errors.Select(error => error.Description)
                    );

                var error = new ApplicationError(
                    UserErrors.CreationFailed.Code,
                    errorMessage,
                    UserErrors.CreationFailed.Type
                    );
            }

            return ApplicationResult<User>.Success(user);
        }

        public async Task<ApplicationResult<User>> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ApplicationResult<User>.Failure(UserErrors.NotFound);

            return ApplicationResult<User>.Success(user);
        }

        public async Task<ApplicationResult> SuspendUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ApplicationResult.Failure(UserErrors.NotFound);

            if (user.Status == UserStatus.Suspended) return ApplicationResult.Failure(UserErrors.Suspended);

            user.Status = UserStatus.Suspended;

            var stampResult = await _userManager.UpdateSecurityStampAsync(user);

            if (!stampResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to update the user's security stamp."
                );
            }

            return ApplicationResult.Success();
        }

        public async Task<ApplicationResult> DeactivateUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ApplicationResult.Failure(UserErrors.NotFound);

            if (user.Status == UserStatus.Inactive) return ApplicationResult.Failure(UserErrors.Inactive);

            user.Status = UserStatus.Inactive;

            var stampResult = await _userManager.UpdateSecurityStampAsync(user);

            if (!stampResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to update the user's security stamp."
                );
            }

            return ApplicationResult.Success();
        }
    }
}
