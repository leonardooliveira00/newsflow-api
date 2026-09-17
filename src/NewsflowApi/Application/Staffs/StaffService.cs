using Microsoft.EntityFrameworkCore;
using NewsflowApi.Application.Common;
using NewsflowApi.Application.Contracts.Requests.Staffs;
using NewsflowApi.Application.Contracts.Responses.Staffs;
using NewsflowApi.Data;
using NewsflowApi.Domain.Entities.Staffs;
using Npgsql;

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
                "staff_email_already_exists",
                "A staff member with this email already exists.",
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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException pgEx &&
                pgEx.SqlState == PostgresErrorCodes.UniqueViolation &&
                pgEx.ConstraintName == "IX_Staffs_Email")
            {
                return ApplicationResult<Staff>.Failure(
                    "staff_email_already_exists",
                    "A staff member with this email already exists.",
                    ApplicationErrorType.Conflict
                    );
            }

            return ApplicationResult<Staff>.Success(staff);
        }

    }
}
