using System.ComponentModel.DataAnnotations;

namespace NewsflowApi.Contracts.Staffs
{
    public class RegisterStaffRequest
    {
        [Required]
        [MaxLength(50, ErrorMessage = "The field FirstName must be at most 50 characters long.")]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field LastName must be at most 50 characters long.")]
        public required string LastName { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = ("The field Email must be at most 255 characters long."))]
        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        public required string Email { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "The field ContactPhone must be at most 20 characters long.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public required string ContactPhone { get; set; }

        [MaxLength(1000, ErrorMessage = "The field Bio must be at most 1000 characters long.")]
        public string? Bio { get; set; }
    }
}