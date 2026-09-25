using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using NewsflowApi.Application.Common.Application;
using NewsflowApi.Application.Common.Errors;
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

            if (user is null) return ApplicationResult.Failure(UserErrors.NotFound);

            if (user.Status != UserStatus.Pending) return ApplicationResult.Failure(UserErrors.NotPendingActivation);

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

        private async Task<ApplicationResult<User>> ValidateInvitationTokenAsync(Guid userId, string invitationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ApplicationResult<User>.Failure(UserErrors.NotFound);

            if (user.Status != UserStatus.Pending) return ApplicationResult<User>.Failure(UserErrors.NotPendingActivation);

            var isTokenValid = await _userManager.VerifyUserTokenAsync(
                user,
                TokenOptions.DefaultProvider,
                InvitationTokenPurpose,
                invitationToken
                );

            if (!isTokenValid) return ApplicationResult<User>.Failure(InvitationErrors.InvalidInvitationToken);

            return ApplicationResult<User>.Success(user);
        }

        public async Task<ApplicationResult> AcceptInvitationTokenAsync(Guid userId, string encodedToken, string password)
        {
            if (!InvitationTokenCodec.TryDecode(encodedToken, out var invitationToken))
            {
                return ApplicationResult.Failure(InvitationErrors.InvalidInvitationToken);
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

                var error = new ApplicationError(
                    AuthErrors.PasswordCreationFailed.Code,
                    errorMessage,
                    AuthErrors.PasswordCreationFailed.Type
                    );

                return ApplicationResult.Failure(error);
            }

            user.EmailConfirmed = true;
            user.Status = UserStatus.Active;

            var updatedResult = await _userManager.UpdateAsync(user);

            if (!updatedResult.Succeeded)
            {
                throw new InvalidOperationException();
            }

            await transaction.CommitAsync();

            return ApplicationResult.Success();
        }

    }
}
