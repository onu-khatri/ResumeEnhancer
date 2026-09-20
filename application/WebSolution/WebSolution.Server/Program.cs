using ResumeEnhancer.Infrastructure.Caching;
using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.WebSolution.ModulesComposition;
using ResumeEnhancer.Infrastructure.Persistence;
using Scalar.AspNetCore;
using EmptyProjectTesting.Middleware;
using ResumeEnhancer.Core.CommonLibrary.Extensions;
using ResumeEnhancer.Core.CommonLibrary.Resilience;
using ResumeEnhancer.AuthModule.SL.Services;
using ResumeEnhancer.AuthModule.SL.Options;
using ResumeEnhancer.Core.WebLibrary.Authorization;
using ResumeEnhancer.WebSolution.ModulesComposition.Authorization;

var builder = WebApplication.CreateBuilder(args);

var cacheOptions = builder.Configuration.GetSection(CacheOptions.SectionName).Get<CacheOptions>() ?? new CacheOptions();
CacheOptionsValidator.Validate(cacheOptions, builder.Environment.IsProduction());

// Add services to the container.

builder.Services.AddCoreResilience(builder.Configuration);
builder.Services.AddApplicationCaching(builder.Configuration);

var trustedOrigins = builder.Configuration
    .GetSection($"{AuthSecurityOptions.SectionName}:TrustedOrigins")
    .Get<string[]>() ?? [];
AuthSecurityOptionsValidator.Validate(new AuthSecurityOptions { TrustedOrigins = trustedOrigins });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AuthTrustedOrigins", policy =>
    {
        policy.WithOrigins(trustedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAppDbContext((_, options) =>
{
    options.UseSqlServer(GetConnectionString(builder));
});
builder.Services.AddApplicationModules(builder.Configuration);
builder.Host.ConfigureSerilog(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.AddPolicy("AdminPlanManagement", policy =>
        policy.RequireAuthenticatedUser());
});

var app = builder.Build();

if (!app.Environment.IsEnvironment("IntegrationTest"))
{
    await using var startupScope = app.Services.CreateAsyncScope();
    await startupScope.ServiceProvider.GetRequiredService<AuthSigningKeyStartupValidator>()
        .ValidateAsync();
}

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

app.UseCors("AuthTrustedOrigins");
app.UseAuthentication();
app.UseMiddleware<EndpointAuthorizationMiddleware>();
app.UseAuthorization();

app.MapApplicationModuleApis();
app.Services.GetRequiredService<EndpointAuthorizationStartupValidator>()
    .Validate(app.Services.GetRequiredService<IEnumerable<Microsoft.AspNetCore.Routing.EndpointDataSource>>());
app.Services.GetRequiredService<AuthorizationDependencyStartupValidator>().Validate();

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

