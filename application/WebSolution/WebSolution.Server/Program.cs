using Microsoft.EntityFrameworkCore;
using Persistence;
using ResumeEnhancer.Infrastructure.Migrations;
using ResumeModulePL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddResumeModulePersistence();
builder.Services.AddAppDbContext((_, options) =>
{
    options.UseSqlServer(GetConnectionString(builder), sqlServerOptions =>
    {
        sqlServerOptions.MigrationsAssembly(MigrationAssembly.AssemblyName);
    });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();

static string GetConnectionString(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        return connectionString;
    }

    if (builder.Environment.IsDevelopment())
    {
        return "Server=(localdb)\\mssqllocaldb;Database=ResumeEnhancerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
    }

    throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");
}
