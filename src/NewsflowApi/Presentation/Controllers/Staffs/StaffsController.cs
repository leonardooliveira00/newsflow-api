using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsflowApi.Application.Contracts.Requests.Staffs;
using NewsflowApi.Application.Contracts.Responses.Staffs;
using NewsflowApi.Application.Staffs;
using NewsflowApi.Domain.Constants.Authorization;
using NewsflowApi.Presentation.Extensions.Http;

namespace NewsflowApi.Presentation.Controllers.Staffs
{
    [Route("api/staffs")]
    [ApiController]
    public class StaffsController(StaffService staffService) : ControllerBase
    {
        private readonly StaffService _staffService = staffService;

        [Authorize(Policy = PermissionConstants.CreateStaff)]
        [HttpPost()]
        public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffRequest request)
        {
            var result = await _staffService.RegisterStaffAsync(request);

            if (!result.Succeeded) return result.ToErrorResult();

            var staff = result.Data!;

            var response = new RegisterStaffResponse
            {
                Id = staff.Id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Email = staff.Email,
                ContactPhone = staff.ContactPhone,
                Bio = staff.Bio,
                CreatedAt = staff.CreatedAt,
                UpdatedAt = staff.UpdatedAt,
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }
    }
}
