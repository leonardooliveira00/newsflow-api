using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsflowApi.Application.Common;
using NewsflowApi.Data;
using NewsflowApi.Domain.Identity.Users;

namespace NewsflowApi.Application.Authentication
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly NewsflowDbContext _context;


        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            NewsflowDbContext context
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<ApplicationResult> CreateUserForStaffAsync(Guid staffId, string email)
        {
            var staff = await _context.Staffs.Include(staff => staff.User).FirstOrDefaultAsync(staff => staff.Id == staffId);

            if (staff is null)
            {
                return ApplicationResult.Failure(
                    "staff_not_found",
                    "Staff not found."
                    );
            }

            if (staff.User is not null)
            {
                return ApplicationResult.Failure(
                    "staff_already_has_user",
                    "Staff already has a user."
                    );
            }

            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser is not null)
            {
                return ApplicationResult.Failure(
                    "email_already_in_use",
                    "Email is already associated with another user."
                    );
            }

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

                return ApplicationResult.Failure(
                    "user_creation_failed",
                    errorMessage
                    );
            }

            return ApplicationResult.Success();
        }

        public async Task<ApplicationResult> SignInAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null) return ApplicationResult.Failure(
                "invalid_credentials",
                "Invalid email or password."
                );

            if (user.Status != UserStatus.Active) return ApplicationResult.Failure(
                "user_not_active",
                "User account is not active."
                );

            var signInResult = await _signInManager.PasswordSignInAsync(
                user,
                password,
                isPersistent: false,
                lockoutOnFailure: true
                );

            if (signInResult.IsLockedOut)
            {
                return ApplicationResult.Failure(
                    "user_locked_out",
                    "User account is temporarily locked."
                );
            }

            if (signInResult.IsNotAllowed)
            {
                return ApplicationResult.Failure(
                    "signin_not_allowed",
                    "Sign in is not allowed for this account."
                );
            }

            if (signInResult.RequiresTwoFactor)
            {
                return ApplicationResult.Failure(
                    "two_factor_required",
                    "Two-factor authentication is required."
                );
            }

            if (!signInResult.Succeeded)
            {
                return ApplicationResult.Failure(
                    "invalid_credentials",
                    "Invalid email or password."
                );
            }

            return ApplicationResult.Success();
        }

        public async Task<ApplicationResult> SignOutAsync()
        {
            await _signInManager.SignOutAsync();

            return ApplicationResult.Success();
        }
    }
}
