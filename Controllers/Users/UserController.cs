using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Authorization;
using NewsflowApi.Contracts.Authorization;
using NewsflowApi.Extensions.Http;

namespace NewsflowApi.Controllers.Users
{
    [Route("api/users")]
    [ApiController]
    public class UserController(RoleAssignmentService roleAssignmentService) : ControllerBase
    {
        private readonly RoleAssignmentService _roleAssignmentService = roleAssignmentService;

        [Authorize(Policy = "ROLE_MANAGE")]
        [HttpPost("{userId:guid}/roles")]
        public async Task<IActionResult> AssignRole(Guid userId, [FromBody] AssignRoleRequest request)
        {
            var result = await _roleAssignmentService.AssignRoleAsync(userId, request.RoleName);

            if (!result.Succeeded) return result.ToErrorResult();

            return NoContent();
        }
    }
}
