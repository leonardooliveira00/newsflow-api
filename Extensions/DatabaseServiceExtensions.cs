using Microsoft.EntityFrameworkCore;
using NewsflowApi.Data;

namespace NewsflowApi.Extensions;

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

        return services;
    }
}