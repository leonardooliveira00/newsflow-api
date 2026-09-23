using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using NewsflowApi.Infrastructure.Hosting;
using NewsflowApi.Infrastructure.DependencyInjection;
using NewsflowApi.Infrastructure.Identity;
using NewsflowApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNewsflowDatabase(builder.Configuration);
builder.Services.AddNewsflowDataProtection();
builder.Services.AddNewsflowIdentity(builder.Configuration);
builder.Services.AddNewsflowApplication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddNewsflowDevelopment(builder.Configuration);
}

var app = builder.Build();

await app.SeedDataAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();