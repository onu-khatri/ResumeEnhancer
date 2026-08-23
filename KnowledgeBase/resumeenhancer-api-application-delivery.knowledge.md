---
title: ResumeEnhancer API And Application Delivery
intent: Help an agent adapt API and application-delivery changes to ResumeEnhancer without duplicating its persistence knowledge.
scope: ResumeEnhancer Web, AM, SL, mapping, and module-composition conventions. Excludes all persistence implementation, repositories, transactions, schemas, setup data, seeders, migrations, and persistence test seams.
audience: Autonomous Codex implementation agents
last_reviewed: 2026-08-23
---

# ResumeEnhancer API And Application Delivery

## When To Use This Knowledge

Read this after the reusable API/application delivery topic when a task must fit ResumeEnhancer's endpoint, contract, application-flow, mapping, or module-composition conventions.

## Ownership

- `ResumeEnhancer.<ModuleName>.Web` owns HTTP concerns, Minimal API endpoint wiring, and FluentValidation request validation.
- `ResumeEnhancer.<ModuleName>.AM` owns request and response contracts.
- `ResumeEnhancer.<ModuleName>.SL` owns Mediator contracts, handlers, and mapping workflow.
- `ResumeEnhancer.WebSolution.ModulesComposition` is the host-facing module composition boundary.

## Implementation Procedure

1. Place HTTP wiring and FluentValidation in `ResumeEnhancer.<ModuleName>.Web`; preserve AM contracts, SL orchestration, and PL persistence ownership.
2. Reuse `ApiEndpointExecutor` for validation and error execution when its semantics apply.
3. Dispatch use cases through `Mediator`; keep Mapster graph ownership explicit in SL mapping workflow.
4. Register modules through `ResumeEnhancer.WebSolution.ModulesComposition`, never by direct host references to module internals.
5. Select the smallest build/test boundary from `AGENTS.md`; use integration coverage for HTTP, composition, or persistence behavior.

## Delivery Flow

Use the existing flow as the starting point: endpoint -> validator -> Mediator command or query -> handler -> mapping. Keep each responsibility in its owning project and trace the existing call path before extending it.

`ApiEndpointExecutor.ValidateOrExecute` centralizes validation and exception-to-response mapping. Reuse it instead of creating endpoint-specific error plumbing when the existing behavior applies.

The project uses `Mediator`, not MediatR. Mapster mapping is explicit about navigation handling; do not allow mapping to infer graph ownership or update semantics.

## Endpoint Example

`CreateResumeCommandEndpoint` demonstrates this shape. Replace the module placeholder with the owning module only after confirming ownership.

**Good:** Web validates and delegates through `Mediator`; the application layer owns use-case behavior.

```csharp
var validationResult = await validator.ValidateAsync(request, cancellationToken);

return await ApiEndpointExecutor.ValidateOrExecute(
    validationResult.ToDictionary(),
    async () =>
    {
        var response = await mediator.Send(
            new Create<ModuleName>Command(request, auditUserId), cancellationToken);
        return Results.Created($"/api/<module>/{response.Id}", response);
    });
```

**Bad:** an endpoint maps, persists, and returns a domain entity directly.

```csharp
var entity = request.Adapt<DomainEntity>();
dbContext.Add(entity);
await dbContext.SaveChangesAsync(cancellationToken);
return Results.Created($"/api/<module>/{entity.Id}", entity);
```

## Composition And Verification

- Keep module registration behind `ResumeEnhancer.WebSolution.ModulesComposition`; do not wire host code directly to module internals.
- Use project build and test commands from `AGENTS.md` after selecting the smallest meaningful verification boundary.

## Persistence Boundary

- Do not add or document `AppDbContext`, repositories, unit-of-work, query specifications, transactions, schemas, setup-data, seeders, migrations, or persistence test seams here.
- For any persistence decision, read [persistence-project.knowledge.md](persistence-project.knowledge.md) and the applicable ADR through `KnowledgeBase/INDEX.md`.
- When a change selects module ownership or a cross-module interaction, retrieve ADR-001 or ADR-002 through the architecture-routing topic. Do not copy those ADR rules into endpoint guidance.

## Suggested phases

1. **Confirm scope** — read the user story and trace the existing request flow (`endpoint -> validator -> handler -> mapper -> repository`).
2. **Contracts** — update `ResumeEnhancer.<ModuleName>.AM/Requests` and `ResumeEnhancer.<ModuleName>.AM/Responses` only when the API shape must change; keep changes backward-compatible.
3. **Validation** — add or refine `AbstractValidator<TRequest>` in `ResumeEnhancer.<ModuleName>.Web/Validation`.
4. **Contracts + handlers** — add `ICommand`/`IQuery` records in `ResumeEnhancer.<ModuleName>.SL/Contracts` and their handlers in `ResumeEnhancer.<ModuleName>.SL/Handlers`.
5. **Mapping** — extend `ResumeModelMapper` (Mapster) with explicit `.Ignore()` for navigation properties.
6. **ResumeEnhancer.Infrastructure.Persistence** — extend the SL abstraction in `Abstractions/Persistence`, then implement in `ResumeEnhancer.<ModuleName>.PL/Repositories` via `IUnitOfWork<AppDbContext>`.
7. **Tests** — add unit/integration tests at the narrowest useful boundary, then run the build and test commands.

## Discover Locally Only When

Inspect the concrete endpoint, contract, handler, active registration, and existing test shape. Do not rediscover the Web/AM/SL ownership, `ApiEndpointExecutor`, `Mediator`, or Mapster conventions.
