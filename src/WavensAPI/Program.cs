using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WavensApi.Infrastructure.DependencyInjection;
using WavensApi.Infrastructure.Hosting;
using WavensApi.Infrastructure.Identity;
using WavensApi.Infrastructure.Persistence;
using WavensApi.Presentation.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWavensDatabase(builder.Configuration);
builder.Services.AddWavensDataProtection();
builder.Services.AddWavensIdentity(builder.Configuration);
builder.Services.AddWavensApplication(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "See the errors property for details.",
                Instance = context.HttpContext.Request.Path
            };

            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json", "application/problem+xml" }
            };
        };
    });

var AllowedOrigins = "_AllowedOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowedOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
            .AllowCredentials();
        });
});

builder.Services.AddOpenApi();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddWavensDevelopment(builder.Configuration);
}

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<WavensDbContext>();

    context.Database.Migrate();
}

await app.SeedDataAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(AllowedOrigins);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();