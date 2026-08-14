using AuthModulePL.Configurations;
using AuthModulePL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Persistence;
using ResumeModulePL;

namespace ResumeEnhancer.Infrastructure.Migrations;

public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string DefaultConnectionString =
        "Server=localhost;Database=ResumeEnhancer;Integrated Security=True;Encrypt=False;";

    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            GetConnectionStringFromArgs(args)
            ?? Environment.GetEnvironmentVariable("RESUME_ENHANCER_CONNECTION_STRING")
            ?? DefaultConnectionString;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly(MigrationAssembly.AssemblyName);
            })
            .Options;

        return new AppDbContext(options, [new ResumeModuleDbContextModelConfiguration(), new AuthModuleDbContextConfigurations()]);
    }

    private static string? GetConnectionStringFromArgs(string[] args)
    {
        for (var index = 0; index < args.Length - 1; index++)
        {
            if (args[index] is "--connection" or "--connection-string")
            {
                return args[index + 1];
            }
        }

        return null;
    }
}
