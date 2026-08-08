# Migration Project

This project owns Entity Framework Core migrations for the application.

The web application should not run database migrations every time it starts. Startup should stay focused on serving the application. Database schema changes and seed data are handled explicitly by this console project.

## What This Project Does

- Creates new EF Core migration files.
- Applies pending migrations to the database.
- Runs registered seeders.
- Keeps migration code in `Infrastructure/Migration/Migrations`.
- Uses the same `AppDbContext` and module model configuration as the application.

## Important Files

| File | Purpose |
| --- | --- |
| `Migration.csproj` | Console project for migration commands. |
| `Program.cs` | Command-line entry point for create, apply, and seed operations. |
| `AppDbContextDesignTimeFactory.cs` | Creates `AppDbContext` for EF Core design-time tooling. |
| `MigrationAssembly.cs` | Exposes this assembly name so EF stores migrations here. |
| `Migrations/` | Folder where EF migration classes and snapshots are stored. |

## Command Format

Run commands from the repository root:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- <migration-options>
```

The `--` is important. Everything before `--` is for `dotnet run`; everything after `--` is passed to the migration console.

## Available Commands

| Option | Meaning | Example |
| --- | --- | --- |
| `-c`, `--create` | Create a new EF migration. Requires a migration name. | `-c AddResumeFields` |
| `-a`, `--apply` | Apply pending migrations to the configured database. | `-a` |
| `-s`, `--seeding` | Run registered seeders. | `-s` |
| `--connection` | Override the database connection string. | `--connection "Server=..."` |
| `--connection-string` | Same as `--connection`. | `--connection-string "Server=..."` |
| `-h`, `--help` | Show command help. | `--help` |

Commands can be combined. When combined, the console runs them in this order:

1. Create migration.
2. Apply migrations.
3. Run seeders.

## Common Usage

Show help:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- --help
```

Create a migration:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -c AddResumeFields
```

You can also pass the migration name with `-n`:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -c -n AddResumeFields
```

Apply pending migrations:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -a
```

Run seed data only:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -s
```

Apply migrations and then seed data:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -a -s
```

Create, apply, and seed in one command:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -c AddResumeFields -a -s
```

Use a custom connection string:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -a -s --connection "Server=(localdb)\mssqllocaldb;Database=ResumeEnhancerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

## Connection String Resolution

The migration console chooses the connection string in this order:

1. Command-line option: `--connection` or `--connection-string`.
2. Environment variable: `RESUME_ENHANCER_CONNECTION_STRING`.
3. Default local development connection string:

```text
Server=(localdb)\mssqllocaldb;Database=ResumeEnhancerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

Example using the environment variable in PowerShell:

```powershell
$env:RESUME_ENHANCER_CONNECTION_STRING = "Server=.;Database=ResumeEnhancerDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -a -s
```

## How Creating A Migration Works

When you run `-c`, the console internally runs:

```powershell
dotnet ef migrations add <MigrationName> --project <MigrationProject> --startup-project <MigrationProject> --context Persistence.AppDbContext --output-dir Migrations
```

This means:

- Migration files are created inside this project.
- The migration assembly stays separate from the web application.
- The web application does not need to reference the migration project.

Make sure `dotnet ef` is available:

```powershell
dotnet ef --version
```

If it is not installed, install it:

```powershell
dotnet tool install --global dotnet-ef
```

Use an EF tool version compatible with the EF packages used by the solution.

## How Applying Migrations Works

When you run `-a`, the console:

1. Creates a service collection.
2. Registers module persistence services.
3. Registers `AppDbContext`.
4. Configures SQL Server with this project as the migration assembly.
5. Runs:

```csharp
await dbContext.Database.MigrateAsync(cancellationToken);
```

`MigrateAsync` applies all pending migrations. If the database does not exist, EF Core creates it.

## How Seeding Works

When you run `-s`, the console calls:

```csharp
await serviceProvider.SeedAppDbContextAsync(cancellationToken);
```

That extension method finds all registered `IAppDbContextSeeder` implementations and runs them.

Current module seeders are registered through:

```csharp
services.AddResumeModulePersistence();
```

The resume module registers `ResumeModuleSeeder`, which inserts or updates default resume section setup data.

## Adding A New Module To Migrations

When a new module has entities that must be included in `AppDbContext`, follow this pattern:

1. Create entity configurations in the module persistence layer.
2. Implement `IAppDbContextModelConfiguration`.
3. Register that implementation in the module dependency injection method.
4. Reference the module persistence project from `Migration.csproj`.
5. Add the module registration in `Program.cs` inside `CreateServiceProvider`.
6. Create a new migration with `-c`.
7. Apply it with `-a`.

Example module registration:

```csharp
services.AddResumeModulePersistence();
services.AddAnotherModulePersistence();
```

## Adding Seed Data

Create a seeder by implementing `IAppDbContextSeeder`:

```csharp
using Microsoft.EntityFrameworkCore;
using Persistence;

public sealed class MyModuleSeeder : IAppDbContextSeeder
{
    public async Task SeedAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var set = dbContext.Set<MyEntity>();

        if (!await set.AnyAsync(cancellationToken))
        {
            set.Add(new MyEntity { Name = "Default value" });
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
```

Register it in the module persistence dependency injection:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistence;

services.TryAddEnumerable(
    ServiceDescriptor.Scoped<IAppDbContextSeeder, MyModuleSeeder>());
```

Seeders should be safe to run multiple times. Prefer insert-or-update logic instead of blindly adding duplicate data.

## Recommended Workflow

For normal local development:

1. Change your entity or EF configuration.
2. Create a migration.

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -c AddNewEntity
```

3. Review the generated migration files.
4. Apply the migration.

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -a
```

5. Run seed data if needed.

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -s
```

## Troubleshooting

If `-c` fails with `dotnet ef` not found, install the EF Core CLI tool:

```powershell
dotnet tool install --global dotnet-ef
```

If the database connection fails, confirm the connection string and SQL Server availability.

If seed data is not inserted, confirm the seeder is registered as `IAppDbContextSeeder` and that the migration console registers that module persistence service.

If migrations do not include your new entity, confirm the module has an `IAppDbContextModelConfiguration` implementation and that it is registered before `AppDbContext` is built.
