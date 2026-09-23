namespace NewsflowApi.Application.Contracts.Responses.Staffs
{
    public sealed class UpdateStaffResponse
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string ContactPhone { get; init; } = null!;
        public string? Bio { get; init; }
    }
}
