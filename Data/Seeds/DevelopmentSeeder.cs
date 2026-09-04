using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewsflowApi.Configuration.Settings;
using NewsflowApi.Domain.Entities.Authorization;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Entities.Staffs;
using NewsflowApi.Domain.Enums.Identity.Users;

namespace NewsflowApi.Data.Seeds
{
    public class DevelopmentSeeder(NewsflowDbContext context, UserManager<User> userManager, IOptions<DevelopmentAdminSettings> options)
    {
        private readonly NewsflowDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;
        private readonly DevelopmentAdminSettings _settings = options.Value;

        public async Task SeedAsync()
        {
            await SeedStaffsAsync();

            var adminUser = await EnsureAdminUserAsync();

            await EnsureAdminUserRoleAsync(adminUser);
        }

        private async Task SeedStaffsAsync()
        {
            var staffs = new Staff[]
            {
                new() {
                    Id = Guid.NewGuid(),
                    FirstName = "Development",
                    LastName = "Admin",
                    Email = "admin@newsflow.com",
                    ContactPhone = "85998765432",
                },

                new() {
                    Id = Guid.NewGuid(),
                    FirstName = "Test",
                    LastName = "Reporter",
                    Email = "reporter@newsflow.com",
                    ContactPhone = "85912345678"
                }
            };

            foreach (var seedStaff in staffs)
            {
                var staffExists = await _context.Staffs.AnyAsync(dbStaff => dbStaff.Email == seedStaff.Email);

                if (staffExists) continue;

                _context.Staffs.Add(seedStaff);

            }
            await _context.SaveChangesAsync();
        }

        private async Task<User> EnsureAdminUserAsync()
        {
            var adminStaff = await _context.Staffs.FirstOrDefaultAsync(staff => staff.Email == "admin@newsflow.com") ?? throw new InvalidOperationException("Admin staff could not be found.");

            var existingAdminUser = await _context.Users.FirstOrDefaultAsync(user => user.StaffId == adminStaff.Id);

            User adminUser;

            if (existingAdminUser is not null)
            {
                return existingAdminUser;
            }

            adminUser = new User
            {
                Id = Guid.NewGuid(),
                StaffId = adminStaff.Id,
                Email = adminStaff.Email,
                UserName = adminStaff.Email,
                EmailConfirmed = true,
                Status = UserStatus.Active
            };

            var createResult = await _userManager.CreateAsync(adminUser, _settings.Password);
            if (!createResult.Succeeded) throw new InvalidOperationException("Admin user could not be created.");

            return adminUser;
        }

        private async Task EnsureAdminUserRoleAsync(User adminUser)
        {
            var adminRole = await _context.Roles.FirstOrDefaultAsync(role => role.Name == "ADMIN") ?? throw new InvalidOperationException("Admin role could not be found.");

            var alreadyHasRole = await _context.UserRoles.AnyAsync(userRole => userRole.UserId == adminUser.Id && userRole.RoleId == adminRole.Id);

            if (alreadyHasRole) return;

            var adminUserRole = new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id,
                AssignedAt = DateTime.UtcNow,
            };

            _context.UserRoles.Add(adminUserRole);

            await _context.SaveChangesAsync();
        }
    }
}
