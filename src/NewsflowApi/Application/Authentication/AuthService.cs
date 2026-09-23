using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewsflowApi.Application.Authorization;
using NewsflowApi.Application.Common;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Enums.Identity.Users;
using NewsflowApi.Infrastructure.Email;
using NewsflowApi.Infrastructure.Persistence;
using NewsflowApi.Infrastructure.Settings;
using System.Security.Claims;

namespace NewsflowApi.Application.Authentication
{
    public class AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        NewsflowDbContext context,
        AuthorizationService authorizationService,
        IEmailService emailService,
        IOptions<FrontendSettings> frontendOptions
            )
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly NewsflowDbContext _context = context;
        private readonly AuthorizationService _authorizationService = authorizationService;
        private readonly IEmailService _emailService = emailService;
        private readonly FrontendSettings _frontendOptions = frontendOptions.Value;

        public async Task<ApplicationResult> CreateUserForStaffAsync(Guid staffId, string email)
        {
            var staff = await _context.Staffs.Include(staff => staff.User).FirstOrDefaultAsync(staff => staff.Id == staffId);

            if (staff is null)
            {
                return ApplicationResult.Failure(
                    "staff_not_found",
                    "Staff not found.",
                    ApplicationErrorType.NotFound
                    );
            }

            if (staff.User is not null)
            {
                return ApplicationResult.Failure(
                    "staff_already_has_user",
                    "Staff already has a user.",
                    ApplicationErrorType.Conflict
                    );
            }

            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser is not null)
            {
                return ApplicationResult.Failure(
                    "email_already_in_use",
                    "Email is already associated with another user.",
                    ApplicationErrorType.Conflict
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
                    errorMessage,
                    ApplicationErrorType.Validation
                    );
            }

            return ApplicationResult.Success();
        }

        public async Task<ApplicationResult> SignInAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null) return ApplicationResult.Failure(
                "invalid_credentials",
                "Invalid email or password.",
                ApplicationErrorType.Unauthorized
                );

            if (user.Status != UserStatus.Active) return ApplicationResult.Failure(
                "user_not_active",
                "User account is not active.",
                ApplicationErrorType.Forbidden
                );

            var signInResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: true
                );

            if (signInResult.IsLockedOut)
            {
                return ApplicationResult.Failure(
                    "user_locked_out",
                    "User account is temporarily locked.",
                    ApplicationErrorType.Locked
                );
            }

            if (signInResult.IsNotAllowed)
            {
                return ApplicationResult.Failure(
                    "signin_not_allowed",
                    "Sign in is not allowed for this account.",
                    ApplicationErrorType.Forbidden
                );
            }

            if (signInResult.RequiresTwoFactor)
            {
                return ApplicationResult.Failure(
                    "two_factor_required",
                    "Two-factor authentication is required.",
                    ApplicationErrorType.Unauthorized
                );
            }

            if (!signInResult.Succeeded)
            {
                return ApplicationResult.Failure(
                    "invalid_credentials",
                    "Invalid email or password.",
                    ApplicationErrorType.Unauthorized
                );
            }

            await _signInManager.SignInAsync(
                user,
                isPersistent: false
                );

            return ApplicationResult.Success();
        }

        public async Task<ApplicationResult> SignOutAsync()
        {
            await _signInManager.SignOutAsync();

            return ApplicationResult.Success();
        }

        public async Task<ApplicationResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null) return ApplicationResult.Failure(
                "user_not_found",
                "User not found",
                ApplicationErrorType.NotFound
                );

            if (user.Status != UserStatus.Active) return ApplicationResult.Failure(
                "user_not_active",
                "User is not active",
                ApplicationErrorType.Conflict
                );

            var verifyPassword = await _userManager.CheckPasswordAsync(user, currentPassword);

            if (!verifyPassword) return ApplicationResult.Failure(
                "invalid_password",
                "Invalid password",
                ApplicationErrorType.Validation
                );

            if (newPassword == currentPassword) return ApplicationResult.Failure(
                "new_password_equals_old_password",
                "New password equals old password",
                ApplicationErrorType.Validation
                );

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!changePasswordResult.Succeeded) return ApplicationResult.Failure(
                "password_change_failed",
                "Password change failed",
                ApplicationErrorType.Validation
                );

            await _signInManager.RefreshSignInAsync(user);

            return ApplicationResult.Success();
        }


        public async Task<ApplicationResult> RequestPasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null) return ApplicationResult.Success();

            if (user.Status != UserStatus.Active) return ApplicationResult.Success();

            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = PasswordResetTokenCodec.Encode(passwordResetToken);

            var resetPasswordUrl = QueryHelpers.AddQueryString(
                _frontendOptions.ResetPasswordUrl,
                new Dictionary<string, string?>
                {
                    ["email"] = user.Email,
                    ["token"] = encodedToken
                }
                );

            await _emailService.SendEmailAsync(
                user.Email!,
                "Newsflow - Password Reset",
                $"""
                A password reset was requested for your Newsflow account.

                Use the link below to create a new password:

                {resetPasswordUrl}

                If you did not request this change, you can ignore this email.
                """
                );

            return ApplicationResult.Success();
        }


        public async Task<ApplicationResult> ResetPasswordAsync(string encodedToken, string email, string newPassword)
        {
            if (!PasswordResetTokenCodec.TryDecode(
                encodedToken,
                out var passwordResetToken
                ))
            {
                return ApplicationResult.Failure(
                    "invalid_reset_token",
                    "Invalid reset token.",
                    ApplicationErrorType.Validation
                    );
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null) return ApplicationResult.Failure(
                "invalid_reset_token",
                "Invalid reset token.",
                ApplicationErrorType.Validation
                );

            if (user.Status != UserStatus.Active) return ApplicationResult.Failure(
                "invalid_reset_token",
                "Invalid reset token.",
                ApplicationErrorType.Validation
                );

            var passwordResetResult = await _userManager.ResetPasswordAsync(user, passwordResetToken, newPassword);

            if (!passwordResetResult.Succeeded) return ApplicationResult.Failure(
                "failed_to_reset_password",
                "Failed to reset password.",
                ApplicationErrorType.Validation
                );

            return ApplicationResult.Success();
        }
    }
}
