using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Authorization;
using NewsflowApi.Domain.Constants.Authorization;
using NewsflowApi.Presentation.Extensions.Http;

namespace NewsflowApi.Presentation.Controllers.Users
{
    [Route("api/users/{userId:guid}/roles")]
    [ApiController]
    public class UsersController(UserRoleManagementService userRoleManagementService) : ControllerBase
    {
        private readonly UserRoleManagementService _userRoleManagementService = userRoleManagementService;

        [Authorize(Policy = PermissionConstants.ManageRole)]
        [HttpPost("{roleId:guid}")]
        public async Task<IActionResult> AssignRole(Guid userId, Guid roleId)
        {
            var result = await _userRoleManagementService.AssignRoleAsync(userId, roleId);

            if (!result.Succeeded) return result.ToErrorResult();

            return NoContent();
        }

        [Authorize(Policy = PermissionConstants.ManageRole)]
        [HttpDelete("{roleId:guid}")]
        public async Task<IActionResult> ResignRole(Guid userId, Guid roleId)
        {
            var result = await _userRoleManagementService.ResignRoleAsync(userId, roleId);

            if (!result.Succeeded) return result.ToErrorResult();

            return NoContent();
        }
    }
}
