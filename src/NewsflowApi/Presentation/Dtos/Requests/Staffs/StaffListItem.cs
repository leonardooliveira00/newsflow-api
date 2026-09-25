namespace NewsflowApi.Presentation.Dtos.Requests.Staffs
{
    public sealed record StaffListItem
    {
        public Guid Id { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
