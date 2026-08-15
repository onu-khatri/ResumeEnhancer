---
title: Shared Persistence Project Knowledge
intent: help an AI agent safely understand, extend, and review the shared Persistence project without crossing module boundaries
scope: in scope is application/Infrastructure/Persistence/Persistence.csproj and its extension seams; out of scope are ResumeModulePL and Infrastructure/Migration internals except as examples of how shared Persistence is consumed
audience: combination
last_reviewed: 2026-08-15
status: draft
source_plan: KnowledgeBase/persistence-project.kb_plan.md
validation:
  gates:
    A_grounding: PASS
    B_specificity: PASS
    C_reproducibility: PASS
    D_user_interview: PASS
    E_consistency: PASS
    F_boundary_discipline: PASS
    G_currency: PASS
    H_validation_record: PASS
  evidence_density:
    claims: 30
    evidence_entries: 27
  cross_examination: clean
  assumptions:
    - none
  known_gaps:
    - No business requirement or user story appears to target the shared Persistence project directly; traceability for this artifact is architectural rather than feature-story driven.
  verified_this_session:
    - Re-read shared Persistence source files, repo READMEs, module integration examples, and persistence-focused tests on 2026-08-15.
    - Ran `dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.csproj --no-restore --filter "FullyQualifiedName~Infrastructure.Persistence|FullyQualifiedName~Composition.DependencyInjectionTests|FullyQualifiedName~Modules.ResumeModule.Persistence.ResumeModuleSeederTests|FullyQualifiedName~Infrastructure.Migration.AppDbContextDesignTimeFactoryTests"` with 75 passing tests.
---

## Intent

This artifact helps an agent work inside the shared Persistence project at `application/Infrastructure/Persistence` with enough context to extend EF Core infrastructure safely, review persistence changes, and route module-specific work back to the correct project. `Observed`: the project owns `AppDbContext`, unit of work, common repositories, model loading, query specification helpers, seeding contracts, and persistence DI registration.

```csharp
public static IServiceCollection AddAppDbContext(
    this IServiceCollection services,
    Action<IServiceProvider, DbContextOptionsBuilder> configureOptions)
{
    services.AddDbContext<AppDbContext>((serviceProvider, options) =>
    {
        configureOptions(serviceProvider, options);
    });
    services.TryAddScoped<IUnitOfWork<AppDbContext>, UnitOfWork<AppDbContext>>();
    services.TryAddScoped<IUnitOfWorkFactory<AppDbContext>, UnitOfWorkFactory<AppDbContext>>();
    services.TryAddScoped(typeof(IAuditEntityRepository<>), typeof(AuditEntityRepository<>));
    services.TryAddTransient(typeof(IModelLoader<>), typeof(ModelLoader<>));
}
```

Reference: `AddAppDbContext` in [DependencyInjection](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Composition/DependencyInjection.cs).

## When to use this knowledge

Use this artifact when an agent needs to:

1. understand how `AppDbContext` is built from module-provided model configurations
2. add or review shared persistence infrastructure such as `IUnitOfWork<AppDbContext>`, `IAuditEntityRepository<T>`, `IModelLoader<T>`, or seeding helpers
3. verify whether a change belongs in shared Persistence versus a module persistence adapter such as `ResumeModulePL`
4. prepare or review the persistence side of adding a new entity, repository, seeder, or migration

Do not use this artifact as the main reference for module-specific EF mappings or business repository logic. `Observed`: repo guidance keeps schema-specific persistence behavior in module `*PL` projects and shared infrastructure behavior in `application/Infrastructure`.

## Core concepts

- `AppDbContext`: the shared EF Core context that receives module model configurations through DI and executes a common save pipeline. Reference: [AppDbContext](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Context/AppDbContext.cs).
- `IAppDbContextModelConfiguration`: the extension contract each module implements so `AppDbContext` can include that module's EF model. Reference: [IAppDbContextModelConfiguration](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Context/IAppDbContextModelConfiguration.cs).
- `IUnitOfWork<AppDbContext>`: the scoped persistence boundary that coordinates repository resolution, transactions, saves, and setup-entity preloading. Reference: [IUnitOfWork](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/UnitOfWork/IUnitOfWork.cs).
- `IAuditEntityRepository<TElement>`: the shared audited-entity repository abstraction for common CRUD, existence, query specification, and paging operations. Reference: [IAuditEntityRepository](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Repositories/IAuditEntityRepository.cs).
- `IModelLoader<TModel>`: a typed include-path builder that lets repositories resolve navigation-only `Include` paths from richer object-member expressions. Reference: [IModelLoader](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Loading/IModelLoader.cs).
- `IQuerySpecification<T>`: the shared query-shaping contract for criteria, includes, ordering, and optional projection. Reference: [IQuerySpecification](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Querying/IQuerySpecification.cs).
- `IAppDbContextSeeder`: the shared seeding contract that lets modules register idempotent seeders against `AppDbContext`. Reference: [IAppDbContextSeeder](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Seeding/IAppDbContextSeeder.cs).

```csharp
public AppDbContext(
    DbContextOptions<AppDbContext> options,
    IEnumerable<IAppDbContextModelConfiguration> modelConfigurations)
    : base(options)
{
    _modelConfigurations = modelConfigurations.ToArray();
}
```

```csharp
public interface IUnitOfWork<TDbContext> : IDisposable, IAsyncDisposable
    where TDbContext : AppDbContext
{
    TDbContext DbContext { get; }
    Task<IUnitOfWorkTransaction> CreateTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveAsync(IAudit auditUser, CancellationToken cancellationToken = default);
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
}
```

```csharp
public interface IAppDbContextSeeder
{
    Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default);
}
```

These snippets are the shortest mental model for the project: the shared context is composed from module model configurators, writes happen through a unit-of-work contract, and seeders run against the shared context.

## Architectural placement

The shared Persistence project is infrastructure, not a feature module. `Observed`: `Persistence.csproj` references `DomainLibrary` but not any module project, which keeps it generic and reusable.

`Inferred`: the intended dependency direction is:

1. module domain entities live outside Persistence
2. module `*PL` projects implement module-specific model configurations and repositories
3. the application host registers those module persistence services
4. shared Persistence supplies the common EF and unit-of-work runtime they plug into

Evidence combined from repo architecture guidance, host composition, and module persistence registration.

The first file to read in code is `application/Infrastructure/Persistence/README.md`, then `application/Infrastructure/Persistence/Context/AppDbContext.cs`, then `application/Infrastructure/Persistence/Composition/DependencyInjection.cs`. `Recommended`: use that order for onboarding because it moves from responsibilities to runtime composition to save behavior.

```xml
<ItemGroup>
  <ProjectReference Include="..\..\Core\DomainLibrary\DomainLibrary.csproj" />
</ItemGroup>
```

Reference: [Persistence.csproj](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Persistence.csproj).

```csharp
public static IServiceCollection AddApplicationModules(this IServiceCollection services)
{
    services.AddResumeModulePersistence();
    services.AddResumeModuleWeb();

    return services;
}
```

This host snippet shows the intended layering: the host brings in module persistence, and module persistence then plugs into shared `AppDbContext` infrastructure.

## Main workflows

### 1. Building the EF Core model

1. The host or migration console registers module persistence services such as `AddResumeModulePersistence()`. `Observed`.
2. Those module services register `IAppDbContextModelConfiguration` instances. `Observed`.
3. `AddAppDbContext(...)` registers `AppDbContext`, `IUnitOfWork<AppDbContext>`, `IUnitOfWorkFactory<AppDbContext>`, `IAuditEntityRepository<>`, and `IModelLoader<>`. `Observed`.
4. `AppDbContext.OnModelCreating(...)` iterates all injected `IAppDbContextModelConfiguration` implementations and lets each one configure the EF model. `Observed`.

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

Reference: `AppDbContext` in [AppDbContext](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Context/AppDbContext.cs).

```csharp
public void Configure(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResumeModuleDbContextModelConfiguration).Assembly);
    modelBuilder.ApplyModuleTableMappings(typeof(Resume).Assembly, _schema);
}
```

This module example is the key extension seam: modules contribute entity configurations and then hand final schema/table convention work back to shared Persistence.

### 2. Saving changes through the shared pipeline

1. Application code reaches persistence through `IUnitOfWork<AppDbContext>` or directly through `AppDbContext`. `Observed`.
2. `SaveChangesWithPipelineAsync(...)` applies audit values to `IAuditEntity` entries, validates added and modified tracked entities with `Validator.ValidateObject`, then retries concurrency conflicts up to three times by refreshing original values from the database. `Observed`.
3. If a concurrent row no longer exists, the original `DbUpdateConcurrencyException` is rethrown. `Observed`.

```csharp
private async Task<int> SaveChangesWithPipelineAsync(
    IAudit? auditUser,
    bool acceptAllChangesOnSuccess,
    CancellationToken cancellationToken)
{
    ApplyAuditValues(auditUser);
    ValidateTrackedEntities();

    for (var retryCount = 0; ; retryCount++)
    {
        try
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        catch (DbUpdateConcurrencyException concurrencyException)
            when (retryCount < MaxConcurrencyRetryCount)
        {
            if (!await TryRefreshOriginalValuesAsync(concurrencyException, cancellationToken))
            {
                throw;
            }
        }
    }
}
```

Reference: `AppDbContext` in [AppDbContext](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Context/AppDbContext.cs).

### 3. Resolving repositories and transactions inside a scope

1. `UnitOfWork<TDbContext>` caches repository instances per scope in a private dictionary. `Observed`.
2. `GetRepo<TElement>()` resolves the shared audited repository, `GetRepo<TIRepo, TElement>()` resolves a registered module repository, and `GetRepoLight<TIRepo>()` can instantiate an unregistered concrete class through `ActivatorUtilities`. `Observed`.
3. `CreateTransactionAsync(...)` returns `NonRelationalDbTransaction`, `RelationalDbTransaction`, or `NestedDbTransaction` depending on the provider and current transaction state. `Observed`.

```csharp
public IAuditEntityRepository<TElement> GetRepo<TElement>()
    where TElement : AuditEntity
{
    ThrowIfDisposed();

    return GetOrCreateRepository(
        typeof(IAuditEntityRepository<TElement>),
        () => _serviceProvider.GetRequiredService<IAuditEntityRepository<TElement>>());
}

public TIRepo GetRepoLight<TIRepo>()
    where TIRepo : class
{
    ThrowIfDisposed();

    return GetOrCreateRepository(
        typeof(TIRepo),
        ResolveRepository<TIRepo>);
}
```

Reference: `IUnitOfWork` in [IUnitOfWork](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/UnitOfWork/IUnitOfWork.cs) and `UnitOfWork` in [UnitOfWork](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/UnitOfWork/UnitOfWork.cs).

```csharp
public async Task<IUnitOfWorkTransaction> CreateTransactionAsync(
    CancellationToken cancellationToken = default)
{
    ThrowIfDisposed();

    if (!DbContext.Database.IsRelational())
    {
        return new NonRelationalDbTransaction();
    }

    var currentTransaction = DbContext.Database.CurrentTransaction;

    if (currentTransaction is not null)
    {
        return new NestedDbTransaction(currentTransaction);
    }

    var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);

    return new RelationalDbTransaction(transaction);
}
```

An agent usually only needs this snippet to understand transaction behavior: no provider-specific branching lives outside the shared unit of work.

## Key symbols and responsibilities

- `AddAppDbContext(...)`: registers the shared persistence runtime and leaves database-provider configuration to the caller. `Observed`.
- `AppDbContext`: owns model assembly aggregation plus save-time auditing, validation, and concurrency retry. `Observed`.
- `ApplyModuleTableMappings(...)`: normalizes schema names, applies base-column conventions, and maps entity tables with category prefixes. `Observed`.
- `ModuleSchemaName`: validates schema names and builds `root_supporting` integrated schemas. `Observed`.
- `AuditEntityRepository<TElement>`: implements shared query, paging, attach/detach, delete, and query-specification behavior for `AuditEntity` types. `Observed`.
- `ModelLoader<TModel>` and `ModelLoaderNavigator<TModel>`: collect include paths from strongly typed navigation expressions. `Observed`.
- `QuerySpecification<T>`: applies `Criteria`, includes, ordering, and optional projection to an `IQueryable<T>`. `Observed`.
- `SeedSetupDataAsync(...)`: inserts, updates, and obsoletes managed setup rows while preserving user-managed rows. `Observed`.

```csharp
if (typeof(AuditEntity).IsAssignableFrom(entityType))
{
    builder.HasKey(nameof(AuditEntity.Id));
    builder.Property<DateTime>(nameof(AuditEntity.App_CreateDate))
        .HasDefaultValueSql("SYSUTCDATETIME()");
    builder.Property<byte[]>(nameof(AuditEntity.App_Version))
        .IsRowVersion();
}
```

Reference: `ApplyModuleTableMappings` in [ModelBuilderModuleMappingExtensions](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Context/ModelBuilderModuleMappingExtensions.cs).

```csharp
private static string GetTablePrefix(Type entityType)
{
    if (typeof(SetupRelation).IsAssignableFrom(entityType))
    {
        return "SR";
    }

    if (typeof(SetupEntity).IsAssignableFrom(entityType))
    {
        return "S";
    }

    if (typeof(BusinessRelation).IsAssignableFrom(entityType))
    {
        return "BR";
    }

    if (typeof(BusinessEntity).IsAssignableFrom(entityType))
    {
        return "B";
    }

    return string.Empty;
}
```

```csharp
public virtual IQueryable<T> GetQuery(IQueryable<T> inputQuery)
{
    var query = inputQuery.Where(Criteria);

    foreach (var include in Includes)
    {
        query = query.Include(include);
    }

    if (OrderBy is not null)
    {
        query = query.OrderBy(OrderBy);
    }
    else if (OrderByDescending is not null)
    {
        query = query.OrderByDescending(OrderByDescending);
    }

    if (Select is not null)
    {
        query = query.Select(Select);
    }

    return query;
}
```

These are the core shared behaviors agents most often extend: naming conventions and reusable query shaping.

## Rules and invariants

1. Do not put module-specific entities in shared Persistence. `Observed`.
2. Do not hardcode per-entity schema or table mapping in module `IEntityTypeConfiguration<T>` classes when the shared convention should own it. `Observed`.
3. Shared Persistence owns reusable EF infrastructure; schema-specific repository implementations belong in module `*PL` projects. `Observed`.
4. `ModuleSchemaName` accepts only letters, digits, and underscores, and the first character must be a letter or underscore. `Observed`.
5. `AuditEntityRepository<TElement>.FindAsync(...)` enforces `pageNumber >= 1`, `pageSize >= 1`, and `pageSize <= 500`. `Observed`.
6. Seeders must be idempotent and setup data must use stable `Guid` and `Code` identity. `Observed`.

```csharp
if (existingRow is null)
{
    StampCreate(seed);
    setupDataSet.Add(seed);
    continue;
}

var hasChanges = ApplySetupData(existingRow, seed);
hasChanges |= updateFunction(existingRow, seed);
```

Reference: `SeedSetupDataAsync` in [SetupDataSeedingExtensions](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Seeding/SetupDataSeedingExtensions.cs).

```csharp
public static string FromModule(string moduleSchema) =>
    NormalizeSegment(moduleSchema, nameof(moduleSchema));

private static bool IsValidFirstCharacter(char character) =>
    char.IsAsciiLetter(character) || character == IntegratedSchemaSeparator;
```

```csharp
if (pageSize > MaxPageSize)
{
    throw new ArgumentOutOfRangeException(
        nameof(pageSize),
        pageSize,
        $"Page size must be less than or equal to {MaxPageSize}.");
}
```

These validation snippets are the invariants that most often break during extension work: schema names and paging limits are enforced in shared code, not by convention alone.

## Extension pattern

### Add a new entity

1. Create the entity in the owning module domain-model project, not in shared Persistence. `Observed`.
2. Add a module `IEntityTypeConfiguration<T>` in that module's `*PL` project. `Observed`.
3. Ensure the module has an `IAppDbContextModelConfiguration` that calls `ApplyConfigurationsFromAssembly(...)` and `ApplyModuleTableMappings(...)`. `Observed`.
4. Register the module persistence service so the shared `AppDbContext` can see the model configuration. `Observed`.
5. Create and review a migration from `application/Infrastructure/Migration`, because the web host should not run migrations on startup. `Observed`.

Reference example: `ResumeModuleDbContextModelConfiguration` plus the entity configurations under `application/Modules/ResumeModule/ResumeModulePL/Configurations/`.

```csharp
public sealed class ResumeModuleDbContextModelConfiguration : IAppDbContextModelConfiguration
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResumeModuleDbContextModelConfiguration).Assembly);
        modelBuilder.ApplyModuleTableMappings(typeof(Resume).Assembly, _schema);
    }
}
```

Use this as the copy shape. The shared project expects modules to contribute configurations through `IAppDbContextModelConfiguration`, not by editing `AppDbContext` directly.

### Add a new repository

1. Decide whether the repository belongs in shared Persistence or in a module adapter. `Observed`: shared Persistence already covers generic `AuditEntity` behavior; `ResumeRepository` lives in module PL because it implements a module port.
2. If generic audited-entity behavior is enough, use `IAuditEntityRepository<TElement>` through `IUnitOfWork<AppDbContext>.GetRepo<TElement>()`. `Observed`.
3. If module-specific queries are required, define the port outside Persistence and implement it in the module `*PL` project. `Observed`.
4. Register the implementation in module persistence DI and consume it through `GetRepo<TIRepo, TElement>()` or normal DI. `Observed`.

```csharp
public async Task<Resume> AddAsync(
    Resume resume,
    int? auditUserId,
    CancellationToken cancellationToken = default)
{
    await _unitOfWork.GetRepo<Resume>().AddAsync(resume, cancellationToken);
    await SaveAsync(auditUserId, cancellationToken);

    return resume;
}
```

```csharp
services.TryAddScoped<IResumeRepository, ResumeRepository>();
```

This is the module-repository pattern to copy: reuse shared audited repository operations through the unit of work, then register the module-specific interface in module DI.

### Add a new seeder

1. Implement `IAppDbContextSeeder` in the owning module or infrastructure location. `Observed`.
2. Register it with `TryAddEnumerable(ServiceDescriptor.Scoped<IAppDbContextSeeder, ...>())`. `Observed`.
3. For setup tables, use `SeedSetupDataAsync(...)` so inserts, updates, and obsoleting behavior follow the shared conventions. `Observed`.
4. Run seeding from the migration console, not from the normal web app startup path. `Observed`.

```csharp
public sealed class ResumeModuleSeeder : IAppDbContextSeeder
{
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<ResumeSectionSetup>().SeedSetupDataAsync(
            ResumeSectionSetupSeedData.Create(),
            (existingSection, seed) =>
            {
                var hasChanges = false;
                // custom field sync
                return hasChanges;
            },
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
```

This example shows the preferred seeder shape: call the shared setup helper, apply only module-specific field updates in the callback, then save once.

### Add a migration

1. Finish the entity and module mapping change first; migration generation reflects the current `AppDbContext` model. `Observed`.
2. Make sure the migration project references the relevant module persistence project and registers its persistence service in `Program.cs`. `Observed`.
3. Run `dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -c <MigrationName>` and review the generated migration before applying it. `Observed`.
4. Apply with `-a` and seed with `-s` as needed. `Observed`.

```csharp
services.AddResumeModulePersistence();
services.AddAppDbContext((_, options) =>
{
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.MigrationsAssembly(MigrationAssembly.AssemblyName);
    });
});
```

```csharp
processStartInfo.ArgumentList.Add("migrations");
processStartInfo.ArgumentList.Add("add");
processStartInfo.ArgumentList.Add(migrationName);
processStartInfo.ArgumentList.Add("--context");
processStartInfo.ArgumentList.Add(typeof(AppDbContext).FullName!);
```

These two snippets explain almost the whole migration story: the console builds the same shared context shape as the app, then points `dotnet ef` at that context.

## Verification and testing

This session re-verified shared persistence behavior with:

```powershell
dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.csproj --no-restore --filter "FullyQualifiedName~Infrastructure.Persistence|FullyQualifiedName~Composition.DependencyInjectionTests|FullyQualifiedName~Modules.ResumeModule.Persistence.ResumeModuleSeederTests|FullyQualifiedName~Infrastructure.Migration.AppDbContextDesignTimeFactoryTests"
```

Result this session: 75 passed, 0 failed, 0 skipped on 2026-08-15. `Observed`.

```csharp
scope.ServiceProvider.GetRequiredService<AppDbContext>().ShouldNotBeNull();
scope.ServiceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>().ShouldNotBeNull();
scope.ServiceProvider.GetRequiredService<IUnitOfWorkFactory<AppDbContext>>().ShouldNotBeNull();
scope.ServiceProvider.GetRequiredService<IAuditEntityRepository<ResumeModuleDM.Entities.Resume>>()
    .ShouldNotBeNull();
scope.ServiceProvider.GetRequiredService<IModelLoader<ResumeModuleDM.Entities.Resume>>()
    .ShouldNotBeNull();
```

```csharp
resume.GetSchema().ShouldBe(ResumeModuleDatabase.Schema);
resume.GetTableName().ShouldBe("B_Resume");
resume.FindProperty(nameof(Resume.App_Version))!.IsConcurrencyToken.ShouldBeTrue();
```

These test snippets show exactly what the suite is asserting: DI wiring exists, model conventions apply, and concurrency metadata is present.

The most relevant test files for future review are:

- `test/ResumeEnhancer.Tests/Infrastructure/Persistence/AppDbContextTests.cs`
- `test/ResumeEnhancer.Tests/Infrastructure/Persistence/UnitOfWorkTests.cs`
- `test/ResumeEnhancer.Tests/Infrastructure/Persistence/AuditEntityRepositoryTests.cs`
- `test/ResumeEnhancer.Tests/Infrastructure/Persistence/ModelLoaderTests.cs`
- `test/ResumeEnhancer.Tests/Infrastructure/Persistence/ModuleSchemaNameTests.cs`
- `test/ResumeEnhancer.Tests/Infrastructure/Persistence/SetupDataSeedingExtensionsTests.cs`
- `test/ResumeEnhancer.Tests/Composition/DependencyInjectionTests.cs`
- `test/ResumeEnhancer.Tests/Infrastructure/Migration/AppDbContextDesignTimeFactoryTests.cs`

## Pitfalls and boundaries

- Do not move module business rules into shared Persistence just because the code touches EF Core. The shared project is infrastructure-only. `Observed`.
- Do not register the same module model configuration multiple times with different schemas in one `AppDbContext`. `Observed`.
- Do not use `[Table(Schema = ...)]` or per-entity `ToTable(..., schema)` as the primary schema convention path for module entities. `Observed`.
- Do not use EF `HasData` for setup tables that depend on runtime GUID/code matching and obsolete handling. `Observed`.
- If you put a module-specific repository contract in shared Persistence, you create a layering leak between infrastructure and service-layer abstractions. `Inferred`.

```csharp
services.AddSingleton<IAppDbContextModelConfiguration>(
    new ResumeModuleDbContextModelConfiguration(rootEntitySchema));
```

```csharp
services.TryAddEnumerable(
    ServiceDescriptor.Scoped<IAppDbContextSeeder, ResumeModuleSeeder>());
```

These examples are safe because they happen in module composition, not in shared Persistence. If you find yourself editing shared Persistence to register a module-specific repository or schema, you are probably crossing the boundary in the wrong place.

## Clarifications

- Q: Does “Persistence project” mean only the shared Persistence project or also module and migration projects as first-class scope?
  - A: only `Persistence.csproj`; other projects may appear only as examples or evidence.
- Q: Should the artifact optimize for implementation guidance, architectural understanding, or both?
  - A: both.
- Q: Should the artifact include concrete extension recipes?
  - A: yes, for entities, repositories, seeders, and migrations.
- Q: Should evidence be line-level, file-level, or code-snippet based?
  - A: code snippets are preferred.

## Type references

When you need surrounding code, start from these named interfaces and types rather than a general file inventory:

- [IAppDbContextModelConfiguration](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Context/IAppDbContextModelConfiguration.cs)
- [IUnitOfWork](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/UnitOfWork/IUnitOfWork.cs)
- [IUnitOfWorkFactory](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/UnitOfWork/IUnitOfWorkFactory.cs)
- [IAuditEntityRepository](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Repositories/IAuditEntityRepository.cs)
- [IModelLoader](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Loading/IModelLoader.cs)
- [IModelLoaderNavigator](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Loading/IModelLoaderNavigator.cs)
- [IQuerySpecification](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Querying/IQuerySpecification.cs)
- [IAppDbContextSeeder](D:/RND/ResumeEnhancer/application/Infrastructure/Persistence/Seeding/IAppDbContextSeeder.cs)
