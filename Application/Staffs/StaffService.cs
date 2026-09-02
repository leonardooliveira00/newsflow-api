using Microsoft.EntityFrameworkCore;
using NewsflowApi.Application.Common;
using NewsflowApi.Contracts.Staffs;
using NewsflowApi.Data;
using NewsflowApi.Domain.Identity.Staffs;

namespace NewsflowApi.Application.Staffs
{
    public class StaffService(NewsflowDbContext newsflowDbContext)
    {
        private readonly NewsflowDbContext _context = newsflowDbContext;

        public async Task<ApplicationResult<Staff>> RegisterStaffAsync(RegisterStaffRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var staffExists = await _context.Staffs.AnyAsync(staff => staff.Email == normalizedEmail);

            if (staffExists) return ApplicationResult<Staff>.Failure(
                "staff_already_exists",
                "Staff already exists.",
                ApplicationErrorType.Conflict
                );

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = normalizedEmail,
                ContactPhone = request.ContactPhone.Trim(),
                Bio = request.Bio?.Trim(),
            };

            _context.Staffs.Add(staff);

            await _context.SaveChangesAsync();

            return ApplicationResult<Staff>.Success(staff);
        }

    }
}
