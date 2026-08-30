using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Authentication;
using NewsflowApi.Contracts.Authentication;

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

            if (result.Succeeded)
            {
                return Ok(new
                {
                    token = result.Data
                });
            }

            return result.ErrorCode switch
            {
                "user_not_found" => NotFound(new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),

                "user_not_pending" => Conflict(new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationRequest request)
        {
            var result = await _invitationService.AcceptInvitationTokenAsync(
                request.UserId,
                request.Token,
                request.Password
                );

            if (result.Succeeded)
            {
                return Ok();
            }

            return result.ErrorCode switch
            {
                "user_not_found" => NotFound(new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),
                "user_not_pending" => Conflict(new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),
                "invalid_invitation_token" => BadRequest(new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),
                "password_creation_failed" => BadRequest(new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),
                "user_activation_failed" => StatusCode(
                    StatusCodes.Status500InternalServerError, new
                    {
                        code = result.ErrorCode,
                        message = result.ErrorMessage
                    }),

                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
