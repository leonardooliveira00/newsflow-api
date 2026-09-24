using NewsflowApi.Application.Common.Application;

namespace NewsflowApi.Application.Common.Errors
{
    public static class AuthorizationErrors
    {
        public static readonly ApplicationError RoleNotFound = new(
            "role_not_found",
            "Role not found.",
            ApplicationErrorType.NotFound
            );

        public static readonly ApplicationError RoleAlreadyAssignedToUser = new(
            "role_already_assigned_to_user",
            "Role already assigned to user.",
            ApplicationErrorType.Conflict
            );

        public static readonly ApplicationError RoleNotAssignedToUser = new(
            "role_not_assigned_to_user",
            "Role not assigned to user.",
            ApplicationErrorType.Conflict
            );
    }
}
