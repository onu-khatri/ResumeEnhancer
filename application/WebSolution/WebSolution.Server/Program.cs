using Caching;
using Microsoft.EntityFrameworkCore;
using ModulesComposition;
using Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationCaching(builder.Configuration);
builder.Services.AddAppDbContext((_, options) =>
{
    options.UseSqlServer(GetConnectionString(builder));
});
builder.Services.AddApplicationModules();

builder.Services.AddOpenApi();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Resume Enhancer API");
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapApplicationModuleApis();

app.MapFallbackToFile("/index.html");

app.Run();

static string GetConnectionString(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        return connectionString;
    }

    throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");
}

public partial class Program;
