using System.ComponentModel.DataAnnotations;

namespace NewsflowApi.Application.Contracts.Requests.Authentication
{
    public sealed record AcceptInvitationRequest
    {
        public Guid UserId { get; init; }

        [Required]
        public required string Token { get; init; }

        [Required]
        public required string Password { get; init; }
    }
}
