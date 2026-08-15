# EF Core & ResumeEnhancer.Infrastructure.Persistence Patterns (ResumeEnhancer)

This reference describes the persistence conventions actually used in this repository, so schema and data-access changes stay consistent and migration-safe.

## Entity base types (`Core/DomainLibrary`)

All persistent entities derive from a shared base that carries auditing fields:

- `AuditEntity` — `Id` (int, `[Key]`), `App_CreateUserId`, `App_UpdateUserId`, `App_CreateDate`, `App_UpdateDate`, `App_Version` (rowversion).
- `BusinessData : AuditEntity, IBusinessData` — base for business-owned data.
- `BusinessEntity : BusinessData` — base for aggregate/business entities (`Resume`).
- `SetupData : AuditEntity, ISetupData` — base for setup/lookup data (`Code`, `Description`, `Guid`, `ObsoleteFlag`).

Keep domain entities (`ResumeEnhancer.<ModuleName>.DM/Entities`) dependent only on `ResumeEnhancer.Core.DomainLibrary`, and use `[MaxLength]` annotations for column hints.

```csharp
public class Resume : BusinessEntity
{
    [MaxLength(200)] public string Title { get; set; } = string.Empty;
    public PersonalInformation? PersonalInformation { get; set; }
    public ICollection<Education> Education { get; set; } = new List<Education>();
}
```

## Shared `AppDbContext` (`Infrastructure/Persistence`)

`AppDbContext` is the single aggregate context. Modules do not create their own contexts; they contribute model configuration via `IAppDbContextModelConfiguration`.

Key behaviors baked into `SaveChangesAsync`:

- **Audit pipeline** — `SaveChangesAsync(IAudit auditUser, ...)` sets `App_CreateDate`/`App_UpdateDate` and `App_CreateUserId`/`App_UpdateUserId` from the caller-supplied `IAudit { int? UserId }`.
- **Concurrency retry** — `DbUpdateConcurrencyException` triggers an original-values refresh (up to 3 attempts).
- **DataAnnotations validation** — tracked Added/Modified entities are validated with `Validator.ValidateObject` before saving.

```csharp
public async Task<int> SaveChangesAsync(IAudit auditUser, CancellationToken ct = default)
    => await SaveChangesWithPipelineAsync(auditUser, acceptAllChangesOnSuccess: true, ct);
```

## Per-module schema and model configuration

Each module declares its own schema and a model configuration that is registered into the shared context.

```csharp
// ResumeEnhancer.<ModuleName>.PL/Context/<ModuleName>ModuleDatabase.cs
public static class <ModuleName>ModuleDatabase
{
    public const string Schema = "resume";
    public static string GetSchema(string? rootEntitySchema = null) =>
        string.IsNullOrWhiteSpace(rootEntitySchema)
            ? ModuleSchemaName.FromModule(Schema)
            : ModuleSchemaName.FromRootAndSupportingModule(rootEntitySchema, Schema);
}
```

`<ModuleName>ModuleDbContextModelConfiguration` implements `IAppDbContextModelConfiguration.Configure(ModelBuilder)` and applies the module's `IEntityTypeConfiguration<>` classes plus its schema. Registration is idempotent:

```csharp
services.TryAddEnumerable(ServiceDescriptor.Scoped<IAppDbContextSeeder, <ModuleName>ModuleSeeder>());
services.TryAddScoped<IResumeRepository, ResumeRepository>();
```

## Entity configuration (`ResumeEnhancer.<ModuleName>.PL/Configurations`)

One `IEntityTypeConfiguration<T>` per entity, kept in `ResumeEnhancer.<ModuleName>.PL/Configurations`. Prefer this over `OnModelCreating` sprawl.

```csharp
public sealed class ResumeConfiguration : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Title).HasMaxLength(200).IsRequired();
        builder.Property(r => r.UserId).HasMaxLength(450).IsRequired();
        builder.HasIndex(r => r.UserId);
    }
}
```

## Typed `DbSet` accessors

Modules expose `DbSet<T>` through extension methods instead of adding properties to the shared context:

```csharp
public static class AppDbContext<ModuleName>ModuleExtensions
{
    public static DbSet<Resume> Resumes(this AppDbContext dbContext) => dbContext.Set<Resume>();
    public static DbSet<Skill> Skills(this AppDbContext dbContext) => dbContext.Set<Skill>();
}
```

## Repository base (`IAuditEntityRepository<T>`)

`AuditEntityRepository<TElement>` is the generic repository used through the unit of work. It provides `Query()`, `GetQuery(id/ids)`, `FindAsync`, `ExistsAsync`, `AddAsync`, `Update`, `Delete`, `FindBySpecification`, and a paged `FindAsync` with `IModelLoader` support.

## Query shaping

- Use `AsNoTracking()` for read paths.
- Use `Include`/`ThenInclude` + `AsSplitQuery()` for wide graphs (see `ResumeRepository.GetAsync` / `SearchAsync`).
- Validate paging and date ranges explicitly before querying (page size is capped; `MaxPageSize`).
- Apply deterministic ordering with a `ThenBy(Id)` tiebreaker.
- Do not materialize before filtering, sorting, or paging.

## QuerySpecification and ModelLoader

For reusable query shapes, `IQuerySpecification<T>` / `QuerySpecification<T>` bundle `Criteria`, `Select`, `Includes`, `OrderBy`, and `OrderByDescending` behind `GetQuery`. `IModelLoader<T>` / `ModelLoader<T>` build include paths via `IModelLoaderNavigator` for dynamic navigation loading.

## Transactions and Unit of Work

`IUnitOfWork<TDbContext>` is the entry point for repositories and transactions:

- `GetRepo<TElement>()` / `GetRepo<TIRepo, TElement>()` / `GetRepoLight<TIRepo>()` resolve repositories per unit of work.
- `CreateTransactionAsync()` returns an `IUnitOfWorkTransaction` (relational, nested, or non-relational depending on the provider).
- `SaveAsync(IAudit auditUser, ct)` persists with audit metadata.
- `PreloadSetupEntities(...)` attaches setup entities so lookups do not trigger duplicate inserts.

## Migrations (`Infrastructure/Migration`)

Migrations live in a dedicated `Migration` project that references `ResumeEnhancer.Infrastructure.Persistence` and `ResumeEnhancer.<ModuleName>.PL`. It uses `AppDbContextDesignTimeFactory` for design-time configuration and exposes a CLI entry point:

```
dotnet run --project application\Infrastructure\Migration\ResumeEnhancer.Infrastructure.Migration.csproj -- --help
```

Add migrations deliberately; never hand-edit generated migration code without review, and state schema impact in the PR.

## Seeding

Seeding is pluggable via `IAppDbContextSeeder`. `SeedAppDbContextAsync` runs every registered seeder in a scope; module seeders (e.g. `<ModuleName>ModuleSeeder`) use `SeedSetupDataAsync` for idempotent setup data.

```csharp
public sealed class <ModuleName>ModuleSeeder : IAppDbContextSeeder
{
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken ct = default)
    {
        await dbContext.Set<ResumeSectionSetup>().SeedSetupDataAsync(
            ResumeSectionSetupSeedData.Create(),
            (existing, seed) => { /* reconcile fields, return hasChanges */ },
            ct);
        await dbContext.SaveChangesAsync(ct);
    }
}
```

