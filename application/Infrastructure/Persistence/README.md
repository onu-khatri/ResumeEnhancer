# Persistence Project

This project contains the shared Entity Framework Core infrastructure used by the application and all modules.

The Persistence project should stay generic. It should know how to build `AppDbContext`, run module model configurations, apply table/schema conventions, and execute seeders. It should not contain business entities for a specific module.

## What This Project Does

- Provides the shared `AppDbContext`.
- Allows modules to plug their EF model configuration into `AppDbContext`.
- Provides a common table/schema mapping convention for modules.
- Applies table prefixes based on `DomainLibrary.DomainModel` categories.
- Provides a common seeding contract.
- Provides setup-data seeding helpers for `SetupEntity` and `SetupRelation` data.
- Maps `AuditEntity.App_Version` as a database-managed rowversion concurrency token.
- Provides a scoped `UnitOfWork<AppDbContext>` persistence boundary.
- Provides common audited-entity repositories and query loading helpers.
- Provides dependency injection helpers for registering the database context.

## Important Files

| File | Purpose |
| --- | --- |
| `Audit/IAudit.cs` | Audit-user contract used by audit-aware saves. |
| `Composition/DependencyInjection.cs` | Registers `AppDbContext`, unit of work, repositories, and loaders in dependency injection. |
| `Context/AppDbContext.cs` | Shared EF Core `DbContext` used by the application. |
| `Context/IAppDbContextModelConfiguration.cs` | Contract that modules implement to configure their entities. |
| `Context/ModelBuilderModuleMappingExtensions.cs` | Applies module table/schema conventions. |
| `Context/ModuleSchemaName.cs` | Builds and validates schema names. |
| `Loading/ModelLoader.cs` | Typed model-loader builder for nested include paths. |
| `Querying/IQuerySpecification.cs` | Query specification contract for reusable repository queries. |
| `Repositories/AuditEntityRepository.cs` | Default repository implementation for common `AuditEntity` operations. |
| `Seeding/AppDbContextSeederExtensions.cs` | Runs all registered seeders. |
| `Seeding/IAppDbContextSeeder.cs` | Contract that modules implement to seed data. |
| `Seeding/SetupDataSeedingExtensions.cs` | Inserts, updates, and obsoletes setup data by `Guid` and `Code`. |
| `Seeding/SeedingUser.cs` | Defines the technical seed user id used by setup seeding. |
| `Transactions/*DbTransaction.cs` | Relational, nested, and non-relational transaction wrappers. |
| `UnitOfWork/UnitOfWork.cs` | Coordinates repositories, saves, setup preloading, and transactions for one persistence scope. |

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

## Unit Of Work

`UnitOfWork<AppDbContext>` is registered as scoped when `AddAppDbContext` is called. In a normal request or service scope, the system creates one scoped `AppDbContext` and one scoped `UnitOfWork<AppDbContext>`.

The unit of work is an infrastructure boundary. It coordinates repository access, save operations, setup entity preloading, and transaction creation. It does not contain business rules.

```csharp
public sealed class ResumeService
{
    private readonly IUnitOfWork<AppDbContext> _unitOfWork;

    public ResumeService(IUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task SaveResumeAsync(Resume resume, IAudit audit, CancellationToken cancellationToken)
    {
        var resumes = _unitOfWork.GetRepo<Resume>();

        await resumes.AddAsync(resume, cancellationToken);
        await _unitOfWork.SaveAsync(audit, cancellationToken);
    }
}
```

`SaveAsync(IAudit)` delegates to `AppDbContext.SaveChangesAsync(auditUser)`, where audit fields, validation, and optimistic concurrency retry are handled.

## Repository Access

Use `GetRepo<TElement>()` for common audited-entity work:

```csharp
var resumes = unitOfWork.GetRepo<Resume>();

var exists = await resumes.ExistsAsync(resumeId, cancellationToken);
var page = await resumes.FindAsync(
    pageNumber: 1,
    pageSize: 25,
    filter: resume => resume.UserId == userId,
    cancellationToken: cancellationToken);
```

Use `GetRepo<TIRepo, TElement>()` for custom repositories registered by a module:

```csharp
var resumeRepository = unitOfWork.GetRepo<IResumeRepository, Resume>();
```

Use `GetRepoLight<TIRepo>()` for custom repositories that are not tied to a single entity type.

## Transactions

`CreateTransactionAsync` returns a transaction wrapper:

- non-relational providers receive a no-op `NonRelationalDbTransaction`.
- relational providers begin a real EF transaction when none exists.
- relational providers return a nested wrapper when a transaction is already active.

```csharp
await using var transaction = await unitOfWork.CreateTransactionAsync(cancellationToken);

await unitOfWork.SaveAsync(audit, cancellationToken);
await transaction.CommitAsync(cancellationToken);
```

Nested transaction commit is a no-op. Nested rollback rolls back the active EF transaction.

## Query Specification And Model Loader

`IQuerySpecification<T>` gives modules a reusable query shape with criteria, includes, ordering, projection, and a final `GetQuery` method.

`IModelLoader<TModel>` provides a typed way to describe nested include paths:

```csharp
var loader = new ModelLoader<Resume>()
    .Build(resume => resume
        .Load(model => model.PersonalInformation)
        .Load(model => model.Skills)
        .NavigateCollection(model => model.WorkExperiences)
            .LoadRelated());
```

The repository resolves loader paths against EF metadata and includes only valid navigation paths. This means a path can point through a navigation to a specific scalar property, while EF only receives the navigation portion needed to load the object graph.

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
3. If the entity inherits one of the shared domain category bases, add the category prefix.

| Base Type | Prefix | Example |
| --- | --- | --- |
| `SetupEntity` | `S_` | `S_ResumeSectionSetup` |
| `SetupRelation` | `SR_` | `SR_AssignmentType_CommTemplate` |
| `BusinessEntity` | `B_` | `B_Resume` |
| `BusinessRelation` | `BR_` | `BR_Education` |

Default example:

```csharp
using DomainLibrary.DomainModel;

public class Resume : BusinessEntity
{
    public int Id { get; set; }
}
```

Maps to:

```text
B_Resume
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
B_CandidateResume
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

## Audit And Concurrency

Entities that inherit `AuditEntity` receive audit columns and `App_Version`.

`App_Version` is mapped with `.IsRowVersion()`. SQL Server manages this value, and EF Core uses it for optimistic concurrency checks during update and delete operations. Application code should not manually increment or edit it.

`AppDbContext.SaveChangesAsync` retries concurrency conflicts by refreshing original values from the database, then retrying the save. If the row was deleted, the original concurrency exception is rethrown.

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
- Use stable `Guid` and `Code` values for setup data.
- Mark removed setup seeds as obsolete instead of deleting them.

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

Do not use EF `HasData` for setup tables that need runtime code/GUID matching or obsolete behavior.

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

