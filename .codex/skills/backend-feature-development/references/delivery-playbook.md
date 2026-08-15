# Backend Delivery Playbook (ResumeEnhancer)

Use this playbook for backend stories that span more than a single direct edit. It follows the real layer order in the `<ModuleName>`.

## Suggested phases

1. **Confirm scope** — read the user story and trace the existing request flow (`endpoint -> validator -> handler -> mapper -> repository`).
2. **Contracts** — update `<ModuleName>ModuleAM/Requests` and `<ModuleName>ModuleAM/Responses` only when the API shape must change; keep changes backward-compatible.
3. **Validation** — add or refine `AbstractValidator<TRequest>` in `<ModuleName>ModuleWeb/Validation`.
4. **Contracts + handlers** — add `ICommand`/`IQuery` records in `<ModuleName>ModuleSL/Contracts` and their handlers in `<ModuleName>ModuleSL/Handlers`.
5. **Mapping** — extend `ResumeModelMapper` (Mapster) with explicit `.Ignore()` for navigation properties.
6. **Persistence** — extend the SL abstraction in `Abstractions/Persistence`, then implement in `<ModuleName>ModulePL/Repositories` via `IUnitOfWork<AppDbContext>`.
7. **Tests** — add unit/integration tests at the narrowest useful boundary, then run the build and test commands.

## Delivery concerns

- Backward compatibility of request/response models (additive over breaking).
- Validator coverage for user-controlled inputs, using `SetValidator`/`RuleForEach` for nested data.
- Mapper correctness: normalize strings, ignore relational properties, build child graphs explicitly.
- Repository query shape: `AsNoTracking`, `AsSplitQuery`, paging caps, deterministic sort with an `Id` tiebreaker.
- Migration and seed-data impact; call them out in the PR.
- Audit user propagation: pass the audit user id from the endpoint header through to `SaveAsync(IAudit, ct)`.

## Definition of Done

- `dotnet build application\ResumeEnhancerApp.slnx` passes.
- `dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.csproj --no-restore` passes.
- Integration tests pass when contracts or persistence change: `dotnet test test\IntegrationTest\ResumeEnhancer.IntegrationTests.csproj --no-restore`.