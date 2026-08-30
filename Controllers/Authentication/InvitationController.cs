using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Authentication;
using NewsflowApi.Contracts.Authentication;
using NewsflowApi.Extensions.Http;

namespace NewsflowApi.Controllers.Authentication
{
    [Route("api/invitations")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        private readonly InvitationService _invitationService;

        public InvitationController(InvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpPost("{userId:guid}")]
        public async Task<IActionResult> GenerateInvitation(Guid userId)
        {
            var result = await _invitationService.GenerateInvitationTokenAsync(userId);

            if (!result.Succeeded) return result.ToErrorResult();

            return Ok(new
            {
                token = result.Data
            });
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
