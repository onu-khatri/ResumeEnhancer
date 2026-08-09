using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Persistence;
using ResumeModulePL;

namespace ResumeEnhancer.Infrastructure.Migrations;

public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string DefaultConnectionString =
        "Data Source=localhost;Integrated Security=True;Persist Security Info=False;Server=TLG-PF5R29H7;Encrypt=True;TrustServerCertificate=True;Initial Catalog=ResumeEnhancer";

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
            if (args[index] is "--connection" or "--connection-string")
            {
                return args[index + 1];
            }
        }

        return null;
    }
}
