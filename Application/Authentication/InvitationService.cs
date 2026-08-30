using Microsoft.AspNetCore.Identity;
using NewsflowApi.Application.Common;
using NewsflowApi.Data;
using NewsflowApi.Domain.Identity.Users;

namespace NewsflowApi.Application.Authentication
{
    public class InvitationService
    {
        private readonly UserManager<User> _userManager;
        private readonly NewsflowDbContext _context;

        public InvitationService(UserManager<User> userManager, NewsflowDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        private const string InvitationTokenPurpose = "NewsflowInvitation";

        public async Task<ApplicationResult<string>> GenerateInvitationTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ApplicationResult<string>.Failure(
                "user_not_found",
                "User not found"
                );

            if (user.Status != UserStatus.Pending) return ApplicationResult<string>.Failure(
                "user_not_pending",
                "User is not pending activation."
                );

            var invitationToken = await _userManager.GenerateUserTokenAsync(
                user,
                TokenOptions.DefaultProvider,
                InvitationTokenPurpose
                );

            return ApplicationResult<string>.Success(invitationToken);
        }

        public async Task<ApplicationResult> ValidateInvitationTokenAsync(Guid userId, string invitationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ApplicationResult.Failure(
                "user_not_found",
                "User not found"
                );

            if (user.Status != UserStatus.Pending) return ApplicationResult.Failure(
                "user_not_pending",
                "User is not pending activation."
                );

            var isTokenValid = await _userManager.VerifyUserTokenAsync(
                user,
                TokenOptions.DefaultProvider,
                InvitationTokenPurpose,
                invitationToken
                );

            if (!isTokenValid) return ApplicationResult.Failure(
                "invalid_invitation_toke",
                "Invitation token invalid ou expired."
                );

            return ApplicationResult.Success();
        }

        public async Task<ApplicationResult> AcceptInvitationTokenAsync(Guid userId, string invitationToken, string password)
        {
            var validationResult = await ValidateInvitationTokenAsync(userId, invitationToken);

            if (!validationResult.Succeeded) return validationResult;

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ApplicationResult.Failure(
                "user_not_found",
                "User not found."
                );

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
                    errorMessage
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
                    errorMessage
                    );
            }

            await transaction.CommitAsync();

            return ApplicationResult.Success();
        }

    }
}
