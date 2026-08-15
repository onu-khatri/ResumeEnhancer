---
name: ef-core-database-architect
description: Design and review ResumeEnhancer database and persistence changes using EF Core, SQL Server, shared persistence infrastructure, and migration discipline. Use when Codex needs to shape schemas, repositories, seeding, mappings, or migration strategy for this project.
---

# EF Core Database Architect

Use this skill for persistence design in the actual stack this repository uses, so schema and data-access changes stay consistent, migration-safe, and reviewable.

## Use this skill when

- a change adds or modifies entities, relationships, indexes, or database schema
- a repository, unit-of-work, or EF configuration needs to be designed or reviewed
- a migration, seed data, or rollout strategy is involved

## Do not use this skill when

- the task is pure application logic with no persistence impact
- a single trivial query tweak does not need schema-level reasoning

## Design workflow

1. Start from the domain entity and the business behavior it supports.
2. Keep EF configuration in `<ModuleName>ModulePL` and persistence abstractions in `<ModuleName>ModuleSL`.
3. Keep domain entities and domain-only concepts in `<ModuleName>ModuleDM`.
4. Prefer existing repository conventions over new abstractions.
5. Add or update migrations deliberately; never hand-edit generated migration code without review.
6. Keep setup data and seed behavior separate from request-time business logic.

## Review lenses

- entity-to-table mapping and configuration placement
- key, index, and relationship correctness
- query shape: `AsNoTracking`, projections, `Include`, paging, and N+1 risk
- transaction boundaries and unit-of-work behavior
- migration safety, ordering, and idempotency
- backward compatibility of schema changes against existing data

## ResumeEnhancer focus

- `<ModuleName>ModulePL` for EF configuration and repository adapters
- `<ModuleName>ModuleSL` for persistence abstractions
- shared infrastructure under `application/Infrastructure`
- migration help: `dotnet run --project application\Infrastructure\Migration\Migration.csproj -- --help`
- for detailed persistence conventions, read `dotnet-backend-patterns/references/ef-core-best-practices.md`

## Definition of Done

- `dotnet build application\ResumeEnhancerApp.slnx` passes.
- When schema or persistence changes, integration tests pass: `dotnet test test\IntegrationTest\ResumeEnhancer.IntegrationTests.csproj --no-restore`.
- Migration impact and seed-data changes are stated explicitly for the reviewer.