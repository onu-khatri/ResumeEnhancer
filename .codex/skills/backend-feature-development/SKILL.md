---
name: backend-feature-development
description: Implement backend features in ResumeEnhancer using the existing .NET modular architecture, Minimal APIs, FluentValidation, Mediator handlers, Mapster, repositories, EF Core, and tests. Use when Codex needs to add or modify backend behavior in a project-consistent way.
---

# Backend Feature Development

Use this skill to move from a ResumeEnhancer story to review-ready backend code without losing architectural discipline.

## Use this skill when

- a story changes API behavior, validation, service logic, persistence, or backend tests
- the task spans multiple backend layers and needs coordinated delivery
- contract, migration, or rollout risks should be called out early

## Do not use this skill when

- the task is frontend-only
- the change is a tiny local bug fix with no workflow value
- you only need architecture review without implementation

## Delivery workflow

1. Start from the user story, acceptance criteria, and existing behavior.
2. Identify the entry point: endpoint, background flow, or service contract.
3. Update request or response contracts only if the external behavior truly changes.
4. Add or refine validation in the Web layer.
5. Implement business behavior in the SL layer with focused handlers and mapping.
6. Extend persistence abstractions only when the behavior requires it.
7. Implement repository or EF Core details in PL.
8. Add or update tests at the narrowest useful boundary.

## Implementation lenses

- backward compatibility of contracts
- explicit validation and business preconditions
- handler cohesion and mapper correctness
- repository query shape and transaction safety
- migration, seed data, and rollout implications

## ResumeEnhancer focus

- `ResumeEnhancer.<ModuleName>.Web` request validation and endpoint wiring
- `ResumeEnhancer.<ModuleName>.SL` contracts, handlers, and mapping
- `ResumeEnhancer.<ModuleName>.PL` repository and EF Core behavior
- traceability from story language to code and tests

## Output requirements

- impacted layers
- contract changes
- persistence and migration notes
- verification and test coverage summary

## Definition of Done

- `dotnet build application\ResumeEnhancerApp.slnx` passes.
- Unit tests pass: `dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore`.
- When contracts or persistence change, integration tests pass: `dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore`.
- Contract changes are backward-compatible or explicitly flagged as breaking.
- Migration and seed-data impact is stated for the reviewer.

