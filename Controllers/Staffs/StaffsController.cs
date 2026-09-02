using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Staffs;
using NewsflowApi.Contracts.Staffs;
using NewsflowApi.Extensions.Http;

namespace NewsflowApi.Controllers.Staffs
{
    [Route("api/staffs")]
    [ApiController]
    public class StaffsController(StaffService staffService) : ControllerBase
    {
        private readonly StaffService _staffService = staffService;

        [Authorize(Policy = "STAFF_CREATE")]
        [HttpPost()]
        public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffRequest request)
        {
            var result = await _staffService.RegisterStaffAsync(request);

            if (!result.Succeeded) return result.ToErrorResult();

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
