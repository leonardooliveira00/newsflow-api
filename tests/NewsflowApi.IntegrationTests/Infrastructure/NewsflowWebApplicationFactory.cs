using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NewsflowApi.IntegrationTests.Infrastructure
{
    public sealed class NewsflowWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString) ||
                !connectionString.Contains("newsflow_test"))
            {
                throw new InvalidOperationException(
                    "Integration tests must use the test database.");
            }

            builder.ConfigureTestServices(services =>
            {
                services.PostConfigure<SecurityStampValidatorOptions>(options =>
                {
                    options.ValidationInterval = TimeSpan.Zero;
                });
            });
        }
    }
}
