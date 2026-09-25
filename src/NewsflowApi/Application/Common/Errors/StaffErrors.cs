using NewsflowApi.Application.Common.Application;

namespace NewsflowApi.Application.Common.Errors
{
    public static class StaffErrors
    {
        public static readonly ApplicationError NotFound = new(
            "staff_not_found",
            "Staff not found.",
            ApplicationErrorType.NotFound
            );

        public static readonly ApplicationError NotActive = new(
            "staff_not_active",
            "Staff is not active.",
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
