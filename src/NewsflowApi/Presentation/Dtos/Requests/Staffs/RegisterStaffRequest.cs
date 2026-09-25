namespace NewsflowApi.Presentation.Dtos.Requests.Staffs
{
    public sealed record RegisterStaffRequest
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Email { get; init; }
        public required string ContactPhone { get; init; }
        public string? Bio { get; init; }
    }
}