using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Authentication;
using NewsflowApi.Presentation.Dtos.Requests.Authentication;
using NewsflowApi.Presentation.Dtos.Responses.Authentication;
using NewsflowApi.Presentation.Extensions.Http;

namespace NewsflowApi.Presentation.Controllers.Authentication
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