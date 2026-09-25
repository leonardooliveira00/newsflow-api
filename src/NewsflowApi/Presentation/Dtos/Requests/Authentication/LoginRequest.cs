using System.ComponentModel.DataAnnotations;

namespace NewsflowApi.Presentation.Dtos.Requests.Authentication
{
    public sealed record LoginRequest
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
