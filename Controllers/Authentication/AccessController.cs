using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Authentication;
using NewsflowApi.Contracts.Authentication;

namespace NewsflowApi.Controllers.Authentication
{
    [Route("api/access")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly AuthService _authService;

        public AccessController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateAccess([FromBody] CreateUserForStaffRequest request)
        {
            var result = await _authService.CreateUserForStaffAsync(request.StaffId, request.Email);

            if (result.Succeeded) return StatusCode(StatusCodes.Status201Created);

            return result.ErrorCode switch
            {
                "staff_not_found" => NotFound(new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),

                "staff_already_has_user" or "email_already_in_use" => Conflict(new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),

                "user_creation_failed" => BadRequest(new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
