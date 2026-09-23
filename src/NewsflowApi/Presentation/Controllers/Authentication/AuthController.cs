using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Authentication;
using NewsflowApi.Application.Authorization;
using NewsflowApi.Application.Common;
using NewsflowApi.Application.Contracts.Requests.Authentication;
using NewsflowApi.Application.Contracts.Responses.Authentication;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Presentation.Extensions.Http;
using System.Security.Claims;

namespace NewsflowApi.Presentation.Controllers.Authentication
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(AuthService authService, UserManager<User> userManager) : ControllerBase
    {
        private readonly AuthService _authService = authService;
        private readonly UserManager<User> _userManager = userManager;

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

        [Authorize]
        [HttpPost("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = _userManager.GetUserId(User);

            if (userId is null) return StatusCode(StatusCodes.Status401Unauthorized);

            var result = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded) return result.ToErrorResult();

            return StatusCode(StatusCodes.Status204NoContent);
        }


        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetRequest request)
        {
            var result = await _authService.RequestPasswordResetAsync(request.Email);

            if (!result.Succeeded) return result.ToErrorResult();

            return StatusCode(StatusCodes.Status200OK);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request.Token, request.Email, request.NewPassword);

            if (!result.Succeeded) return result.ToErrorResult();

            return NoContent();
        }
    }
}
