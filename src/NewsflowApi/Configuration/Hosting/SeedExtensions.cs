using NewsflowApi.Data.Seeds;

namespace NewsflowApi.Configuration.Hosting
{
    public static class SeedExtensions
    {
        public static async Task SeedDataAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();

            var structualSeeder = scope.ServiceProvider.GetRequiredService<StructuralSeeder>();

            await structualSeeder.SeedAsync();

            if (!app.Environment.IsDevelopment()) return;

            var developmentSeeder = scope.ServiceProvider.GetRequiredService<DevelopmentSeeder>();

            await developmentSeeder.SeedAsync();
        }
    }
}
