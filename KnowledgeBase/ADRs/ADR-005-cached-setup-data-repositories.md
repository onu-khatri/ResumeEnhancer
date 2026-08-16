---
title: Cached Setup-Data Repositories
status: accepted
date: 2026-08-16
---

# What

This ADR defines how setup data should be exposed to application logic.

The rule is:

1. Setup data should be exposed through repository contracts in the owning module's `SL`.
2. Implementations should live in the owning module's `PL`.
3. Because setup data changes rarely and is read often, those reads should be cached.
4. Write-side persistence should evict the relevant setup cache keys after setup changes are saved.

# Why

Setup data is used repeatedly for:

- dropdowns
- validation
- FK resolution
- display metadata
- cross-module lookups

Without a dedicated pattern, engineers either:

- query setup tables directly from the wrong layer
- duplicate setup lookups in multiple places
- skip caching and repeat the same database reads

# When

Use this ADR whenever:

- a handler needs setup data
- another module needs a setup lookup owned by this module
- a setup table backs a repeated UI or validation flow

# Where

Recommended placement:

- interface: `OwningModule.SL/Abstractions/Persistence`
- implementation: `OwningModule.PL/Repositories`
- cache invalidation: owning module write repository in `PL`

# Who

This ADR is for:

- backend developers
- reviewers
- architects

# Problem And Constraints

Setup data is persisted module data, but it is not updated as frequently as business transactions.

The code needs a consistent way to:

- reuse setup reads
- keep module ownership clear
- avoid repeated direct queries
- keep stale setup metadata from lingering after writes

# Decision Drivers

- Performance
- Boundary clarity
- Maintainability
- Reuse

# Decision

## Rule 1: Expose setup data from the owning module

Good pattern:

```csharp
public interface ITemplateSetupDataRepository
{
    Task<IReadOnlyList<TemplateRenderTypeSetup>> ListTemplateRenderTypesAsync(
        CancellationToken cancellationToken = default);
}
```

## Rule 2: Cache setup reads

Good pattern:

```csharp
return cacheProvider.GetOrSetAsync<IReadOnlyList<TemplateRenderTypeSetup>>(
    "template:setup:render-types",
    async token => ...,
    cacheOptions,
    cancellationToken);
```

## Rule 3: Evict setup cache on writes

If setup data can be changed through CRUD or admin operations, the owning module should remove relevant setup cache keys after save.

# How A Junior Engineer Should Decide

Use this checklist:

1. Do I need setup data in a handler?
   Use the owning module's setup-data repository.

2. Am I about to query a setup table directly from another module's logic?
   Stop. Go through the owning module's contract.

3. Will this setup table be read often?
   Cache it.

# Considered Options

## Option 1: Direct setup-table queries everywhere

### Benefits

- simple to write at first

### Costs

- duplicated logic
- weak boundaries
- inconsistent caching

## Option 2: Module-owned cached setup-data repositories

### Benefits

- clear ownership
- reusable reads
- cheaper repeated access

### Costs

- extra repository contracts and cache invalidation discipline

This option is accepted.

# Consequences

## Positive

- Setup reads are centralized and consistent.
- Cross-module access stays within module boundaries.
- Repeated setup lookups become cheaper.

## Negative

- More repository code exists.
- Cache eviction must stay aligned with setup writes.

# Follow-Up Actions

1. Add setup-data repositories for module-owned setup tables that are used at runtime.
2. Route setup ID resolution through those repositories.
3. Keep cache eviction aligned with setup writes.

# Related ADRs And Evidence

- [ADR-002-cross-module-integration-rules.md](./ADR-002-cross-module-integration-rules.md)
- [ADR-003-setup-entities-over-persisted-enums.md](./ADR-003-setup-entities-over-persisted-enums.md)
- [ADR-004-setup-table-identity-and-code-based-seeding.md](./ADR-004-setup-table-identity-and-code-based-seeding.md)
- [TemplateSetupDataRepository.cs](/D:/RND/ResumeEnhancer/application/Modules/TemplateModule/TemplateModulePL/Repositories/TemplateSetupDataRepository.cs)
- [ProfilingSetupDataRepository.cs](/D:/RND/ResumeEnhancer/application/Modules/ProfilingModule/ProfilingModulePL/Repositories/ProfilingSetupDataRepository.cs)
