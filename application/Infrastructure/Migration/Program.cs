using System.Diagnostics;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence;
using ResumeEnhancer.Infrastructure.Migrations;
using ResumeModulePL;

return await MigrationConsole.RunAsync(args);

internal static class MigrationConsole
{
    private const string DefaultConnectionString =
        "Data Source=localhost;Integrated Security=True;Persist Security Info=False;Server=TLG-PF5R29H7;Encrypt=True;TrustServerCertificate=True;Initial Catalog=ResumeEnhancer";
    private static readonly HashSet<string> BranchesThatRequireExplicitMigrationName =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "main",
            "dev",
            "test"
        };
    private static readonly object ConsoleSync = new();

    public static async Task<int> RunAsync(string[] args)
    {
        var options = MigrationCommandLine.Parse(args);

        if (options.ShowHelp || !options.HasActions)
        {
            WriteInfo(MigrationCommandLine.Usage);
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
            WriteStep("Migration command started.");
            WriteInfo($"Actions: {options.DescribeActions()}");

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
                    await SeedDataAsync(serviceProvider, cancellationTokenSource.Token);
                }
            }

            WriteStep("Migration command completed.");
            return 0;
        }
        catch (OperationCanceledException)
        {
            WriteWarning("Migration command was cancelled.");
            return 130;
        }
        catch (Exception exception)
        {
            WriteError("Migration command failed with an unhandled exception.");
            WriteError(exception.ToString());
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
            options.EnableDetailedErrors();
            options.LogTo(WriteEfLog, LogLevel.Debug);
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
        var databaseProvider = dbContext.Database.ProviderName ?? "unknown provider";
        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

        WriteStep("Checking pending EF Core migrations.");
        WriteInfo($"Database provider: {databaseProvider}");

        if (pendingMigrations.Count == 0)
        {
            WriteInfo("Pending migrations: none.");
        }
        else
        {
            WriteInfo($"Pending migrations ({pendingMigrations.Count}):");

            foreach (var migration in pendingMigrations)
            {
                WriteInfo($"  - {migration}");
            }
        }

        WriteStep("Applying pending EF Core migrations.");
        await dbContext.Database.MigrateAsync(cancellationToken);
        WriteSuccess("Pending migrations have been applied.");
    }

    private static async Task SeedDataAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeders = scope.ServiceProvider
            .GetServices<IAppDbContextSeeder>()
            .ToList();

        WriteStep("Checking registered seeders.");

        if (seeders.Count == 0)
        {
            WriteWarning("Registered seeders: none.");
            return;
        }

        WriteInfo($"Registered seeders ({seeders.Count}):");

        foreach (var seeder in seeders)
        {
            WriteInfo($"  - {seeder.GetType().FullName}");
        }

        foreach (var seeder in seeders)
        {
            WriteStep($"Running seeder {seeder.GetType().FullName}.");
            await seeder.SeedAsync(dbContext, cancellationToken);
            WriteSuccess($"Seeder completed: {seeder.GetType().FullName}");
        }

        WriteSuccess("Seed data has been applied.");
    }

    private static async Task CreateMigrationAsync(
        MigrationCommandLine options,
        CancellationToken cancellationToken)
    {
        var projectPath = FindMigrationProjectPath();
        var projectDirectory = Path.GetDirectoryName(projectPath)
            ?? throw new InvalidOperationException("Unable to resolve the migration project directory.");
        var migrationName = ResolveMigrationName(options.MigrationName, projectDirectory);

        WriteStep("Creating EF Core migration.");
        WriteInfo($"Migration project: {projectPath}");
        WriteInfo($"Migration name: {migrationName}");

        var processStartInfo = new ProcessStartInfo("dotnet")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = projectDirectory
        };

        processStartInfo.ArgumentList.Add("ef");
        processStartInfo.ArgumentList.Add("--verbose");
        processStartInfo.ArgumentList.Add("migrations");
        processStartInfo.ArgumentList.Add("add");
        processStartInfo.ArgumentList.Add(migrationName);
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

        WriteStep("Launching EF Core CLI.");
        WriteDebug($"  {FormatCommand(processStartInfo)}");

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
            throw new InvalidOperationException(
                $"Migration creation failed with exit code {process.ExitCode}. Command: {FormatCommand(processStartInfo)}");
        }

        WriteSuccess("Migration files have been created.");
    }

    private static string ResolveMigrationName(string? migrationName, string projectDirectory)
    {
        var requestedName = string.IsNullOrWhiteSpace(migrationName)
            ? GetMigrationNameFromCurrentBranch(projectDirectory)
            : migrationName.Trim();
        var normalizedName = NormalizeMigrationName(requestedName);

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new InvalidOperationException(
                "Unable to resolve a valid migration name. Provide one with -c <name> or -n <name>.");
        }

        return EnsureUniqueMigrationName(normalizedName, projectDirectory);
    }

    private static string GetMigrationNameFromCurrentBranch(string projectDirectory)
    {
        var branchName = GetCurrentBranchName(projectDirectory);

        if (string.IsNullOrWhiteSpace(branchName))
        {
            throw new InvalidOperationException(
                "A migration name is required because the current Git branch could not be resolved.");
        }

        if (BranchesThatRequireExplicitMigrationName.Contains(branchName))
        {
            throw new InvalidOperationException(
                $"A migration name is required on the '{branchName}' branch. Example: dotnet run --project <path-to>/Infrastructure/Migration/Migration.csproj -- -c AddResumeFields");
        }

        return branchName;
    }

    private static string? GetCurrentBranchName(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);

        while (directory is not null)
        {
            var gitPath = Path.Combine(directory.FullName, ".git");

            if (Directory.Exists(gitPath))
            {
                return ReadBranchNameFromGitDirectory(gitPath);
            }

            if (File.Exists(gitPath))
            {
                var gitFile = File.ReadAllText(gitPath).Trim();

                if (gitFile.StartsWith("gitdir:", StringComparison.OrdinalIgnoreCase))
                {
                    var gitDirectoryPath = gitFile["gitdir:".Length..].Trim();
                    var resolvedGitDirectoryPath = Path.IsPathRooted(gitDirectoryPath)
                        ? gitDirectoryPath
                        : Path.GetFullPath(Path.Combine(directory.FullName, gitDirectoryPath));

                    return ReadBranchNameFromGitDirectory(resolvedGitDirectoryPath);
                }
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static string? ReadBranchNameFromGitDirectory(string gitDirectory)
    {
        var headPath = Path.Combine(gitDirectory, "HEAD");

        if (!File.Exists(headPath))
        {
            return null;
        }

        var head = File.ReadAllText(headPath).Trim();
        const string headPrefix = "ref: refs/heads/";

        return head.StartsWith(headPrefix, StringComparison.Ordinal)
            ? head[headPrefix.Length..]
            : null;
    }

    private static string NormalizeMigrationName(string migrationName)
    {
        var normalizedName = new StringBuilder();
        var capitalizeNext = true;

        foreach (var character in migrationName)
        {
            if (!char.IsLetterOrDigit(character))
            {
                capitalizeNext = true;
                continue;
            }

            normalizedName.Append(capitalizeNext
                ? char.ToUpperInvariant(character)
                : character);
            capitalizeNext = false;
        }

        if (normalizedName.Length > 0 && !IsValidIdentifierFirstCharacter(normalizedName[0]))
        {
            normalizedName.Insert(0, "Migration");
        }

        return normalizedName.ToString();
    }

    private static bool IsValidIdentifierFirstCharacter(char character) =>
        char.IsLetter(character) || character == '_';

    private static string EnsureUniqueMigrationName(string migrationName, string projectDirectory)
    {
        var migrationsDirectory = Path.Combine(projectDirectory, "Migrations");
        var existingMigrationNames = GetExistingMigrationNames(migrationsDirectory);

        if (!existingMigrationNames.Contains(migrationName))
        {
            return migrationName;
        }

        for (var suffix = 2; ; suffix++)
        {
            var candidateName = $"{migrationName}_{suffix}";

            if (!existingMigrationNames.Contains(candidateName))
            {
                return candidateName;
            }
        }
    }

    private static HashSet<string> GetExistingMigrationNames(string migrationsDirectory)
    {
        var migrationNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!Directory.Exists(migrationsDirectory))
        {
            return migrationNames;
        }

        foreach (var migrationFile in Directory.EnumerateFiles(migrationsDirectory, "*.cs"))
        {
            var fileName = Path.GetFileNameWithoutExtension(migrationFile);

            if (fileName.Equals("AppDbContextModelSnapshot", StringComparison.Ordinal)
                || fileName.EndsWith(".Designer", StringComparison.Ordinal))
            {
                continue;
            }

            var separatorIndex = fileName.IndexOf('_');

            if (separatorIndex >= 0 && separatorIndex < fileName.Length - 1)
            {
                migrationNames.Add(fileName[(separatorIndex + 1)..]);
            }
        }

        return migrationNames;
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
            WriteMessage(ClassifyLogMessage(value, writer == Console.Error), value, writer);
        }
    }

    private static void WriteEfLog(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            var trimmedMessage = message.TrimEnd();
            WriteMessage(ClassifyLogMessage(trimmedMessage, isErrorStream: false), trimmedMessage);
        }
    }

    private static void WriteStep(string message) =>
        WriteMessage(ConsoleMessageSeverity.Step, $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}] {message}");

    private static void WriteDebug(string message) =>
        WriteMessage(ConsoleMessageSeverity.Debug, message);

    private static void WriteInfo(string message) =>
        WriteMessage(ConsoleMessageSeverity.Info, message);

    private static void WriteSuccess(string message) =>
        WriteMessage(ConsoleMessageSeverity.Success, message);

    private static void WriteWarning(string message) =>
        WriteMessage(ConsoleMessageSeverity.Warning, message, Console.Error);

    private static void WriteError(string message) =>
        WriteMessage(ConsoleMessageSeverity.Error, message, Console.Error);

    private static void WriteMessage(
        ConsoleMessageSeverity severity,
        string message,
        TextWriter? writer = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        writer ??= severity is ConsoleMessageSeverity.Warning or ConsoleMessageSeverity.Error
            ? Console.Error
            : Console.Out;

        lock (ConsoleSync)
        {
            var useColor =
                (writer == Console.Out && !Console.IsOutputRedirected)
                || (writer == Console.Error && !Console.IsErrorRedirected);
            var originalColor = Console.ForegroundColor;

            try
            {
                if (useColor)
                {
                    Console.ForegroundColor = GetColor(severity);
                }

                writer.WriteLine(message);
            }
            finally
            {
                if (useColor)
                {
                    Console.ForegroundColor = originalColor;
                }
            }
        }
    }

    private static ConsoleColor GetColor(ConsoleMessageSeverity severity) =>
        severity switch
        {
            ConsoleMessageSeverity.Debug => ConsoleColor.DarkGray,
            ConsoleMessageSeverity.Info => ConsoleColor.Gray,
            ConsoleMessageSeverity.Step => ConsoleColor.Cyan,
            ConsoleMessageSeverity.Success => ConsoleColor.Green,
            ConsoleMessageSeverity.Warning => ConsoleColor.Yellow,
            ConsoleMessageSeverity.Error => ConsoleColor.Red,
            _ => Console.ForegroundColor
        };

    private static ConsoleMessageSeverity ClassifyLogMessage(string message, bool isErrorStream)
    {
        var normalizedMessage = message.TrimStart();

        if (StartsWithAny(normalizedMessage, "fail:", "fatal:", "crit:", "critical:", "error:")
            || ContainsAny(normalizedMessage, "Unhandled exception", "System.Exception", "SqlException", "ERROR "))
        {
            return ConsoleMessageSeverity.Error;
        }

        if (StartsWithAny(normalizedMessage, "warn:", "warning:")
            || ContainsAny(normalizedMessage, " warning ", "Warning:", "NU190"))
        {
            return ConsoleMessageSeverity.Warning;
        }

        if (StartsWithAny(normalizedMessage, "dbug:", "debug:", "trce:", "trace:"))
        {
            return ConsoleMessageSeverity.Debug;
        }

        if (StartsWithAny(normalizedMessage, "info:", "information:"))
        {
            return ConsoleMessageSeverity.Info;
        }

        return isErrorStream
            ? ConsoleMessageSeverity.Error
            : ConsoleMessageSeverity.Info;
    }

    private static bool StartsWithAny(string value, params string[] prefixes) =>
        prefixes.Any(prefix => value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

    private static bool ContainsAny(string value, params string[] fragments) =>
        fragments.Any(fragment => value.Contains(fragment, StringComparison.OrdinalIgnoreCase));

    private static string FormatCommand(ProcessStartInfo processStartInfo)
    {
        var command = new StringBuilder(processStartInfo.FileName);
        var redactNextArgument = false;

        foreach (var argument in processStartInfo.ArgumentList)
        {
            command.Append(' ');
            command.Append(redactNextArgument
                ? QuoteArgument("<connection-string>")
                : QuoteArgument(argument));
            redactNextArgument = argument.Equals("--connection", StringComparison.OrdinalIgnoreCase)
                || argument.Equals("--connection-string", StringComparison.OrdinalIgnoreCase);
        }

        return command.ToString();
    }

    private static string QuoteArgument(string argument) =>
        string.IsNullOrEmpty(argument) || argument.Any(char.IsWhiteSpace) || argument.Contains('"')
            ? $"\"{argument.Replace("\"", "\\\"", StringComparison.Ordinal)}\""
            : argument;
}

internal enum ConsoleMessageSeverity
{
    Debug,
    Info,
    Step,
    Success,
    Warning,
    Error
}

internal sealed class MigrationCommandLine
{
    public const string Usage = """
        Migration commands:
          -c, --create [name]       Create a new EF migration. Uses the current branch name when name is omitted on non-main/dev/test branches.
          -a, --apply               Apply pending migrations to the database.
          -s, --seeding             Run registered seeders.

        Options:
          --connection <string>     Override the database connection string.
          --connection-string <s>   Override the database connection string.
          -n, --name <name>         Migration name used with -c.
          -h, --help                Show this help.

        Examples:
          dotnet run --project <path-to>/Infrastructure/Migration/Migration.csproj -- -c AddResumeFields
          dotnet run --project <path-to>/Infrastructure/Migration/Migration.csproj -- -c
          dotnet run --project <path-to>/Infrastructure/Migration/Migration.csproj -- -a -s
        """;

    public bool ShowHelp { get; private set; }

    public bool CreateMigration { get; private set; }

    public bool ApplyMigrations { get; private set; }

    public bool SeedData { get; private set; }

    public string? MigrationName { get; private set; }

    public string? ConnectionString { get; private set; }

    public bool HasActions => CreateMigration || ApplyMigrations || SeedData;

    public string DescribeActions()
    {
        var actions = new List<string>();

        if (CreateMigration)
        {
            actions.Add("create migration");
        }

        if (ApplyMigrations)
        {
            actions.Add("apply migrations");
        }

        if (SeedData)
        {
            actions.Add("seed data");
        }

        return actions.Count == 0
            ? "none"
            : string.Join(", ", actions);
    }

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
