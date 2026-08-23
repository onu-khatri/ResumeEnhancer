---
name: domain-driven-design
description: Apply pragmatic domain modeling to clarify business language, context boundaries, invariants, and model responsibilities. Use when business complexity materially affects architecture or behavior.
---

# Domain Driven Design

Use this skill pragmatically. The goal is clearer business modeling, not performative DDD vocabulary.

## Use this skill when

- business concepts, rules, or ownership boundaries are ambiguous
- lifecycle, policy, or state-transition invariants affect design
- a design needs a bounded-context, aggregate, value-object, or domain-service decision

## Do not use this skill when

- the task is straightforward data maintenance with no meaningful invariants
- the issue is purely technical and has no business-model consequence

## Knowledge Routing

1. Read `KnowledgeBase/INDEX.md`.
2. Read `domain-modeling.knowledge.md` before proposing a domain-modeling pattern.
3. Read `dotnet-modular-architecture.knowledge.md` when the model decision changes dependencies, composition, or integration seams.
4. For ResumeEnhancer adaptation, read `resumeenhancer-architecture-routing.knowledge.md`, then the authority it identifies.

## Workflow Gate

Stop after the viability assessment when the task has no meaningful invariant, divergent context, or lifecycle rule. Do not introduce tactical DDD patterns for simple CRUD.

## Output Requirements

- DDD viability assessment
- vocabulary, ownership, and invariant decisions when modeling is justified
- explicit translation and integration boundaries when applicable
- evidence and ADR recommendation for durable context or consistency choices
