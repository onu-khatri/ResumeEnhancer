---
name: backend-feature-development
description: Implement backend features through explicit requirements, contracts, validation, application behavior, persistence decisions, and targeted tests. Use when a backend change spans more than a local edit.
---

# Backend Feature Development

Use this skill to move from an approved backend requirement to review-ready code without losing behavioral or architectural clarity.

## Use this skill when

- a story changes API behavior, validation, service logic, persistence, or backend tests
- the task spans multiple backend layers and needs coordinated delivery
- contract, migration, or rollout risks should be called out early

## Do not use this skill when

- the task is frontend-only
- the change is a tiny local bug fix with no workflow value
- you only need architecture review without implementation

## Knowledge routing

1. Check `KnowledgeBase/INDEX.md`.
2. Read the API/application delivery topic for contract, validation, error, mapping, cancellation, or test-boundary decisions.
3. Read the EF Core persistence topic when the change affects data access, schema, migrations, or initialization.
4. Read a project adaptation topic only when the target repository has local architecture or framework conventions that affect the change.

## Delivery phases

### 1. Discover and design

1. Start from the requirement, acceptance criteria, existing behavior, and repository evidence.
2. Identify the affected entry point, module ownership, API or messaging contracts, rules, data, compatibility, security, rollout, and verification boundaries.
3. Identify material unresolved decisions. When one remains, load `$workflow-user-interview`, ask one focused question at a time, and obtain confirmation of the shared-understanding summary before proceeding.
4. Record confirmed decisions, deferred items, and blockers. Do not silently assume material behavior, contract, data, security, or verification details.
5. When the change has material module, dependency, integration, or quality-attribute risk, load `$architecture-review` or `$backend-dotnet-architecture` before implementation. Use `$architecture-domain-modeling` when business rules or bounded-context ownership are unclear.

### 2. Implement backend behavior

1. Implement only the necessary contract, validation, application, domain, and persistence changes, following existing dependency and composition patterns.
2. Make invalid input, authorization boundaries, error behavior, cancellation, mapping, and compatibility intentional at the API or application boundary.
3. When persistence is affected, load `$backend-ef-core` to make schema, query, transaction, migration, initialization, and rollback decisions explicit.
4. When the change creates a meaningful attack surface, load `$backend-security` or `$security-management` for focused secure-design and implementation guidance.

### 3. Verify and harden

1. Add or update tests at the narrowest boundary that proves the changed behavior; include integration coverage when contracts, persistence, composition, or transport behavior changes.
2. Verify happy paths, validation failures, authorization outcomes when applicable, error contracts, persistence behavior, and backward compatibility or explicitly document the breaking change.
3. Investigate test, build, migration, or integration failures before claiming delivery readiness. Keep unverified deployment, performance, or monitoring claims explicit.

### 4. Prepare delivery

1. State migration, initialization, configuration, compatibility, rollout, rollback, observability, and operational follow-up needs that affect the backend change.
2. Route frontend-dependent delivery to `$delivery-full-stack-feature`; this skill owns the backend portion and does not prescribe frontend implementation.
3. Leave deployment execution and production monitoring to the repository's approved delivery process unless the user explicitly requests them.

## Implementation lenses

- contract compatibility and explicit validation
- cohesive application behavior and intentional mapping
- persistence, migration, initialization, and rollout risk
- traceability from requirement to code and tests

## Output requirements

- impacted layers
- user-confirmed decisions, deferred items, and any remaining assumptions or blockers
- contract changes
- persistence, migration, initialization, and rollout notes
- verification and test coverage summary

## Definition of Done

- Run the target project's build and the smallest meaningful tests for the changed behavior.
- Run integration coverage when contract, persistence, composition, or transport behavior changes.
- Contract changes are backward-compatible or explicitly flagged as breaking.
- Persistence, migration, initialization, and rollout impact is stated explicitly.
