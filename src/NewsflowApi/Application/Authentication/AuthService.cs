using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewsflowApi.Application.Authorization;
using NewsflowApi.Application.Common.Application;
using NewsflowApi.Application.Common.Errors;
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

            if (staff is null) return ApplicationResult.Failure(StaffErrors.NotFound);

            if (staff.User is not null) return ApplicationResult.Failure(UserErrors.UserAlreadyExists);

            var existingEmail = await _userManager.FindByEmailAsync(email);

            if (existingEmail is not null) return ApplicationResult.Failure(UserErrors.EmailAlreadyInUse);

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
                    UserErrors.UserCreationFailed.Code,
                    errorMessage,
                    UserErrors.UserCreationFailed.Type
                    );

                return ApplicationResult.Failure(error);
            }

            return ApplicationResult.Success();
        }

        public async Task<ApplicationResult> SignInAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null) return ApplicationResult.Failure(AuthErrors.InvalidCredentials);

            if (user.Status != UserStatus.Active) return ApplicationResult.Failure(AuthErrors.UserNotActive);

            var signInResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: true
                );

            if (signInResult.IsLockedOut) return ApplicationResult.Failure(AuthErrors.UserLockedOut);

            if (signInResult.IsNotAllowed) return ApplicationResult.Failure(AuthErrors.SignInNotAllowed);

            if (!signInResult.Succeeded) return ApplicationResult.Failure(AuthErrors.InvalidCredentials);

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

            if (user is null) return ApplicationResult.Failure(UserErrors.NotFound);

            if (user.Status != UserStatus.Active) return ApplicationResult.Failure(AuthErrors.UserNotActive);

            var verifyPassword = await _userManager.CheckPasswordAsync(user, currentPassword);

            if (!verifyPassword) return ApplicationResult.Failure(AuthErrors.InvalidCurrentPassword);

            if (newPassword == currentPassword) return ApplicationResult.Failure(AuthErrors.NewPasswordEqualsCurrentPassword);

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!changePasswordResult.Succeeded)
            {
                var errorMessage = string.Join(
                    " ",
                    changePasswordResult.Errors.Select(error => error.Description)
                    );

                var error = new ApplicationError(
                    AuthErrors.PasswordChangeFailed.Code,
                    errorMessage,
                    AuthErrors.PasswordChangeFailed.Type
                    );

                return ApplicationResult.Failure(error);
            }

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
                return ApplicationResult.Failure(AuthErrors.InvalidResetToken);
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null) return ApplicationResult.Failure(AuthErrors.InvalidResetToken);

            if (user.Status != UserStatus.Active) return ApplicationResult.Failure(AuthErrors.InvalidResetToken);

            var passwordResetResult = await _userManager.ResetPasswordAsync(user, passwordResetToken, newPassword);

            if (!passwordResetResult.Succeeded)
            {
                var errorMessage = string.Join(
                    " ",
                    passwordResetResult.Errors.Select(error => error.Description)
                    );

                var error = new ApplicationError(
                    AuthErrors.PasswordResetFailed.Code,
                    errorMessage,
                    AuthErrors.PasswordResetFailed.Type
                    );

                return ApplicationResult.Failure(error);
            }

            return ApplicationResult.Success();
        }
    }
}
