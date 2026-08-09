# Migration Project

This project owns Entity Framework Core migrations for the application.

The web application should not run database migrations every time it starts. Startup should stay focused on serving the application. Database schema changes and seed data are handled explicitly by this console project.

## What This Project Does

- Creates new EF Core migration files.
- Applies pending migrations to the database.
- Runs registered seeders.
- Keeps migration code in `Infrastructure/Migration/Migrations`.
- Uses the same `AppDbContext` and module model configuration as the application.
- Uses Persistence project conventions for table names, schemas, domain category prefixes, rowversion columns, and module mappings.

## Important Files

| File | Purpose |
| --- | --- |
| `Migration.csproj` | Console project for migration commands. |
| `Program.cs` | Command-line entry point for create, apply, and seed operations. |
| `AppDbContextDesignTimeFactory.cs` | Creates `AppDbContext` for EF Core design-time tooling. |
| `MigrationAssembly.cs` | Exposes this assembly name so EF stores migrations here. |
| `Migrations/` | Folder where EF migration classes and snapshots are stored. |

## How This Project Fits With Persistence

The Migration project does not define entities or entity rules directly. It uses the Persistence project and each module persistence layer to build the EF Core model.

The flow is:

```text
Migration console
  -> registers module persistence services
  -> registers AppDbContext
  -> AppDbContext runs IAppDbContextModelConfiguration implementations
  -> EF Core compares the model with the migration snapshot
  -> EF Core creates or applies migrations
```

For example, the Resume module registers `ResumeModuleDbContextModelConfiguration`. That configuration loads all Resume module `IEntityTypeConfiguration<T>` classes and then applies the shared table/schema mapping convention.

## Table And Schema Naming Rules

Do not hardcode table names or schema names in individual `IEntityTypeConfiguration<T>` classes.

Each module should define its schema in one common place. For Resume module, that is:

```csharp
public static class ResumeModuleDatabase
{
    public const string Schema = "resume";
}
```

Table names are decided by the Persistence helper `ApplyModuleTableMappings`:

1. If the entity has `[Table("CustomTableName")]`, EF uses `CustomTableName`.
2. If the entity does not have a `[Table]` name, EF uses the entity class name.
3. If the entity inherits a domain category base type, EF prefixes the table name.
4. Schema comes from the module schema, not from each entity configuration.

| Base Type | Prefix | Example |
| --- | --- | --- |
| `SetupEntity` | `S_` | `S_ResumeSectionSetup` |
| `SetupRelation` | `SR_` | `SR_*` |
| `BusinessEntity` | `B_` | `B_Resume` |
| `BusinessRelation` | `BR_` | `BR_WorkExperience` |

Example entity with default table name:

```csharp
using DomainLibrary.DomainModel;

public class Resume : BusinessEntity
{
    public int Id { get; set; }
}
```

This maps to:

```text
schema: resume
table: B_Resume
```

Example entity with explicit table name:

```csharp
using System.ComponentModel.DataAnnotations.Schema;

[Table("CandidateResume")]
public class Resume
{
    public int Id { get; set; }
}
```

This maps to:

```text
schema: resume
table: B_CandidateResume
```

Avoid this in entity configurations:

```csharp
builder.ToTable("Resumes", "resume");
```

The table/schema mapping belongs in the module-level model configuration, not in every entity configuration.

## Integrated Module Schema Rules

Most modules use their own schema:

```text
resume
```

If one module is used as a supporting module for a root entity module, the schema should be:

```text
rootEntitySchema_supportingModuleSchema
```

Example:

```csharp
services.AddResumeModulePersistence(rootEntitySchema: "profile");
```

The Resume module schema becomes:

```text
profile_resume
```

Use this pattern when two modules integrate and one module's data belongs under another module's root boundary.

## Command Format

Run commands from the repository root:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- <migration-options>
```

The `--` is important. Everything before `--` is for `dotnet run`; everything after `--` is passed to the migration console.

## Available Commands

| Option | Meaning | Example |
| --- | --- | --- |
| `-c`, `--create` | Create a new EF migration. Name is optional on feature branches. | `-c AddResumeFields` |
| `-a`, `--apply` | Apply pending migrations to the configured database. | `-a` |
| `-s`, `--seeding` | Run registered seeders. | `-s` |
| `--connection` | Override the database connection string. | `--connection "Server=..."` |
| `--connection-string` | Same as `--connection`. | `--connection-string "Server=..."` |
| `-h`, `--help` | Show command help. | `--help` |

Commands can be combined. When combined, the console runs them in this order:

1. Create migration.
2. Apply migrations.
3. Run seeders.

The migration console is verbose by default. It prints the active action, EF CLI command, pending migrations, registered seeders, EF Core diagnostic logs, warnings, and full exception stack traces when a command fails. Messages are color-coded by severity: debug is dark gray, information is gray, progress steps are cyan, successful operations are green, warnings are yellow, and errors are red.

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

Create a migration using the current branch name:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -c
```

This only works when the current Git branch is not `main`, `dev`, or `test`. On those shared branches, pass an explicit migration name.

When the requested or inferred migration name already exists, the console appends an incremental suffix:

```text
AddResumeFields
AddResumeFields_2
AddResumeFields_3
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
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -a -s --connection "Data Source=localhost;Integrated Security=True;Persist Security Info=False;Server=TLG-PF5R29H7;Encrypt=True;TrustServerCertificate=True;Initial Catalog=ResumeEnhancer"
```

## Connection String Resolution

The migration console chooses the connection string in this order:

1. Command-line option: `--connection` or `--connection-string`.
2. Environment variable: `RESUME_ENHANCER_CONNECTION_STRING`.
3. Default local development connection string:

```text
Data Source=localhost;Integrated Security=True;Persist Security Info=False;Server=TLG-PF5R29H7;Encrypt=True;TrustServerCertificate=True;Initial Catalog=ResumeEnhancer
```

Example using the environment variable in PowerShell:

```powershell
$env:RESUME_ENHANCER_CONNECTION_STRING = "Data Source=localhost;Integrated Security=True;Persist Security Info=False;Server=TLG-PF5R29H7;Encrypt=True;TrustServerCertificate=True;Initial Catalog=ResumeEnhancer"
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -a -s
```

## How Creating A Migration Works

When you run `-c`, the console internally runs:

```powershell
dotnet ef --verbose migrations add <MigrationName> --project <MigrationProject> --startup-project <MigrationProject> --context Persistence.AppDbContext --output-dir Migrations
```

This means:

- Migration files are created inside this project.
- The migration assembly stays separate from the web application.
- The web application does not need to reference the migration project.
- If no migration name is provided, the console tries to use the current Git branch name unless the branch is `main`, `dev`, or `test`.
- Branch names are normalized into valid migration names before `dotnet ef` is called.
- Duplicate migration names are resolved by appending `_2`, `_3`, and so on.

Make sure `dotnet ef` is available:

```powershell
dotnet ef --version
```

If it is not installed, install it:

```powershell
dotnet tool install --global dotnet-ef
```

Use an EF tool version compatible with the EF packages used by the solution.

## When To Create A Migration

Create a migration after any change that affects the EF model, including:

- Adding, removing, or renaming an entity.
- Adding, removing, or renaming a property.
- Changing relationships, indexes, max lengths, precision, or required fields.
- Adding or changing `[Table("...")]` on an entity.
- Changing a module schema.
- Registering a new module persistence configuration.

If a change does not affect the EF model, a migration is not needed.

Examples of changes that usually do not need a migration:

- Changing application service code.
- Changing controller code.
- Changing cache logic.
- Changing README files.

After creating a migration, always review the generated file before applying it.

Look for destructive operations:

```csharp
migrationBuilder.DropTable(...);
migrationBuilder.DropColumn(...);
```

Sometimes these operations are correct, but a junior developer should pause and ask for review before applying them to shared or production databases.

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

Before applying, the console prints the database provider and the pending migration list. EF Core logs are written to the console with detailed errors enabled.

## How Seeding Works

When you run `-s`, the console calls:

```csharp
await serviceProvider.SeedAppDbContextAsync(cancellationToken);
```

That extension method finds all registered `IAppDbContextSeeder` implementations and runs them.

The console prints every registered seeder and reports each seeder before and after it runs.

Current module seeders are registered through:

```csharp
services.AddResumeModulePersistence();
```

The resume module registers `ResumeModuleSeeder`, which inserts, updates, or obsoletes default resume section setup data by stable `Guid` and `Code`.

## Adding A New Module To Migrations

When a new module has entities that must be included in `AppDbContext`, follow this pattern:

1. Create entity configurations in the module persistence layer.
2. Implement `IAppDbContextModelConfiguration`.
3. Define the module schema in one common class.
4. Apply module table mappings from the model configuration.
5. Register that implementation in the module dependency injection method.
6. Reference the module persistence project from `Migration.csproj`.
7. Add the module registration in `Program.cs` inside `CreateServiceProvider`.
8. Create a new migration with `-c`.
9. Apply it with `-a`.

Example module registration:

```csharp
services.AddResumeModulePersistence();
services.AddAnotherModulePersistence();
```

Example model configuration:

```csharp
using Microsoft.EntityFrameworkCore;
using Persistence;
using MyModuleDM.Entities;

public sealed class MyModuleDbContextModelConfiguration : IAppDbContextModelConfiguration
{
    private readonly string _schema;

    public MyModuleDbContextModelConfiguration(string? rootEntitySchema = null)
    {
        _schema = string.IsNullOrWhiteSpace(rootEntitySchema)
            ? ModuleSchemaName.FromModule(MyModuleDatabase.Schema)
            : ModuleSchemaName.FromRootAndSupportingModule(rootEntitySchema, MyModuleDatabase.Schema);
    }

    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyModuleDbContextModelConfiguration).Assembly);
        modelBuilder.ApplyModuleTableMappings(typeof(MyEntity).Assembly, _schema);
    }
}
```

Example module schema:

```csharp
public static class MyModuleDatabase
{
    public const string Schema = "my_module";
}
```

Example entity configuration without hardcoded table/schema:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class MyEntityConfiguration : IEntityTypeConfiguration<MyEntity>
{
    public void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Name).HasMaxLength(200).IsRequired();
    }
}
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

Seeders should be safe to run multiple times. Prefer insert-or-update logic instead of blindly adding duplicate data. For setup data, use the shared setup seeding helper instead of EF `HasData`.

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

## Current Initial Migration

The migration `InitialSchema` is the clean initial baseline for the current model.

Resume module entities inherit `SetupEntity`, `BusinessEntity`, or `BusinessRelation`, so table names include category prefixes from the beginning:

| Entity | Initial table |
| --- | --- |
| `Resume` | `B_Resume` |
| `Address` | `BR_Address` |
| `Award` | `BR_Award` |
| `Certification` | `BR_Certification` |
| `Education` | `BR_Education` |
| `Hobby` | `BR_Hobby` |
| `Language` | `BR_Language` |
| `PersonalInformation` | `BR_PersonalInformation` |
| `Project` | `BR_Project` |
| `Skill` | `BR_Skill` |
| `SocialMediaLink` | `BR_SocialMediaLink` |
| `WorkExperience` | `BR_WorkExperience` |
| `ResumeSectionSetup` | `S_ResumeSectionSetup` |

The initial migration also creates setup columns (`Code`, `Description`, `Guid`, `ObsoleteFlag`) and rowversion-backed `App_Version` columns.

## Troubleshooting

If `-c` fails with `dotnet ef` not found, install the EF Core CLI tool:

```powershell
dotnet tool install --global dotnet-ef
```

If the database connection fails, confirm the connection string and SQL Server availability.

If seed data is not inserted, confirm the seeder is registered as `IAppDbContextSeeder` and that the migration console registers that module persistence service.

If migrations do not include your new entity, confirm the module has an `IAppDbContextModelConfiguration` implementation and that it is registered before `AppDbContext` is built.

If a table name is not what you expected, check whether the entity has a `[Table("...")]` attribute. If it does not, the table name will be the entity class name.

If a table is created in the wrong schema, check the module schema class and the module persistence registration. For integrated modules, confirm the root schema value passed to the module registration.
