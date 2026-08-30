using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Authentication;
using NewsflowApi.Contracts.Authentication;
using NewsflowApi.Extensions.Http;

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
