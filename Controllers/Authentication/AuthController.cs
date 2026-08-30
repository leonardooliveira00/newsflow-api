using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Authentication;
using NewsflowApi.Application.Common;
using NewsflowApi.Contracts.Authentication;

namespace NewsflowApi.Controllers.Authentication
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.SignInAsync(request.Email, request.Password);

            if (result.Succeeded) return Ok();

            return result.ErrorCode switch
            {
                "invalid_credentials" => Unauthorized(new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),

                "user_not_active" => StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),

                "user_locked_out" => StatusCode(StatusCodes.Status423Locked, new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),

                "signin_not_allowed" => StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),

                "two_factor_required" => Unauthorized(new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.SignOutAsync();

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                authenticated = User.Identity?.IsAuthenticated,
                name = User.Identity?.Name
            });
        }
    }
}
