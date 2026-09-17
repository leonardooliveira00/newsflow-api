using System.ComponentModel.DataAnnotations;

namespace NewsflowApi.Application.Contracts.Requests.Authentication
{
    public sealed record CreateUserForStaffRequest
    {
        public Guid StaffId { get; init; }

        [Required]
        [MaxLength(255, ErrorMessage = ("The field Email must be at most 255 characters long."))]
        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        public required string Email { get; init; }
    }
}
