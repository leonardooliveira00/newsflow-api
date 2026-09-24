using NewsflowApi.Application.Common.Application;

namespace NewsflowApi.Application.Common.Errors
{
    public static class InvitationErrors
    {
        public static readonly ApplicationError InvalidInvitationToken = new(
            "invalid_invitation_token",
            "Invitation token invalid or expired.",
            ApplicationErrorType.Validation
            );

        public static readonly ApplicationError FailedToAcceptInvitation = new(
            "failed_to_accept_invitation",
            "Failed to accept the invitation.",
            ApplicationErrorType.Validation
            );
    }
}
