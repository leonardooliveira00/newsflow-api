using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WavensApi.Application.Users;
using WavensApi.Presentation.Dtos.Requests.Authentication;
using WavensApi.Presentation.Dtos.Responses.Authentication;
using WavensApi.Presentation.Extensions.Http;

namespace WavensApi.Presentation.Controllers.Users
{
    [Route("api/users")]
    [ApiController]
    public class UsersController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpPost("create-user")]
        public async Task<IActionResult> ProvideAccess([FromBody] CreateUserForStaffRequest request)
        {
            var result = await _userService.CreateUserForStaffAsync(request.StaffId, request.Email);

            if (!result.Succeeded)
            {
                return result.ToErrorResult();
            }

            var user = result.Data!;

            var response = new UserResponse
            {
                StaffId = user.StaffId,
                Id = user.Id,
                Email = user.Email,
                Status = user.Status
            };

            return CreatedAtAction(
                nameof(GetUserById),
                new { userId = user.Id },
                response
                );
        }

        [Authorize]
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var result = await _userService.GetUserByIdAsync(userId);

            if (!result.Succeeded) return result.ToErrorResult();

            var user = result.Data!;

            var response = new UserResponse
            {
                Id = user.Id,
                StaffId = user.StaffId,
                Email = user.Email,
                Status = user.Status
            };

            return Ok(response);
        }
    }
}
