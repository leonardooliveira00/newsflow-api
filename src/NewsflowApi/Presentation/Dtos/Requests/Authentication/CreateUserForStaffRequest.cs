using System.ComponentModel.DataAnnotations;

namespace NewsflowApi.Presentation.Dtos.Requests.Authentication
{
    public sealed record CreateUserForStaffRequest
    {
        public Guid StaffId { get; init; }
        public required string Email { get; init; }
    }
}
