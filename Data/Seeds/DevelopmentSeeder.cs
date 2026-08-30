using Microsoft.EntityFrameworkCore;
using NewsflowApi.Domain.Identity.Staffs;

namespace NewsflowApi.Data.Seeds
{
    public class DevelopmentSeeder
    {
        private readonly NewsflowDbContext _context;

        public DevelopmentSeeder(NewsflowDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.Staffs.AnyAsync())
            {
                return;
            }

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Reporter"
            };

            _context.Staffs.Add(staff);

            await _context.SaveChangesAsync();
        }
    }
}
