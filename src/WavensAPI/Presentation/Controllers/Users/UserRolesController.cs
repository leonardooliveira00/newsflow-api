using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WavensApi.Application.Authorization;
using WavensApi.Domain.Constants.Authorization;
using WavensApi.Presentation.Extensions.Http;

namespace WavensApi.Presentation.Controllers.Users
{
    [Route("api/users/{userId:guid}/roles")]
    [ApiController]
    public class UserRolesController(UserRoleManagementService userRoleManagementService) : ControllerBase
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
