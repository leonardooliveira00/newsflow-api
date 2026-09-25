namespace NewsflowApi.Presentation.Dtos.Responses.Authentication
{
    public sealed record CurrentUserResponse
    {
        public bool Authenticated { get; init; }

        public string? Name { get; init; }

        public required IReadOnlyCollection<string> Roles { get; init; }

        public required IReadOnlyCollection<string> Permissions { get; init; }
    }
}
