using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsflowApi.Application.Common;
using NewsflowApi.Application.Common.Application;
using NewsflowApi.Application.Common.Errors;
using NewsflowApi.Application.Common.Pagination;
using NewsflowApi.Application.Contracts.Requests.Staffs;
using NewsflowApi.Application.Contracts.Responses.Staffs;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Entities.Staffs;
using NewsflowApi.Domain.Enums.Identity.Users;
using NewsflowApi.Domain.Enums.Staffs;
using NewsflowApi.Infrastructure.Persistence;
using Npgsql;
using System.Security.Claims;
using System.Transactions;

namespace NewsflowApi.Application.Staffs
{
    public class StaffService(
        NewsflowDbContext newsflowDbContext,
        UserManager<User> userManager)
    {
        private readonly NewsflowDbContext _context = newsflowDbContext;
        private readonly UserManager<User> _userManager = userManager;

        public async Task<ApplicationResult<Staff>> RegisterStaffAsync(RegisterStaffRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var staffExists = await _context.Staffs.AnyAsync(staff => staff.Email == normalizedEmail);

            if (staffExists) return ApplicationResult<Staff>.Failure(StaffErrors.StaffAlreadyExists);

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
            catch (DbUpdateException ex)
                when (
                    ex.InnerException is PostgresException pgEx &&
                    pgEx.SqlState == PostgresErrorCodes.UniqueViolation &&
                    pgEx.ConstraintName == "IX_Staffs_Email")
            {
                return ApplicationResult<Staff>.Failure(StaffErrors.EmailAlreadyInUse);
            }

            return ApplicationResult<Staff>.Success(staff);
        }

        public async Task<ApplicationResult<KeysetPagedResult<StaffListItem>>> ListAllStaffsAsync(
            StaffStatus? status,
            StaffKeysetCursor? currentCursor,
            int pageSize = 20)
        {
            if (pageSize <= 0 || pageSize > 100) return ApplicationResult<KeysetPagedResult<StaffListItem>>.Failure(KeysetPaginationErrors.InvalidPageSize);

            var query = _context.Staffs
                .AsNoTracking()
                .AsQueryable();

            if (status.HasValue) query = query.Where(staff => staff.Status == status.Value);

            if (currentCursor is not null)
            {
                query = query.Where(staff =>
                    EF.Functions.LessThan(
                        ValueTuple.Create(staff.CreatedAt, staff.Id),
                        ValueTuple.Create(currentCursor.CreatedAt, currentCursor.Id)
                        ));
            }

            query = query
                .OrderByDescending(staff => staff.CreatedAt)
                .ThenByDescending(staff => staff.Id)
                ;

            var staffs = await query
                .Take(pageSize + 1)
                .Select(staff => new StaffListItem
                {
                    Id = staff.Id,
                    FirstName = staff.FirstName,
                    LastName = staff.LastName,
                    CreatedAt = staff.CreatedAt,
                })
                .ToListAsync();

            var hasMore = staffs.Count > pageSize;

            if (hasMore) staffs.RemoveAt(staffs.Count - 1);

            var lastStaff = staffs.LastOrDefault();

            string? nextCursor = null;

            if (hasMore && lastStaff is not null)
            {
                var nextCursorData = new StaffKeysetCursor(
                    lastStaff.CreatedAt,
                    lastStaff.Id);

                nextCursor = KeysetCursorCodec.Encode(nextCursorData);
            }

            var pagedResult = new KeysetPagedResult<StaffListItem>
            {
                Items = staffs,
                HasMore = hasMore,
                NextCursor = nextCursor
            };

            return ApplicationResult<KeysetPagedResult<StaffListItem>>.Success(pagedResult);
        }

        public async Task<ApplicationResult<Staff>> FindStaffByIdAsync(Guid staffId)
        {
            var staff = await _context.Staffs
                .AsNoTracking()
                .FirstOrDefaultAsync(staff => staff.Id == staffId);

            if (staff is null) return ApplicationResult<Staff>.Failure(StaffErrors.NotFound);

            return ApplicationResult<Staff>.Success(staff);
        }

        public async Task<ApplicationResult<Staff>> UpdateStaffAsync(UpdateStaffRequest request, Guid staffId)
        {
            var staff = await _context.Staffs.FirstOrDefaultAsync(staff => staff.Id == staffId);

            if (staff is null) return ApplicationResult<Staff>.Failure(StaffErrors.NotFound);

            if (staff.Status != StaffStatus.Active) return ApplicationResult<Staff>.Failure(StaffErrors.NotActive);

            UpdateHelper.UpdateIfProvided(
                request.FirstName,
                value => staff.FirstName = value.Trim()
                );

            UpdateHelper.UpdateIfProvided(
                request.LastName,
                value => staff.LastName = value.Trim()
                );

            UpdateHelper.UpdateIfProvided(
                request.ContactPhone,
                value => staff.ContactPhone = value.Trim()
                );

            UpdateHelper.UpdateIfProvided(
                request.Bio,
                value => staff.Bio = value.Trim()
                );

            if (request.Email is not null)
            {
                var normalizedEmail = request.Email.Trim().ToLowerInvariant();

                if (normalizedEmail != staff.Email)
                {
                    var emailAlreadyInUse = await _context.Staffs
                    .AnyAsync(otherStaff => otherStaff.Id != staffId &&
                              otherStaff.Email == normalizedEmail
                    );

                    if (emailAlreadyInUse) return ApplicationResult<Staff>.Failure(StaffErrors.EmailAlreadyInUse);

                    staff.Email = normalizedEmail;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is PostgresException postgresException &&
                    postgresException.SqlState == PostgresErrorCodes.UniqueViolation &&
                    postgresException.ConstraintName == "IX_Staffs_Email"
                )
            {
                return ApplicationResult<Staff>.Failure(StaffErrors.EmailAlreadyInUse);
            }

            return ApplicationResult<Staff>.Success(staff);
        }

        public async Task<ApplicationResult> DeactivateStaffAsync(Guid staffId)
        {
            var staff = await _context.Staffs
                .Include(staff => staff.User)
                .FirstOrDefaultAsync(staff => staff.Id == staffId);

            if (staff is null) return ApplicationResult.Failure(StaffErrors.NotFound);

            if (staff.Status != StaffStatus.Active) return ApplicationResult.Failure(StaffErrors.NotActive);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            staff.Status = StaffStatus.Inactive;

            if (staff.User is not null)
            {
                staff.User.Status = UserStatus.Inactive;

                var stampResult = await _userManager.UpdateSecurityStampAsync(staff.User);

                if (!stampResult.Succeeded)
                {
                    throw new InvalidOperationException();
                }
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return ApplicationResult.Success();
        }

    }
}
