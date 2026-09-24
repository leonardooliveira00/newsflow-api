using NewsflowApi.Application.Common.Application;

namespace NewsflowApi.Application.Common.Errors
{
    public static class AuthErrors
    {
        public static readonly ApplicationError InvalidResetToken = new(
            "invalid_reset_token",
            "Invalid reset token.",
            ApplicationErrorType.Validation
            );

        public static readonly ApplicationError ExpiredResetToken = new(
            "expired_reset_token",
            "Expired reset token.",
            ApplicationErrorType.Validation
            );

        public static readonly ApplicationError PasswordCreationFailed = new(
            "password_creation_failed",
            "Password creation failed.",
            ApplicationErrorType.Validation
            );

        public static readonly ApplicationError PasswordResetFailed = new(
            "password_reset_failed",
            "Failed to reset password.",
            ApplicationErrorType.Validation
            );

        public static readonly ApplicationError PasswordChangeFailed = new(
            "password_change_failed",
            "Password change failed.",
            ApplicationErrorType.Validation
            );

        public static readonly ApplicationError InvalidCurrentPassword = new(
            "invalid_current_password",
            "Current password is invalid.",
            ApplicationErrorType.Validation
            );

        public static readonly ApplicationError NewPasswordEqualsCurrentPassword = new(
            "new_password_equals_current_password",
            "The new password is equal to the current password.",
            ApplicationErrorType.Validation
            );

        public static readonly ApplicationError UserNotActive = new(
            "user_not_active",
            "User is not active.",
            ApplicationErrorType.Forbidden
            );

        public static readonly ApplicationError InvalidCredentials = new(
            "invalid_credentials",
            "Invalid email or password.",
            ApplicationErrorType.Unauthorized
            );

        public static readonly ApplicationError UserLockedOut = new(
            "user_locked_out",
            "User account is temporarily locked.",
            ApplicationErrorType.Locked
            );

        public static readonly ApplicationError SignInNotAllowed = new(
            "signin_not_allowed",
            "Sign in is not allowed for this account.",
            ApplicationErrorType.Forbidden
            );
    }
}
