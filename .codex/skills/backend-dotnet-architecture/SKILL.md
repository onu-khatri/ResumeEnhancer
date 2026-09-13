---
name: backend-dotnet-architecture
description: Design .NET backend and modular-application architecture with explicit ownership, dependency, composition, integration, and verification decisions. Use when a change needs architecture judgment before implementation.
---

# Dotnet Architect

Use this skill when an architecture decision needs .NET-aware reasoning rather than a local implementation pattern.

## Use this skill when

- a backend or cross-layer change needs ownership, dependency, composition, or integration decisions
- a new abstraction, module seam, registration boundary, or durable architecture choice is proposed
- architecture guidance must account for application, domain, transport, persistence, and host responsibilities together

## Do not use this skill when

- the task is not meaningfully architecture-sensitive
- an established local pattern answers the question without a boundary decision
- you only need a design review of an existing change; use `$architecture-review`

## Knowledge Routing

1. Read `KnowledgeBase/INDEX.md`.
2. Read `dotnet-modular-architecture.knowledge.md` before selecting a boundary, dependency, composition, or integration pattern.
3. Read `domain-modeling.knowledge.md` only when business boundaries, language, or invariants are material.
4. For ResumeEnhancer adaptation, read `resumeenhancer-architecture-routing.knowledge.md`, then the authority it identifies.
5. Retrieve Group 1 API/application or EF/persistence knowledge only when that decision area is affected.

## Workflow

1. Clarify the behavior, constraints, and affected owners.
2. Make the smallest boundary decision that preserves dependency direction and explicit composition.
3. Evaluate lifecycle, configuration, failure, and test consequences.
4. State alternatives and tradeoffs when the choice is durable.
5. Use `$architecture-adr` when the project needs the decision recorded.

## Output Requirements

- proposed ownership and dependency direction
- applicable project authority and any unresolved conflict
- composition and verification consequences
- ADR recommendation when the decision is durable
