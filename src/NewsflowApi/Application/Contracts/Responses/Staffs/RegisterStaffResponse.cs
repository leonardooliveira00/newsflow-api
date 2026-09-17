namespace NewsflowApi.Application.Contracts.Responses.Staffs
{
    public sealed record RegisterStaffResponse
    {
        public Guid Id { get; init; }

        public required string FirstName { get; init; }

        public required string LastName { get; init; }

        public required string Email { get; init; }

        public required string ContactPhone { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; init; }

        public string? Bio { get; init; }
    }
}
