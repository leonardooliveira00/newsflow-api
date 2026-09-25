namespace NewsflowApi.Presentation.Dtos.Responses.Staffs
{
    public sealed record ListStaffsResponse
    {
        public Guid Id { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
    }
}
