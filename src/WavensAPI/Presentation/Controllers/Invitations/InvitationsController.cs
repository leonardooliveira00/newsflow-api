using Microsoft.AspNetCore.Mvc;
using WavensApi.Application.Invitation;
using WavensApi.Presentation.Dtos.Requests.Authentication;
using WavensApi.Presentation.Extensions.Http;

namespace WavensApi.Presentation.Controllers.Invitations
{
    [Route("api/invitations")]
    [ApiController]
    public class InvitationsController(InvitationService invitationService) : ControllerBase
    {
        private readonly InvitationService _invitationService = invitationService;

        [HttpPost("{userId:guid}")]
        public async Task<IActionResult> GenerateInvitation(Guid userId)
        {
            var result = await _invitationService.GenerateInvitationTokenAsync(userId);

            if (!result.Succeeded) return result.ToErrorResult();

            return Ok();
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationRequest request)
        {
            var result = await _invitationService.AcceptInvitationTokenAsync(
                request.UserId,
                request.Token,
                request.Password
                );

            if (!result.Succeeded) return result.ToErrorResult();

            return Ok();
        }
    }
}