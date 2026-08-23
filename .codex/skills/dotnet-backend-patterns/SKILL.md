---
name: dotnet-backend-patterns
description: Apply .NET backend implementation patterns for API boundaries, application behavior, persistence, and testing. Use when a change needs pattern selection or a framework-aware implementation review.
---

# Dotnet Backend Patterns

Use this skill to select and apply the smallest proven .NET backend pattern that fits the change and the target project.

## Use this skill when

- implementing or reviewing .NET backend code
- you need a pattern for API boundaries, application behavior, persistence, or verification
- a change should align with proven patterns instead of introducing a new stack shape

## Knowledge routing

1. Check `KnowledgeBase/INDEX.md`.
2. Read the API/application delivery topic for request flow, validation, mapping, error behavior, and test-boundary choices.
3. Read the EF Core persistence topic for model, query, transaction, migration, or initialization choices.
4. Read the target project's adaptation topic before applying project-specific frameworks, composition conventions, or abstractions.

## Pattern selection

- Preserve explicit transport, application, domain, and persistence boundaries.
- Prefer established project patterns over introducing a framework or abstraction without a demonstrated need.
- Treat contract compatibility, validation, cancellation, error behavior, query shape, and verification as deliberate design decisions.
- Do not assume a particular mediator, mapper, repository, unit-of-work, or validation library unless the project adaptation guidance requires it.

## Output requirements

- impacted backend layers
- selected pattern and rationale
- persistence and testing notes

## Definition of Done

- The selected pattern fits the target project's existing architecture.
- The build and relevant tests have been run or their gaps are stated explicitly.
- Persistence changes include migration, initialization, and rollout notes.

