using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Authentication;
using NewsflowApi.Application.Common;
using NewsflowApi.Contracts.Authentication;
using NewsflowApi.Extensions.Http;

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

            if (!result.Succeeded)
            {
                return result.ToErrorResult();
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.SignOutAsync();

            if (!result.Succeeded)
            {
                return result.ToErrorResult();
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
