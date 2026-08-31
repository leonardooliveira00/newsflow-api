using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using NewsflowApi.Data;
using NewsflowApi.Extensions.DependencyInjection;
using NewsflowApi.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNewsflowDatabase(builder.Configuration);
builder.Services.AddNewsflowDataProtection();
builder.Services.AddNewsflowIdentity(builder.Configuration);
builder.Services.AddNewsflowApplication();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

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