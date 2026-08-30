using NewsflowApi.Data.Seeds;

namespace NewsflowApi.Extensions
{
    public static class DevelopmentSeedExtensions
    {
        public static async Task DevelopmentSeedDataAsync(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                return;
            }

            await using var scope = app.Services.CreateAsyncScope();

            var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentSeeder>();

            await seeder.SeedAsync();
        }
    }
}
