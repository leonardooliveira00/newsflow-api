using NewsflowApi.Application.Common.Application;

namespace NewsflowApi.Application.Common.Errors
{
    public static class UserErrors
    {
        public static readonly ApplicationError NotFound = new(
            "user_not_found",
            "User not found.",
            ApplicationErrorType.NotFound
            );

        public static readonly ApplicationError NotPendingActivation = new(
            "user_not_pending_activation",
            "User is not pending activation",
            ApplicationErrorType.Conflict
            );

        public static readonly ApplicationError NotActive = new(
            "user_not_active",
            "User not active.",
            ApplicationErrorType.Conflict
            );

        public static readonly ApplicationError UserAlreadyExists = new(
            "user_already_exists",
            "User already exists.",
            ApplicationErrorType.Conflict
            );

        public static readonly ApplicationError EmailAlreadyInUse = new(
            "email_already_in_use",
            "Email already in use.",
            ApplicationErrorType.Conflict
            );

        public static readonly ApplicationError UserCreationFailed = new(
            "user_creation_failed",
            "User creation failed.",
            ApplicationErrorType.Validation
            );
    }
}
