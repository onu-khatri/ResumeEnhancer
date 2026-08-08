# Persistence Project

This project contains the shared Entity Framework Core infrastructure used by the application and all modules.

The Persistence project should stay generic. It should know how to build `AppDbContext`, run module model configurations, apply table/schema conventions, and execute seeders. It should not contain business entities for a specific module.

## What This Project Does

- Provides the shared `AppDbContext`.
- Allows modules to plug their EF model configuration into `AppDbContext`.
- Provides a common table/schema mapping convention for modules.
- Provides a common seeding contract.
- Provides dependency injection helpers for registering the database context.

## Important Files

| File | Purpose |
| --- | --- |
| `AppDbContext.cs` | Shared EF Core `DbContext` used by the application. |
| `DependencyInjection.cs` | Registers `AppDbContext` in dependency injection. |
| `IAppDbContextModelConfiguration.cs` | Contract that modules implement to configure their entities. |
| `IAppDbContextSeeder.cs` | Contract that modules implement to seed data. |
| `AppDbContextSeederExtensions.cs` | Runs all registered seeders. |
| `ModelBuilderModuleMappingExtensions.cs` | Applies module table/schema conventions. |
| `ModuleSchemaName.cs` | Builds and validates schema names. |

## Big Picture

`AppDbContext` does not directly know about Resume module, User module, or any future module. Instead, modules register implementations of:

```csharp
IAppDbContextModelConfiguration
```

At runtime, `AppDbContext` receives all registered model configurations and runs them in `OnModelCreating`.

```text
AppDbContext
  -> IAppDbContextModelConfiguration from Resume module
  -> IAppDbContextModelConfiguration from another module
  -> EF Core model
```

This keeps the core persistence layer reusable and lets each module own its own mapping rules.

## Register AppDbContext

Use `AddAppDbContext` from the application startup project.

Example from a web application:

```csharp
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppDbContext((_, options) =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});
```

The application should register module persistence services before or around `AddAppDbContext`, so module configurations are available through dependency injection.

Example:

```csharp
builder.Services.AddResumeModulePersistence();

builder.Services.AddAppDbContext((_, options) =>
{
    options.UseSqlServer(connectionString);
});
```

## How AppDbContext Uses Module Configurations

`AppDbContext` accepts an `IEnumerable<IAppDbContextModelConfiguration>`.

```csharp
public AppDbContext(
    DbContextOptions<AppDbContext> options,
    IEnumerable<IAppDbContextModelConfiguration> modelConfigurations)
    : base(options)
{
    _modelConfigurations = modelConfigurations.ToArray();
}
```

Then it runs each configuration:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    foreach (var modelConfiguration in _modelConfigurations)
    {
        modelConfiguration.Configure(modelBuilder);
    }
}
```

This is the main extension point for modules.

## Creating A Module Model Configuration

Each module should create one class that implements `IAppDbContextModelConfiguration`.

Example:

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

The important parts are:

- `ApplyConfigurationsFromAssembly(...)` loads all `IEntityTypeConfiguration<T>` classes from the module.
- `ApplyModuleTableMappings(...)` applies the table name and schema convention to every entity in the module entity assembly.

## Entity Configuration Rules

Entity configurations should describe keys, properties, indexes, and relationships.

They should not hardcode table names or schema names.

Good:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ResumeConfiguration : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.HasKey(resume => resume.Id);

        builder.Property(resume => resume.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(resume => resume.UserId);
    }
}
```

Avoid:

```csharp
public void Configure(EntityTypeBuilder<Resume> builder)
{
    builder.ToTable("Resumes", "resume");
}
```

The shared convention handles table and schema names.

## Table Name Convention

Table names are resolved by `ApplyModuleTableMappings`.

Rule:

1. If the entity has `[Table("CustomTableName")]`, use that name.
2. Otherwise, use the entity class name.

Default example:

```csharp
public class Resume
{
    public int Id { get; set; }
}
```

Maps to:

```text
Resume
```

Custom table example:

```csharp
using System.ComponentModel.DataAnnotations.Schema;

[Table("CandidateResume")]
public class Resume
{
    public int Id { get; set; }
}
```

Maps to:

```text
CandidateResume
```

The schema part of `[Table]` should not be used for module entities. Schema comes from the module schema convention so the whole module stays together.

## Schema Convention

Each module should define its base schema in one common place.

Example:

```csharp
public static class MyModuleDatabase
{
    public const string Schema = "my_module";
}
```

Use `ModuleSchemaName.FromModule` for normal module usage:

```csharp
var schema = ModuleSchemaName.FromModule(MyModuleDatabase.Schema);
```

Result:

```text
my_module
```

For integrated modules, use `FromRootAndSupportingModule`.

```csharp
var schema = ModuleSchemaName.FromRootAndSupportingModule(
    rootEntitySchema: "profile",
    supportingModuleSchema: "resume");
```

Result:

```text
profile_resume
```

Use integrated schemas when one module behaves as the root entity module and another module supports it.

## Schema Name Validation

`ModuleSchemaName` validates schema names before they are used.

Allowed:

```text
resume
profile_resume
_internal
module1
```

Not allowed:

```text
resume-module
profile.resume
1resume
resume module
```

Schema names must:

- Not be empty.
- Start with a letter or underscore.
- Contain only letters, numbers, and underscores.

## Registering A Module Configuration

A module should register its model configuration in its own dependency injection class.

Example:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddMyModulePersistence(this IServiceCollection services)
    {
        return services.AddMyModulePersistence(rootEntitySchema: null);
    }

    public static IServiceCollection AddMyModulePersistence(
        this IServiceCollection services,
        string? rootEntitySchema)
    {
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IAppDbContextModelConfiguration>(
                new MyModuleDbContextModelConfiguration(rootEntitySchema)));

        return services;
    }
}
```

After this is registered, `AppDbContext` automatically receives the module configuration through dependency injection.

## Using AppDbContext In A Module

Modules can add extension methods to make `DbSet<T>` access readable.

Example:

```csharp
using Microsoft.EntityFrameworkCore;
using Persistence;

public static class AppDbContextMyModuleExtensions
{
    public static DbSet<MyEntity> MyEntities(this AppDbContext dbContext) =>
        dbContext.Set<MyEntity>();
}
```

Then application code can use:

```csharp
var entities = await dbContext.MyEntities()
    .Where(entity => entity.IsActive)
    .ToListAsync(cancellationToken);
```

This is optional. You can always use:

```csharp
dbContext.Set<MyEntity>()
```

## Seeding Data

Use `IAppDbContextSeeder` when a module needs default data.

Example:

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

        var existing = await set.FirstOrDefaultAsync(
            entity => entity.Code == "DEFAULT",
            cancellationToken);

        if (existing is null)
        {
            set.Add(new MyEntity
            {
                Code = "DEFAULT",
                Name = "Default item"
            });
        }
        else
        {
            existing.Name = "Default item";
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
```

Register the seeder:

```csharp
services.TryAddEnumerable(
    ServiceDescriptor.Scoped<IAppDbContextSeeder, MyModuleSeeder>());
```

Seeders are run by calling:

```csharp
await serviceProvider.SeedAppDbContextAsync(cancellationToken);
```

In this solution, the Migration console runs seeders when called with `-s`.

## Seeder Rules

Seeders should be idempotent. That means they should be safe to run many times.

Good seeder behavior:

- Insert missing default data.
- Update existing default data when values change.
- Avoid duplicates.
- Use stable ids or stable unique keys for lookup data.

Risky seeder behavior:

- Always adding rows without checking if they exist.
- Deleting user data.
- Assuming seeders run only once.

## Adding A New Entity To A Module

Follow this checklist:

1. Add the entity class in the module domain model project.
2. Add `IEntityTypeConfiguration<T>` in the module persistence layer.
3. Do not call `ToTable` inside the entity configuration.
4. Confirm the module model configuration calls `ApplyModuleTableMappings`.
5. Register the module persistence services.
6. Create a migration from the Migration project.
7. Review the generated migration.
8. Apply the migration.

Example entity:

```csharp
public class MyEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
```

Example configuration:

```csharp
public sealed class MyEntityConfiguration : IEntityTypeConfiguration<MyEntity>
{
    public void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}
```

The table name will be:

```text
MyEntity
```

To use a different table name:

```csharp
using System.ComponentModel.DataAnnotations.Schema;

[Table("MyEntities")]
public class MyEntity
{
    public int Id { get; set; }
}
```

## Common Mistakes

Do not put module-specific entities in the Persistence project. Keep entities in module domain model projects.

Do not hardcode `ToTable` in every entity configuration. Use `ApplyModuleTableMappings`.

Do not use `[Table(Schema = "...")]` for module entities. Use the module schema helper instead.

Do not register the same module model configuration multiple times with different schemas in the same `AppDbContext`.

Do not write seeders that create duplicate data each time they run.

Do not call database migrations from normal web application startup. Use the Migration project for create/apply/seed operations.

## Troubleshooting

If an entity is missing from migrations, check:

- The entity has an `IEntityTypeConfiguration<T>`.
- The module model configuration calls `ApplyConfigurationsFromAssembly`.
- The module model configuration is registered as `IAppDbContextModelConfiguration`.
- The Migration project references the module persistence project.

If a table name is wrong, check:

- Whether the entity has `[Table("...")]`.
- Whether the entity class name is what you expect.
- Whether `ApplyModuleTableMappings` is being called after `ApplyConfigurationsFromAssembly`.

If a schema name is wrong, check:

- The module schema constant.
- Whether the module was registered with a `rootEntitySchema`.
- Whether the integrated schema should be `root_supporting`.

If seed data is not applied, check:

- The seeder implements `IAppDbContextSeeder`.
- The seeder is registered in dependency injection.
- The Migration console was run with `-s`.

