using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Authentication;
using NewsflowApi.Contracts.Authentication;
using NewsflowApi.Extensions.Http;

namespace NewsflowApi.Controllers.Authentication
{
    [Route("api/access")]
    [ApiController]
    public class AccessController(AuthService authService) : ControllerBase
    {
        private readonly AuthService _authService = authService;

        [HttpPost]
        public async Task<IActionResult> CreateAccess([FromBody] CreateUserForStaffRequest request)
        {
            var result = await _authService.CreateUserForStaffAsync(request.StaffId, request.Email);

            if (!result.Succeeded)
            {
                return result.ToErrorResult();
            }

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
