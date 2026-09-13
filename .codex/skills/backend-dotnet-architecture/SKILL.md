---
name: backend-dotnet-architecture
description: Design .NET backend and modular-application architecture with explicit ownership, dependency, composition, integration, and verification decisions. Use when a change needs architecture judgment before implementation.
---

# .NET Backend Architecture

Use this skill for architecture design before implementation. It produces a decision handoff; it is not the implementation owner and does not replace an independent review.

## Use this skill when

- a backend or cross-layer change needs ownership, dependency, composition, integration, or lifecycle decisions
- a new abstraction, module seam, registration boundary, persistence boundary, or durable architecture choice is proposed
- application, domain, transport, persistence, and host responsibilities must be evaluated together

## Do not use this skill when

- an established local pattern answers the question without a boundary decision
- the task is implementation of an already-settled design; use the owning delivery skill
- the task is an independent review of an existing change; use `$architecture-review`

## Authority discovery

1. Establish the decision from the request, story, requirements, current code, tests, and host composition.
2. When present, use `KnowledgeBase/INDEX.md` as a retrieval map. Select only the current authority whose scope matches the decision area; do not require a fixed knowledge filename.
3. Discover applicable ADRs through the repository's current index/registry or targeted search. Verify status, scope, and conflicts; do not assume a particular ADR number.
4. Read [ResumeEnhancer architecture routing](../architecture-review/references/resumeenhancer-architecture-routing.md) only when project-specific ownership or authority selection is material.
5. Retrieve API/application, persistence, security, performance, or other specialist authority only when that decision area is affected.

## Workflow

1. Define behavior, constraints, affected owners, quality attributes, and unresolved decisions.
2. Trace the current runtime path and dependency graph through transport, application, domain, persistence/integration, composition, configuration, and tests.
3. Propose the smallest ownership and dependency decision that preserves explicit boundaries and existing contracts.
4. Evaluate composition, lifecycle, failure, consistency, configuration, security, observability, rollout, and test consequences that apply.
5. Compare credible alternatives when the choice is durable, costly to reverse, or materially affects quality attributes.
6. Route domain uncertainty to `$architecture-domain-modeling`, persistence detail to `$backend-ef-core`, implementation patterns to `$backend-dotnet-patterns`, security risk to `$backend-security`, and independent review to `$architecture-review`.
7. Recommend `$architecture-adr` only when the decision is durable; state the authority and status rather than duplicating its policy.

## Boundaries with related skills

- `$architecture-domain-modeling` resolves business language and invariants; this skill converts settled constraints into .NET structure.
- `$architecture-review` challenges an existing proposal or diff; this skill owns the initial architecture decision.
- `$backend-feature-development` and `$backend-dotnet-patterns` implement an approved design; they do not reopen architecture without evidence.
- `$backend-ef-core` owns detailed EF Core schema/query/transaction decisions when persistence is affected.
- `$architecture-adr` records the agreed durable decision and consequences.

## Output contract

- decision summary and constraints
- ownership map and dependency direction
- composition, integration, lifecycle, and failure behavior
- affected authority links/status and any unresolved conflict
- alternatives and tradeoffs when material
- verification strategy and precise handoff to implementation, specialist review, or ADR recording
