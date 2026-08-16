---
title: Cross-Module Integration Rules
status: accepted
date: 2026-08-16
---

# What

This ADR defines the default integration rules between business modules in ResumeEnhancer.

It answers a common architecture question:

"When one module needs data or behavior from another module, how should we connect them without breaking boundaries?"

The decision is:

1. Use entity relationships and EF Core navigation loading for persistence-level relational data.
2. Use small SL integration interfaces for validation, controlled reads, or cross-module business decisions.
3. Do not let one module reach directly into another module's persistence internals.

# Why

ResumeEnhancer is a modular monolith. That means modules live in one solution, but they still need clear boundaries.

Without a rule, engineers usually fall into one of these bad patterns:

- everything becomes a direct project reference and module boundaries stop meaning anything
- every simple relationship becomes a service call, which makes normal relational queries harder than they need to be
- one module starts depending on another module's repositories, handlers, or EF entities in the wrong layer

This ADR exists to keep the code:

- understandable
- consistent
- scalable
- reviewable by junior and senior engineers

# When

Use this ADR whenever:

- one module has a foreign key to data owned by another module
- one module needs to load related data from another module
- one module needs to validate that another module's record exists
- one module needs a small subset of another module's business data in its service layer

Do not overuse it for:

- purely local module logic
- UI-only composition concerns
- cross-cutting infrastructure concerns that belong in shared infrastructure instead of a business module

# Where

This ADR applies to all backend modules under:

- `application/Modules/*`

It applies especially to these layers:

- `*.DM`
- `*.PL`
- `*.SL`

It does not change the existing repository structure rules:

- `Web` owns HTTP concerns
- `AM` owns contracts
- `SL` owns orchestration and use-case logic
- `DM` owns entities and relationships
- `PL` owns EF Core and persistence adapters

# Who

This ADR is for:

- backend developers
- reviewers
- architects
- junior engineers who need a repeatable decision rule

If someone asks:

- "Should I use `Include`?"
- "Should I call another module's service?"
- "Can I inject another module's repository?"

this ADR is the default answer guide.

# Problem And Constraints

Modules in ResumeEnhancer are allowed to collaborate, but they must not collapse into one large codebase with fake boundaries.

The system needs to support:

- one-to-one relationships
- one-to-many relationships
- many-to-many relationships
- validation across module boundaries
- controlled reads of another module's data

At the same time, the code must preserve:

- clean layering
- dependency direction
- low coupling
- understandable ownership

# Decision Drivers

- Maintainability
- Clean code
- Dependency inversion
- Query clarity
- Boundary safety
- Junior readability

# Decision

## Rule 1: DM may reference another module's DM for explicit business relationships

If two modules have a real business relationship, it is acceptable for one module's DM to reference another module's DM entity.

Typical examples:

- a business entity holds an FK to another module's entity
- a many-to-many relation joins records from different modules

Good example:

```csharp
public class ChildEntity
{
    public int ParentEntityId { get; set; }
    public ParentEntity? ParentEntity { get; set; }
}
```

This is acceptable when:

- the relationship is explicit
- the relationship is needed in the data model
- the owning module still owns its own invariants

This is not an excuse to merge domains mentally. It only means the data relationship is real.

## Rule 2: PL owns EF Core `Include`, `ThenInclude`, and relational query shaping

If a module is loading its own aggregate or query result and needs related data from another module, that loading belongs in `PL`.

Good example:

```csharp
var entity = await dbContext.Set<ChildEntity>()
    .Include(x => x.ParentEntity)
    .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
```

If deeper related data is needed:

```csharp
var entity = await dbContext.Set<ChildEntity>()
    .Include(x => x.ParentEntity)
        .ThenInclude(x => x.ParentCategory)
    .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
```

Why this is correct:

- the query starts from the current module's entity
- EF Core navigation loading is a persistence concern
- the repository controls query shape in one place

Recommended repository pattern:

```csharp
private static IQueryable<ChildEntity> ApplyGraphIncludes(IQueryable<ChildEntity> query) =>
    query
        .Include(x => x.ParentEntity)
        .AsSplitQuery();
```

## Rule 3: SL may depend on another module's SL integration abstractions

If a module needs:

- existence validation
- controlled read access
- seeded setup data owned by another module
- a small business snapshot
- cross-module decision support

then `SL` should depend on a narrow interface exposed by the other module's integration surface.

By default, that integration surface should live inside the owning module's `SL` project under an `Integrations` folder.

Recommended default structure:

```text
TemplateModule.SL
  Abstractions/
  Contracts/
  Handlers/
  Integrations/
    ITemplateLookupService.cs
    ITemplateIntegrationService.cs
    TemplateSnapshot.cs
  Mapping/
  Services/
```

Default rule:

- keep integration interfaces inside `OwningModule.SL/Integrations`
- keep integration DTOs or snapshots beside those interfaces
- do not create a separate `OwningModule.Integration` project by default

Create a separate integration project only when one or more of these become true:

- many modules consume the same integration contracts and `SL` becomes too heavy
- the integration contracts need a more stable and independent lifecycle
- dependency trimming becomes important
- compile-time dependency management becomes painful with the current structure

Good pattern:

```csharp
public interface IExternalLookupService
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
```

Or, when more than existence is needed:

```csharp
public interface IExternalIntegrationService
{
    Task<ExternalSnapshot?> GetAsync(int id, CancellationToken cancellationToken = default);
}

public sealed record ExternalSnapshot(
    int Id,
    string Code,
    string DisplayName);
```

Why this is clean:

- dependency points to abstraction
- the owning module controls what it exposes
- no repository internals leak across modules

For seeded setup data that rarely changes, prefer a dedicated repository contract in the owning module's `SL` and an implementation in the owning module's `PL` backed by cache.

Example:

```csharp
public interface ITemplateSetupDataRepository
{
    Task<IReadOnlyList<TemplateRenderTypeSetup>> ListTemplateRenderTypesAsync(
        CancellationToken cancellationToken = default);
}
```

The consuming module may depend on that contract, but it must not query the owning module's setup tables directly.

## Rule 4: SL integrations must return DTOs or snapshots, not tracked EF entities

Cross-module SL contracts should return:

- booleans
- IDs
- small DTOs
- immutable snapshots

They should not return:

- tracked EF Core entities
- repositories
- `DbContext`

Bad example:

```csharp
Task<ExternalEntity> GetAsync(int id);
```

Good example:

```csharp
Task<ExternalSnapshot?> GetAsync(int id);
```

## Rule 5: Never inject another module's repository into your module's SL

This is the most important guardrail.

Do not do this:

```csharp
public class SomeHandler
{
    public SomeHandler(IOtherModuleRepository repository)
    {
    }
}
```

Why it is wrong:

- it couples one module's orchestration to another module's persistence details
- it makes refactoring harder
- it breaks clean dependency direction
- it weakens module ownership

If you need another module in SL, depend on that module's integration interface instead.

## Rule 6: Do not call another module's handlers casually

Avoid this pattern:

```csharp
await _mediator.Send(new OtherModuleCommand(...));
```

This can be tempting, but it creates hidden module-to-module orchestration.

Use it only if you intentionally define a higher-level orchestration boundary and document it clearly.

The default rule is:

- module A should not casually execute module B's handlers
- module A should consume module B through a narrow integration contract

# How A Junior Engineer Should Decide

Use this checklist:

1. Do I only need a database relationship and related data loading?
   Then use FK plus navigation plus `Include` in `PL`.

2. Do I only need to check whether a record in another module exists?
   Then use a small lookup interface in `SL`.

3. Do I need a few business properties from another module in my service layer?
   Then use a small integration service returning a DTO or snapshot.

4. Do I need seeded setup data such as render types, plans, or section definitions?
   Then use the owning module's cached setup-data contract from `SL`, implemented in that module's `PL`.

5. Am I about to inject another module's repository or `DbContext` into my `SL` code?
   Stop. That is the wrong direction.

6. Am I about to return another module's EF entity from an integration service?
   Stop. Return a snapshot instead.

# Good Examples

## Good: FK plus EF Core include

```csharp
var order = await dbContext.Set<Order>()
    .Include(x => x.Customer)
    .SingleOrDefaultAsync(x => x.Id == orderId, cancellationToken);
```

## Good: narrow lookup service

```csharp
if (!await _customerLookupService.ExistsAsync(request.CustomerId, cancellationToken))
{
    throw new InvalidOperationException("Customer was not found.");
}
```

## Good: integration snapshot

```csharp
var customer = await _customerIntegrationService.GetAsync(request.CustomerId, cancellationToken);

if (customer is null)
{
    throw new InvalidOperationException("Customer was not found.");
}

if (!customer.IsActive)
{
    throw new InvalidOperationException("Customer is inactive.");
}
```

# Bad Examples

## Bad: inject another module's repository

```csharp
public CreateOrderHandler(ICustomerRepository customerRepository)
```

## Bad: inject another module's `DbContext`

```csharp
public CreateOrderHandler(CustomerDbContext dbContext)
```

## Bad: return EF entities through SL integration

```csharp
Task<Customer> GetAsync(int id);
```

## Bad: handler-to-handler casual orchestration

```csharp
await _mediator.Send(new CreateCustomerCommand(...));
```

# Example Only: ResumeModule And TemplateModule

This section is only an example of the generic rule above.

`ResumeModule` has:

- `TemplateId : int?`
- `Template : Template?`

That is valid under Rule 1.

`ResumeModule.PL` may load template data with:

```csharp
query.Include(x => x.Template)
```

That is valid under Rule 2.

`ResumeModule.SL` may validate template existence with:

```csharp
ITemplateLookupService
```

That is valid under Rule 3.

`ResumeModule.SL` must not inject `TemplateRepository`.

That is Rule 5.

# Considered Options

## Option 1: Only relational FK and `Include`

### Benefits

- simple persistence model
- fewer interfaces

### Costs

- not enough when service-layer validation or business snapshots are needed

## Option 2: Only service-to-service integration

### Benefits

- strict service abstraction

### Costs

- makes normal relational queries unnecessarily complex
- duplicates EF Core strengths

## Option 3: FK and `Include` in PL, narrow integration services in SL

### Benefits

- best fit for modular monolith design
- clean separation of query loading and orchestration
- easiest default rule for future development

### Costs

- requires discipline in interface design
- engineers must understand two valid integration mechanisms

## Option 4: Create a separate integration project for every module

### Benefits

- strongest assembly-level separation
- contracts can evolve independently from the rest of `SL`

### Costs

- adds project and composition overhead too early
- creates more assembly churn than most module integrations need
- makes the modular monolith heavier without solving an immediate problem

This option is rejected as the default. It may be introduced later only when scale or dependency pressure makes it necessary.

# Consequences

## Positive

- Future module integrations follow one consistent rule.
- Reviewers have a clear standard for boundary checks.
- Junior engineers have a practical decision model.
- EF Core is used where it is strongest.
- Service abstractions are used where they add real value.
- The solution avoids unnecessary extra projects for small or medium integrations.

## Negative

- Engineers still need judgment about when a relationship is truly business-relevant.
- Poorly designed integration interfaces can still become too broad if not reviewed.

## Important Tradeoff

This ADR intentionally allows both:

- relational integration in `DM` and `PL`
- abstraction-based integration in `SL`

That is not duplication.

They solve different problems:

- `DM` and `PL` solve data relationships and query loading
- `SL` solves validation, orchestration, and controlled cross-module business access

# Follow-Up Actions

1. Use this ADR as the default review rule for new module integrations.
2. Keep cross-module integration contracts small and intention-revealing.
3. Reject PRs that inject another module's repository into `SL`.
4. Add specific follow-up ADRs only when a special integration case needs an exception to this default rule.

# Related ADRs And Evidence

- [ADR-001-backlog-driven-module-boundaries.md](./ADR-001-backlog-driven-module-boundaries.md)
- [Resume.cs](/D:/RND/ResumeEnhancer/application/Modules/ResumeModule/ResumeModuleDM/Entities/Resume.cs)
- [ResumeConfiguration.cs](/D:/RND/ResumeEnhancer/application/Modules/ResumeModule/ResumeModulePL/Configurations/ResumeConfiguration.cs)
- [ResumeRepository.cs](/D:/RND/ResumeEnhancer/application/Modules/ResumeModule/ResumeModulePL/Repositories/ResumeRepository.cs)
- [ITemplateLookupService.cs](/D:/RND/ResumeEnhancer/application/Modules/TemplateModule/TemplateModelSL/Integrations/ITemplateLookupService.cs)
