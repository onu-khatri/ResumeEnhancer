---
name: dotnet-backend-patterns
description: Apply ResumeEnhancer backend implementation patterns for ASP.NET Core, Minimal APIs, FluentValidation, the Mediator source generator, Mapster, EF Core + SQL Server, repositories, unit of work, and testing. Use when Codex needs the preferred house style for backend changes in this repository.
---

# Dotnet Backend Patterns

Use this skill as the backend implementation playbook for ResumeEnhancer.

## Use this skill when

- implementing or reviewing backend code in this repository
- you need the local house style for endpoints, validation, handlers, repositories, and persistence
- a change should align with proven patterns instead of introducing a new stack shape

## Read these references based on the task

- `references/implementation-playbook.md` for the end-to-end backend flow (contracts, Minimal API endpoints, validators, Mediator handlers, Mapster mapping, repositories, composition)
- `references/ef-core-best-practices.md` for persistence specifics (AppDbContext audit pipeline, entity configuration, module schema, migrations, seeding, query shaping)

## Reusable assets

- `assets/repository-template.cs` for the `IUnitOfWork<AppDbContext>` repository pattern
- `assets/service-template.cs` for the Mediator contract + handler + Mapster mapping slice

## Core rules

- keep HTTP concerns in `<ModuleName>ModuleWeb`
- keep request/response contracts in `<ModuleName>ModuleAM`
- keep contracts, handlers, and mapping workflow in `<ModuleName>ModuleSL`
- keep domain entities in `<ModuleName>ModuleDM`
- keep EF configuration, repositories, and seeding in `<ModuleName>ModulePL`
- use the `Mediator` source generator, not MediatR
- use Mapster mapping with explicit `.Ignore()` for navigation properties
- prefer `IUnitOfWork<AppDbContext>` and `IAuditEntityRepository<>` over new abstractions
- validate with FluentValidation in the Web layer; keep request DTOs in AM

## Output requirements

- impacted backend layers
- pattern or template chosen
- persistence and testing notes

## Definition of Done

- `dotnet build application\ResumeEnhancerApp.slnx` passes.
- Relevant unit tests pass: `dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.csproj --no-restore`.
- When persistence changes, integration tests pass: `dotnet test test\IntegrationTest\ResumeEnhancer.IntegrationTests.csproj --no-restore`.