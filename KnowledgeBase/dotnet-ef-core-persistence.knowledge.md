---
title: .NET EF Core Persistence Engineering
intent: Help an agent make deliberate EF Core modeling, query, transaction, migration, initialization, and verification decisions.
scope: Reusable EF Core engineering guidance. Excludes ResumeEnhancer persistence infrastructure, schema conventions, setup-data rules, migration tooling, and test commands.
audience: Autonomous Codex implementation agents
last_reviewed: 2026-08-23
---

# .NET EF Core Persistence Engineering

## When To Use This Knowledge

Read this when a backend change affects an entity, relationship, index, query, transaction, concurrency rule, migration, data initialization, or persistence test strategy.

## Implementation Procedure

1. Define relationship, requiredness, delete, index, and concurrency consequences before changing the model.
2. Shape queries deliberately: filter, deterministic order, bounded page, required projection or graph, and intentional tracking.
3. Define the transaction boundary and concurrency outcome before adding retries or a transaction.
4. Review generated migration operations and data compatibility; keep repeatable initialization separate from request-time behavior.
5. Use relational integration coverage when provider translation, transaction, or schema semantics determine correctness.

## Modeling And Queries

- Model relationships, requiredness, delete behavior, indexes, and concurrency expectations deliberately. A schema change is also a data-compatibility decision.
- Keep persistence configuration and data-access code outside transport and application orchestration layers.
- Choose tracking intentionally. Read-only queries should avoid unnecessary tracking; update flows need an explicit tracking or attach strategy.
- Project only required fields, page bounded result sets, order deterministically, and select eager-loading strategies that avoid N+1 queries and uncontrolled graph expansion.
- Treat query shape as behavior: validate filters and limits, and test the edge cases that affect result correctness.

## Transactions And Concurrency

- Define the operation boundary before introducing a transaction. Do not use a transaction to hide unclear ownership or cross-service consistency problems.
- Treat concurrency conflicts as expected business outcomes when users can edit the same data; choose retry, rejection, merge, or compensation deliberately.
- Keep transaction and retry behavior close to the persistence boundary so application rules do not depend on provider details.

## Query Shape Example

Derived from the bounded query pattern in `ResumeRepository`.

**Good:** make tracking, ordering, paging, and graph loading explicit.

```csharp
var query = db.Set<Aggregate>()
    .AsNoTracking()
    .Where(item => item.OwnerId == ownerId)
    .OrderByDescending(item => item.UpdatedAt)
    .ThenBy(item => item.Id);

var page = await query.Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .Include(item => item.Children)
    .AsSplitQuery()
    .ToListAsync(cancellationToken);
```

**Bad:** result size, tracking, and graph-loading cost become accidental.

```csharp
return await db.Set<Aggregate>()
    .Include(item => item.Children)
    .ToListAsync(cancellationToken);
```

## Migrations And Initialization

- Generate migrations from a reviewed model change, inspect the generated operations, and plan deployment and rollback before applying them.
- Keep initialization repeatable and separate from request-time business operations.
- Seed stable reference data by an immutable business identity rather than an incidental database-generated key when environments can diverge.

## Verification Boundary

- Test mappings, query filters, ordering, paging, relationship behavior, concurrency, and migration-sensitive behavior at the narrowest meaningful persistence boundary.
- Use a relational integration environment when provider behavior, SQL translation, transaction semantics, or schema behavior is material to correctness.

## Boundaries

- Do not duplicate ResumeEnhancer `AppDbContext`, repository, unit-of-work, schema, setup-data, seeder, migration-console, or persistence-test guidance here. Retrieve [persistence-project.knowledge.md](persistence-project.knowledge.md) through the index.
- Do not treat generic EF Core advice as a substitute for the target project's data lifecycle and deployment constraints.

## Discover Locally Only When

Inspect the actual model, provider, migration history, deployment constraints, and data-volume characteristics. Do not rediscover the bounded-query or persistence-verification procedure.
