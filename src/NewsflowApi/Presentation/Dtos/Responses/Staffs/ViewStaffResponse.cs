using NewsflowApi.Domain.Enums.Staffs;

namespace NewsflowApi.Presentation.Dtos.Responses.Staffs
{
    public sealed record ViewStaffResponse
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string ContactPhone { get; init; } = null!;
        public string? Bio { get; init; }
        public StaffStatus Status { get; init; }
    }
}
