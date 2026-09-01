namespace NewsflowApi.Application.Authorization
{
    public class UserAuthorizationContext
    {
        public required List<string> Roles { get; init; }

        public required List<string> Permissions { get; init; }
    }
}
