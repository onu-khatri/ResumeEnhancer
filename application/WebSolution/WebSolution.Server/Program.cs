using ResumeEnhancer.Infrastructure.Caching;
using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.WebSolution.ModulesComposition;
using ResumeEnhancer.Infrastructure.Persistence;
using Scalar.AspNetCore;
using EmptyProjectTesting.Middleware;
using ResumeEnhancer.Core.CommonLibrary.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationCaching(builder.Configuration);

builder.Services.AddAppDbContext((_, options) =>
{
    options.UseSqlServer(GetConnectionString(builder));
});
builder.Services.AddApplicationModules();
builder.Host.ConfigureSerilog(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();
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

app.UseAuthorization();

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

