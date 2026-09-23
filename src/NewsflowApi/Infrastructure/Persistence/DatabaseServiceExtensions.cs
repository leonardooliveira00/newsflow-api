using Microsoft.EntityFrameworkCore;
using NewsflowApi.Infrastructure.Persistence.Data.Seeds;

namespace NewsflowApi.Infrastructure.Persistence;

public static class DatabaseServiceExtensions
{
    public static IServiceCollection AddNewsflowDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection String 'DefaultConnection' not found."
            );

        services.AddDbContext<NewsflowDbContext>(options =>
            options.UseNpgsql(connectionString)
        );

        services.AddScoped<RoleSeeder>();
        services.AddScoped<PermissionSeeder>();
        services.AddScoped<StructuralSeeder>();
        services.AddScoped<RolePermissionSeeder>();
        services.AddScoped<DevelopmentSeeder>();

        return services;
    }
}