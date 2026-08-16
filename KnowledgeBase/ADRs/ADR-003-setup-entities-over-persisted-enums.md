---
title: Setup Entities Over Persisted Enums
status: accepted
date: 2026-08-16
---

# What

This ADR defines the default rule for values that must be stored in the database and selected by business entities.

The rule is:

1. Do not persist business choice values as enums on entities.
2. Store those values in setup tables and reference them through foreign keys from business entities.

Examples of setup data covered by this rule:

- resume section definitions
- template render types
- address types
- billing plans
- roles and access profiles when treated as setup data

# Why

Persisted enums look simple at first, but they become brittle when the product evolves.

They create several problems:

- renaming an enum member can become a data migration problem
- adding metadata such as `DisplayName`, `Description`, ordering, or deactivation becomes awkward
- different modules start hard-coding the same values in multiple places
- administrators and future internal tooling cannot manage the values cleanly
- cross-module integrations end up depending on code symbols instead of stored data

Setup tables solve those problems because the database stores:

- relational foreign keys
- human-readable code values
- descriptions and display names
- flags such as `ObsoleteFlag` or `IsDeactivated`
- ordering and other setup-specific metadata

# When

Use this ADR whenever a business entity needs to store a constrained value that:

- can be selected from a known list
- needs metadata beyond just the raw value
- may be referenced by more than one feature or module
- may need reporting, filtering, or admin visibility
- may change over time without rewriting business entity property types

Do not use persisted enums for these cases.

Enum-only persistence is acceptable only when the value is purely in-memory behavior and is not part of durable database state.

# Where

This ADR applies to backend modules under:

- `application/Modules/*`

It applies especially to:

- `*.DM` for entity property design
- `*.PL` for EF mappings

# Who

This ADR is for:

- backend developers
- database designers
- reviewers
- architects
- junior engineers adding new lookup or configuration-like data

If someone asks:

- "Should this be an enum property on the entity?"
- "Where should render types or section types live?"

this ADR is the default answer.

# Problem And Constraints

ResumeEnhancer has several kinds of data that look enum-like in code but behave like shared business reference data in the database.

That data must support:

- stable foreign keys
- metadata columns
- future growth without breaking old rows
- safe reuse across modules

The solution must also preserve the repository layering already used in the codebase:

- `DM` owns the entity model
- `PL` owns EF Core and database access

# Decision Drivers

- Maintainability
- Database evolution safety
- Query clarity
- Cross-module consistency
- Junior readability

# Decision

## Rule 1: Persist setup choices as setup entities, not enum properties

If a value is stored in the database as part of an entity record, prefer this shape:

```csharp
public int RenderTypeId { get; set; }
public TemplateRenderTypeSetup? RenderType { get; set; }
```

Avoid this shape:

```csharp
public TemplateRenderType RenderType { get; set; }
```

The persisted record should depend on a setup-table FK, not on a code enum.

## Rule 2: Setup entities remain first-class module data

Setup values are not just constants. They are rows with metadata and lifecycle.

That means they should live in the owning module's `DM` and `PL`, and they should be treated as real persisted data instead of pretending the database is only storing an enum number.

# How A Junior Engineer Should Decide

Use this checklist:

1. Will this value be stored in the database?
   If yes, do not default to a persisted enum.

2. Does this value need metadata such as display name, description, ordering, or deactivation?
   If yes, create a setup table.

3. Will another feature or module reuse this value?
   If yes, create a setup table with an FK reference from business entities.

# Considered Options

## Option 1: Persist enums directly on entities

### Benefits

- less code at the start
- simple property definitions

### Costs

- weak metadata support
- harder renames and migrations
- poor reuse across modules
- poor fit for admin-managed or queryable reference data

## Option 2: Store strings directly on entities

### Benefits

- easy to read in raw database rows

### Costs

- no relational integrity
- repeated magic strings
- higher risk of inconsistent values

## Option 3: Use setup tables and foreign keys from business entities

### Benefits

- stable relational model
- room for metadata and future growth
- clear ownership by module

### Costs

- more initial code
- requires explicit setup-entity design

This option is accepted.

# Consequences

## Positive

- New setup-like data follows one repeatable pattern.
- Entity models stay stable as product metadata grows.
- Modules can reuse setup data without copying enum knowledge.

## Negative

- More tables and mappings must be maintained.

## Important Tradeoff

This ADR accepts a little more structure in exchange for better long-term safety.

The repository will have more explicit setup entities, but the model will be easier to extend and safer to integrate across modules.

# Follow-Up Actions

1. Use this ADR when adding any new enum-like persisted value.
2. Refactor existing persisted enums to setup tables as they are touched by feature work.
3. Review new module designs for accidental enum persistence before merge.

# Related ADRs And Evidence

- [ADR-001-backlog-driven-module-boundaries.md](./ADR-001-backlog-driven-module-boundaries.md)
- [ADR-002-cross-module-integration-rules.md](./ADR-002-cross-module-integration-rules.md)
- [ADR-004-setup-table-identity-and-code-based-seeding.md](./ADR-004-setup-table-identity-and-code-based-seeding.md)
- [ADR-005-cached-setup-data-repositories.md](./ADR-005-cached-setup-data-repositories.md)
