---
name: backend-ef-core
description: Design and review EF Core persistence changes with explicit schema, query, transaction, migration, initialization, and verification decisions. Use when a backend change has meaningful persistence impact.
---

# EF Core Database Architect

Use this skill for persistence design that is migration-safe, reviewable, and compatible with the target application's data lifecycle.

## Use this skill when

- a change adds or modifies entities, relationships, indexes, or database schema
- a repository, unit-of-work, or EF configuration needs to be designed or reviewed
- a migration, seed data, or rollout strategy is involved

## Do not use this skill when

- the task is pure application logic with no persistence impact
- a single trivial query tweak does not need schema-level reasoning

## Design workflow

1. Check `KnowledgeBase/INDEX.md`, then read the EF Core persistence topic.
2. Read the target project's persistence knowledge before relying on local abstractions, schema conventions, migration tooling, or initialization rules.
3. Start from the business behavior and data lifecycle, then identify the entity, relationship, query, transaction, migration, and rollout impact.
4. Keep persistence configuration and adapters outside transport and application orchestration code.
5. Prefer established project persistence conventions over new abstractions.
6. Add or update migrations deliberately; never hand-edit generated migration code without review.
7. Keep initialization and seed behavior separate from request-time business logic.

## Review lenses

- entity-to-table mapping and configuration placement
- key, index, and relationship correctness
- query shape: `AsNoTracking`, projections, `Include`, paging, and N+1 risk
- transaction boundaries and unit-of-work behavior
- migration safety, ordering, and idempotency
- backward compatibility of schema changes against existing data

## Definition of Done

- The project build and relevant persistence tests have been run or their gaps are stated explicitly.
- Migration, initialization, compatibility, and rollout impact is stated for the reviewer.
- The design does not depend on unverified project-specific persistence assumptions.

