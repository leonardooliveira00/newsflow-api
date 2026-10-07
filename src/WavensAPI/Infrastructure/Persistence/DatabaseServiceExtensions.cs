using Microsoft.EntityFrameworkCore;
using WavensApi.Infrastructure.Persistence.Data.Seeds;

namespace WavensApi.Infrastructure.Persistence;

public static class DatabaseServiceExtensions
{
    public static IServiceCollection AddWavensDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection String 'DefaultConnection' not found."
            );

        services.AddDbContext<WavensDbContext>(options =>
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