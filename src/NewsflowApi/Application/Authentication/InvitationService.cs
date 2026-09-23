using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using NewsflowApi.Application.Common;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Enums.Identity.Users;
using NewsflowApi.Infrastructure.Email;
using NewsflowApi.Infrastructure.Persistence;
using NewsflowApi.Infrastructure.Settings;

namespace NewsflowApi.Application.Authentication
{
    public class InvitationService(
        NewsflowDbContext context,
        UserManager<User> userManager,
        IEmailService emailService,
        IOptions<FrontendSettings> options
        )
    {

        private readonly NewsflowDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IEmailService _emailService = emailService;
        private readonly FrontendSettings _options = options.Value;

        private const string InvitationTokenPurpose = "NewsflowInvitation";

        public async Task<ApplicationResult> GenerateInvitationTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ApplicationResult.Failure(
                "user_not_found",
                "User not found",
                ApplicationErrorType.NotFound
                );

            if (user.Status != UserStatus.Pending) return ApplicationResult.Failure(
                "user_not_pending",
                "User is not pending activation.",
                ApplicationErrorType.Conflict
                );

            var invitationToken = await _userManager.GenerateUserTokenAsync(
                user,
                TokenOptions.DefaultProvider,
                InvitationTokenPurpose
                );

            var encodedToken = InvitationTokenCodec.Encode(invitationToken);

            var invitationTokenUrl = QueryHelpers.AddQueryString(
                _options.InvitationTokenUrl,
                new Dictionary<string, string?>
                {
                    ["userId"] = user.Id.ToString(),
                    ["token"] = encodedToken
                }
                );

            await _emailService.SendEmailAsync(
                user.Email!,
                "Newsflow - Invitation",
                $"""
                You have been invited to join Newsflow.

                Use the link below to accept the invitation and create your password:

                {invitationTokenUrl}

                If you were not expecting this invitation, you can ignore this email.
                """
                );

            return ApplicationResult.Success();
        }

        public async Task<ApplicationResult<User>> ValidateInvitationTokenAsync(Guid userId, string invitationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ApplicationResult<User>.Failure(
                "user_not_found",
                "User not found",
                ApplicationErrorType.NotFound
                );

            if (user.Status != UserStatus.Pending) return ApplicationResult<User>.Failure(
                "user_not_pending",
                "User is not pending activation.",
                ApplicationErrorType.Conflict
                );

            var isTokenValid = await _userManager.VerifyUserTokenAsync(
                user,
                TokenOptions.DefaultProvider,
                InvitationTokenPurpose,
                invitationToken
                );

            if (!isTokenValid) return ApplicationResult<User>.Failure(
                "invalid_invitation_token",
                "Invitation token invalid or expired.",
                ApplicationErrorType.Validation
                );

            return ApplicationResult<User>.Success(user);
        }

        public async Task<ApplicationResult> AcceptInvitationTokenAsync(Guid userId, string encodedToken, string password)
        {
            if (!InvitationTokenCodec.TryDecode(encodedToken, out var invitationToken))
            {
                return ApplicationResult.Failure(
                    "invalid_invitation_token",
                    "Invalid invitation token",
                    ApplicationErrorType.Validation
                    );
            }

            var validationResult = await ValidateInvitationTokenAsync(userId, invitationToken);

            if (!validationResult.Succeeded) return validationResult;

            var user = validationResult.Data!;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var passwordResult = await _userManager.AddPasswordAsync(user, password);

            if (!passwordResult.Succeeded)
            {
                await transaction.RollbackAsync();

                var errorMessage = string.Join(
                    " ",
                    passwordResult.Errors.Select(error => error.Description)
                    );

                return ApplicationResult.Failure(
                    "password_creation_failed",
                    errorMessage,
                    ApplicationErrorType.Validation
                    );
            }

            user.EmailConfirmed = true;
            user.Status = UserStatus.Active;

            var updatedResult = await _userManager.UpdateAsync(user);

            if (!updatedResult.Succeeded)
            {
                await transaction.RollbackAsync();

                var errorMessage = string.Join(
                    " ",
                    updatedResult.Errors.Select(error => error.Description)
                    );

                return ApplicationResult.Failure(
                    "user_activation_failed",
                    errorMessage,
                    ApplicationErrorType.Internal
                    );
            }

            await transaction.CommitAsync();

            return ApplicationResult.Success();
        }

    }
}
