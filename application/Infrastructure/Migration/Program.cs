using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using ResumeEnhancer.Infrastructure.Migrations;
using ResumeModulePL;

return await MigrationConsole.RunAsync(args);

internal static class MigrationConsole
{
    private const string DefaultConnectionString =
        "Server=(localdb)\\mssqllocaldb;Database=ResumeEnhancerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    public static async Task<int> RunAsync(string[] args)
    {
        var options = MigrationCommandLine.Parse(args);

        if (options.ShowHelp || !options.HasActions)
        {
            Console.WriteLine(MigrationCommandLine.Usage);
            return options.ShowHelp ? 0 : 1;
        }

        using var cancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellationTokenSource.Cancel();
        };

        try
        {
            if (options.CreateMigration)
            {
                await CreateMigrationAsync(options, cancellationTokenSource.Token);
            }

            if (options.ApplyMigrations || options.SeedData)
            {
                using var serviceProvider = CreateServiceProvider(options.ConnectionString);

                if (options.ApplyMigrations)
                {
                    await ApplyMigrationsAsync(serviceProvider, cancellationTokenSource.Token);
                }

                if (options.SeedData)
                {
                    await serviceProvider.SeedAppDbContextAsync(cancellationTokenSource.Token);
                    Console.WriteLine("Seed data has been applied.");
                }
            }

            return 0;
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine("Migration command was cancelled.");
            return 130;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private static ServiceProvider CreateServiceProvider(string? connectionStringOverride)
    {
        var connectionString =
            connectionStringOverride
            ?? Environment.GetEnvironmentVariable("RESUME_ENHANCER_CONNECTION_STRING")
            ?? DefaultConnectionString;

        var services = new ServiceCollection();

        services.AddResumeModulePersistence();
        services.AddAppDbContext((_, options) =>
        {
            options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly(MigrationAssembly.AssemblyName);
            });
        });

        return services.BuildServiceProvider(validateScopes: true);
    }

    private static async Task ApplyMigrationsAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
        Console.WriteLine("Pending migrations have been applied.");
    }

    private static async Task CreateMigrationAsync(
        MigrationCommandLine options,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(options.MigrationName))
        {
            throw new InvalidOperationException("A migration name is required when using -c. Example: dotnet run --project <path-to>/Infrastructure/Migration/Migration.csproj -- -c AddResumeFields");
        }

        var projectPath = FindMigrationProjectPath();
        var projectDirectory = Path.GetDirectoryName(projectPath)
            ?? throw new InvalidOperationException("Unable to resolve the migration project directory.");

        var processStartInfo = new ProcessStartInfo("dotnet")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = projectDirectory
        };

        processStartInfo.ArgumentList.Add("ef");
        processStartInfo.ArgumentList.Add("migrations");
        processStartInfo.ArgumentList.Add("add");
        processStartInfo.ArgumentList.Add(options.MigrationName);
        processStartInfo.ArgumentList.Add("--project");
        processStartInfo.ArgumentList.Add(projectPath);
        processStartInfo.ArgumentList.Add("--startup-project");
        processStartInfo.ArgumentList.Add(projectPath);
        processStartInfo.ArgumentList.Add("--context");
        processStartInfo.ArgumentList.Add(typeof(AppDbContext).FullName!);
        processStartInfo.ArgumentList.Add("--output-dir");
        processStartInfo.ArgumentList.Add("Migrations");

        if (!string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            processStartInfo.ArgumentList.Add("--");
            processStartInfo.ArgumentList.Add("--connection");
            processStartInfo.ArgumentList.Add(options.ConnectionString);
        }

        using var process = new Process { StartInfo = processStartInfo };

        process.OutputDataReceived += (_, eventArgs) => WriteLineIfPresent(Console.Out, eventArgs.Data);
        process.ErrorDataReceived += (_, eventArgs) => WriteLineIfPresent(Console.Error, eventArgs.Data);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (!process.HasExited)
        {
            process.Kill(entireProcessTree: true);
            throw;
        }

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Migration creation failed with exit code {process.ExitCode}.");
        }
    }

    private static string FindMigrationProjectPath()
    {
        var roots = new[]
        {
            Directory.GetCurrentDirectory(),
            AppContext.BaseDirectory
        };

        foreach (var root in roots)
        {
            var directory = new DirectoryInfo(root);

            while (directory is not null)
            {
                var directProject = Path.Combine(directory.FullName, "Migration.csproj");

                if (File.Exists(directProject))
                {
                    return directProject;
                }

                var infrastructureProject = Path.Combine(
                    directory.FullName,
                    "Infrastructure",
                    "Migration",
                    "Migration.csproj");

                if (File.Exists(infrastructureProject))
                {
                    return infrastructureProject;
                }

                var applicationProject = Path.Combine(
                    directory.FullName,
                    "application",
                    "Infrastructure",
                    "Migration",
                    "Migration.csproj");

                if (File.Exists(applicationProject))
                {
                    return applicationProject;
                }

                directory = directory.Parent;
            }
        }

        throw new InvalidOperationException("Unable to locate Infrastructure/Migration/Migration.csproj.");
    }

    private static void WriteLineIfPresent(TextWriter writer, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            writer.WriteLine(value);
        }
    }
}

internal sealed class MigrationCommandLine
{
    public const string Usage = """
        Migration commands:
          -c, --create <name>       Create a new EF migration.
          -a, --apply               Apply pending migrations to the database.
          -s, --seeding             Run registered seeders.

        Options:
          --connection <string>     Override the database connection string.
          --connection-string <s>   Override the database connection string.
          -n, --name <name>         Migration name used with -c.
          -h, --help                Show this help.

        Examples:
          dotnet run --project <path-to>/Infrastructure/Migration/Migration.csproj -- -c AddResumeFields
          dotnet run --project <path-to>/Infrastructure/Migration/Migration.csproj -- -a -s
        """;

    public bool ShowHelp { get; private set; }

    public bool CreateMigration { get; private set; }

    public bool ApplyMigrations { get; private set; }

    public bool SeedData { get; private set; }

    public string? MigrationName { get; private set; }

    public string? ConnectionString { get; private set; }

    public bool HasActions => CreateMigration || ApplyMigrations || SeedData;

    public static MigrationCommandLine Parse(string[] args)
    {
        var options = new MigrationCommandLine();

        for (var index = 0; index < args.Length; index++)
        {
            var argument = args[index];

            switch (argument)
            {
                case "-h":
                case "--help":
                    options.ShowHelp = true;
                    break;

                case "-c":
                case "--create":
                    options.CreateMigration = true;
                    options.MigrationName ??= ReadOptionalValue(args, ref index);
                    break;

                case "-a":
                case "--apply":
                    options.ApplyMigrations = true;
                    break;

                case "-s":
                case "--seed":
                case "--seeding":
                    options.SeedData = true;
                    break;

                case "-n":
                case "--name":
                    options.MigrationName = ReadRequiredValue(args, ref index, argument);
                    break;

                case "--connection":
                case "--connection-string":
                    options.ConnectionString = ReadRequiredValue(args, ref index, argument);
                    break;

                default:
                    if (TryReadInlineValue(argument, "-c:", out var inlineCreateName)
                        || TryReadInlineValue(argument, "--create=", out inlineCreateName))
                    {
                        options.CreateMigration = true;
                        options.MigrationName = inlineCreateName;
                        break;
                    }

                    if (TryReadInlineValue(argument, "-n:", out var inlineMigrationName)
                        || TryReadInlineValue(argument, "--name=", out inlineMigrationName))
                    {
                        options.MigrationName = inlineMigrationName;
                        break;
                    }

                    if (TryReadInlineValue(argument, "--connection=", out var inlineConnection)
                        || TryReadInlineValue(argument, "--connection-string=", out inlineConnection))
                    {
                        options.ConnectionString = inlineConnection;
                        break;
                    }

                    throw new InvalidOperationException($"Unknown argument '{argument}'.");
            }
        }

        return options;
    }

    private static string? ReadOptionalValue(string[] args, ref int index)
    {
        if (index + 1 >= args.Length || IsOption(args[index + 1]))
        {
            return null;
        }

        index++;
        return args[index];
    }

    private static string ReadRequiredValue(string[] args, ref int index, string optionName)
    {
        if (index + 1 >= args.Length || IsOption(args[index + 1]))
        {
            throw new InvalidOperationException($"A value is required for {optionName}.");
        }

        index++;
        return args[index];
    }

    private static bool IsOption(string argument) => argument.StartsWith('-');

    private static bool TryReadInlineValue(string argument, string prefix, out string? value)
    {
        if (argument.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            && argument.Length > prefix.Length)
        {
            value = argument[prefix.Length..];
            return true;
        }

        value = null;
        return false;
    }
}
