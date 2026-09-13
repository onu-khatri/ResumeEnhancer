---
title: .NET Modular Architecture
intent: Help an agent make architecture decisions about ownership, dependency direction, composition, integration seams, and verification.
scope: Reusable modular-.NET architecture guidance. Excludes project module maps, framework helpers, and persistence implementation details.
audience: Autonomous Codex implementation agents, architecture planners, and reviewers
last_reviewed: 2026-08-23
---

# .NET Modular Architecture

## When To Use This Knowledge

Read this after `KnowledgeBase/INDEX.md` when a change crosses layers or modules, introduces a shared abstraction, changes composition or ownership, or needs a design decision before implementation. Do not use it for a local implementation whose boundaries are already established.

## Architecture Decision Workflow

1. State the behavior and boundary under decision. Separate a new capability from a new architectural seam.
2. Identify the primary owner: transport, application orchestration, domain policy, persistence adapter, shared infrastructure, or host composition.
3. Draw dependency direction at the abstraction level. Consumers depend on stable contracts and concepts, not private persistence or transport mechanisms.
4. Choose the smallest interaction seam: local call, relational/query boundary, narrow contract, or intentional higher-level workflow.
5. Evaluate composition: registration location, lifetime, configuration ownership, failure behavior, and test seam. Composition joins owned capabilities; it must not bypass their boundaries.
6. Record rejected alternatives when a durable boundary changes, and use the ADR workflow when the project needs a lasting decision record.
7. Define proof: unit tests for deterministic policy, integration tests for composition/protocol/persistence behavior, and dependency evidence for architecture direction.

## Implementation Procedure

1. Name the behavior and its primary owner before choosing a layer or module.
2. Choose the smallest seam: local call, relational query boundary, narrow contract, or explicit higher-level orchestration.
3. Verify dependency direction, composition location, lifetime, configuration ownership, failure behavior, and test seam.
4. State alternatives and use the ADR workflow for a durable boundary change.

## Decision Lenses

- **Ownership:** One concern has one primary owner, even when other components collaborate.
- **Dependency direction:** Producers can change private delivery mechanisms without forcing consumer changes.
- **Composition:** Registrations and lifetimes are explicit, local, compatible with their dependencies, and replaceable in tests.
- **Integration:** The mechanism is proportionate to the need and makes failure, consistency, and ownership explicit.
- **Evolution:** The design centralizes policy without speculative abstractions or duplicate rules.
- **Operability:** Configuration, errors, cancellation, telemetry, and verification are observable at the appropriate boundary.

## Composition And Integration Example

Derived from `AddApplicationModules` and the narrow lookup contracts consumed by `CreateResumeCommandHandler`.

**Good:** host composition joins module entry points; application collaboration uses a narrow contract.

```csharp
services.Add<ModuleName>Persistence();
services.Add<ModuleName>Web();

public interface I<Capability>LookupService
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
```

**Bad:** application orchestration depends on another module's persistence detail.

```csharp
public Create<ModuleName>Handler(IOtherModuleRepository repository)
{
}
```

## Boundaries

- Do not make an endpoint, UI component, or persistence adapter the owner of a business rule solely because it receives input first.
- Do not expose data contexts, repository implementations, persistence entities, or transport framework types through cross-component application contracts.
- Do not create a shared library merely to avoid one intentional dependency; promote a concept only with stable shared ownership and an independent lifecycle.
- Do not use a mediator, service locator, or composition root as a hidden shortcut around an explicit component contract.
- Do not introduce extra services, eventing, CQRS partitions, or projects for naming purity. Require a concrete ownership, deployment, scale, consistency, or team-autonomy pressure.
- Do not model every collaboration as a service call or every data relationship as direct sharing. Choose from responsibility, not convenience.

## Verification Evidence

- Trace a changed call path from entry point to owner and observable outcome.
- Inspect references and registrations for an unintended reverse or private dependency.
- Exercise the smallest integration boundary when correctness depends on composition, protocol, authorization, transactions, or real persistence.
- Record a durable architecture decision through the owning ADR process rather than leaving its rationale only in a review.

## Project Adaptation Boundary

This topic is generic. For ResumeEnhancer facts, retrieve [ResumeEnhancer architecture routing](../.codex/skills/architecture-review/references/resumeenhancer-architecture-routing.md) through the index. That routing reference directs module, cross-module, API/application, and persistence decisions to their existing authorities.

For ResumeEnhancer, ADR-001 governs module ownership and ADR-002 governs cross-module interaction choice. Retrieve the ADR authority; do not reproduce its rules here.

## Discover Locally Only When

Inspect real project references, composition registrations, and current module state. Do not inspect code merely to rederive generic ownership or narrow-contract rules.
