using Microsoft.EntityFrameworkCore;
using NewsflowApi.Domain.Entities.Authorization;

namespace NewsflowApi.Data.Seeds
{
    public class RoleSeeder
    {
        private readonly NewsflowDbContext _context;

        public RoleSeeder(NewsflowDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            var roles = new[]
            {
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "ADMIN",
                    Description = "System admnistrator."
                },

                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "MANAGER",
                    Description = "Managment and analytics access."
                },

                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "EDITOR_IN_CHIEF",
                    Description = "Editorial leadership and publishing authority."
                },

                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "EDITOR",
                    Description = "Editorial review and approval."
                },

                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "REPORTER",
                    Description = "Article creation and submission."
                },

                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "PHOTOGRAPHER",
                    Description = "Media upload and management."
                }
            };

            var existingRoles = await _context.Roles.Select(role => role.Name).ToListAsync();

            var missingRoles = roles
                .Where(role => !existingRoles.Contains(role.Name))
                .ToList();

            if (missingRoles.Count == 0)
            {
                return;
            }

            _context.Roles.AddRange(missingRoles);

            await _context.SaveChangesAsync();
        }
    }
}
