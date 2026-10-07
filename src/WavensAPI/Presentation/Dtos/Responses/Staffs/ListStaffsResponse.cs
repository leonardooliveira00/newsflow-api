namespace WavensApi.Presentation.Dtos.Responses.Staffs
{
    public sealed record ListStaffsResponse
    {
        public Guid Id { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Email { get; init; }
    }
}
