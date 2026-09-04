using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Authentication;
using NewsflowApi.Application.Authorization;
using NewsflowApi.Application.Common;
using NewsflowApi.Application.Contracts.Requests.Authentication;
using NewsflowApi.Application.Contracts.Responses.Authentication;
using NewsflowApi.Presentation.Extensions.Http;
using System.Security.Claims;

namespace NewsflowApi.Presentation.Controllers.Authentication
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(AuthService authService) : ControllerBase
    {
        private readonly AuthService _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.SignInAsync(request.Email, request.Password);

            if (!result.Succeeded)
            {
                return result.ToErrorResult();
            }

            return StatusCode(StatusCodes.Status200OK);
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

            return StatusCode(StatusCodes.Status204NoContent);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var response = new CurrentUserResponse
            {
                Authenticated = User.Identity?.IsAuthenticated ?? false,
                Name = User.Identity?.Name,
                Roles = [.. User.Claims.Where(claim
                => claim.Type == ClaimTypes.Role)
                .Select(claim => claim.Value)],
                Permissions = [.. User.Claims.Where(claim
                => claim.Type == AuthorizationClaimTypes.Permission)
                .Select(claim => claim.Value)]
            };

            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}
