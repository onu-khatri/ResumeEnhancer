# Dotnet Architecture Lenses (ResumeEnhancer)

Use these lenses when reviewing or designing .NET changes for ResumeEnhancer. They are grounded in the actual modular monolith, not generic .NET advice.

## Layering and dependency direction

- Do `ResumeEnhancer.<ModuleName>.Web` -> `ResumeEnhancer.<ModuleName>.SL` -> `ResumeEnhancer.<ModuleName>.AM`/`ResumeEnhancer.<ModuleName>.DM` -> `ResumeEnhancer.Core.DomainLibrary` arrows stay intact?
- Is `ResumeEnhancer.<ModuleName>.Web`'s reference to SL compile-time only (`PrivateAssets="all"`)?
- Does `ResumeEnhancer.<ModuleName>.PL` depend on `ResumeEnhancer.Infrastructure.Persistence` + `DM` + `SL` abstractions, never on `Web`?
- Do host concerns stay in `ResumeEnhancer.WebSolution.Server` and cross-module wiring stay in `ResumeEnhancer.WebSolution.ModulesComposition`?

## Composition and registration

- Is each module's DI self-contained in its `DependencyInjection` static class?
- Is Mediator registered once with the SL assembly and `ServiceLifetime.Scoped`?
- Are repositories registered with `TryAddScoped` and seeders with `TryAddEnumerable`?
- Is `AddAppDbContext` / `AddApplicationCaching` invoked only at the composition root?

## Endpoint discipline

- Are endpoints grouped under `MapGroup(...)` and split into command/query endpoint classes?
- Do endpoints validate through `ApiEndpointExecutor.ValidateOrExecute` instead of inline try/catch?
- Are 404/403/400 semantics centralized rather than repeated per endpoint?

## Handler and service-layer cohesion

- Do handlers do one business job and return `ValueTask<TResponse>`?
- Is mapping kept in `ResumeModelMapper` with explicit navigation `.Ignore()`?
- Are persistence abstractions declared in SL and implemented in PL?

## ResumeEnhancer.Infrastructure.Persistence integration

- Do repositories go through `IUnitOfWork<AppDbContext>` and `IAuditEntityRepository<>`?
- Is the audit pipeline (`SaveChangesAsync(IAudit, ...)`) used rather than bypassed?
- Are queries `AsNoTracking`, include-safe (`AsSplitQuery`), paged, and deterministically ordered?
- Is schema isolated per module via `IAppDbContextModelConfiguration` and `<ModuleName>ModuleDatabase.Schema`?

## Operational checks

- Migration safety: are migrations in the dedicated `Migration` project with a design-time factory?
- Seeding: is setup data idempotent via `IAppDbContextSeeder` + `SeedSetupDataAsync`?
- Test seams: do `InternalsVisibleTo("ResumeEnhancer.Tests.Unit")` and the `WebApplicationFactory` integration host exist?

