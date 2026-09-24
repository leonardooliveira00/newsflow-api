using NewsflowApi.Application.Common.Application;

namespace NewsflowApi.Application.Common.Errors
{
    public static class StaffErrors
    {
        public static readonly ApplicationError NotFound = new(
            "user_not_found",
            "User not found.",
            ApplicationErrorType.NotFound
            );

        public static readonly ApplicationError NotActive = new(
            "user_not_active",
            "User not active.",
            ApplicationErrorType.Conflict
            );

        public static readonly ApplicationError StaffAlreadyExists = new(
            "staff_already_exists",
            "Staff already exists.",
            ApplicationErrorType.Conflict
            );

        public static readonly ApplicationError EmailAlreadyInUse = new(
            "staff_email_already_in_use",
            "Staff email already in use.",
            ApplicationErrorType.Conflict
            );
    }
}
