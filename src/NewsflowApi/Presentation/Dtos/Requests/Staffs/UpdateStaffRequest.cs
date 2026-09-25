namespace NewsflowApi.Presentation.Dtos.Requests.Staffs
{
    public sealed record UpdateStaffRequest
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? Email { get; init; }
        public string? ContactPhone { get; init; }
        public string? Bio { get; init; }
    }
}
