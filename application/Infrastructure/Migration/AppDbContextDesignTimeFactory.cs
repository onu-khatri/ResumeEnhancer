using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Persistence;
using ResumeModulePL;

namespace ResumeEnhancer.Infrastructure.Migrations;

public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string DefaultConnectionString =
        "Server=(localdb)\\mssqllocaldb;Database=ResumeEnhancerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

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

        return new AppDbContext(options, [new ResumeModuleDbContextModelConfiguration()]);
    }

    private static string? GetConnectionStringFromArgs(string[] args)
    {
        for (var index = 0; index < args.Length - 1; index++)
        {
            if (args[index] is "--connection" or "-c")
            {
                return args[index + 1];
            }
        }

        return null;
    }
}
