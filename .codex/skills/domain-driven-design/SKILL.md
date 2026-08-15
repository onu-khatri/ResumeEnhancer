---
name: domain-driven-design
description: Apply domain-driven design thinking to ResumeEnhancer by clarifying business concepts, boundaries, invariants, and model responsibilities. Use when Codex needs to shape new domain behavior or review whether a design fits the business model cleanly.
---

# Domain Driven Design

Use this skill pragmatically. The goal is clearer business modeling for ResumeEnhancer, not performative DDD vocabulary.

## Use this skill when

- business concepts, rules, or boundaries are ambiguous
- you need to shape a new feature around business language and invariants
- a design should be checked for domain leakage or weak model boundaries

## Do not use this skill when

- the task is straightforward CRUD with low business complexity
- the issue is purely technical and not domain-shaped

## Workflow

1. Decide whether the problem actually deserves DDD depth.
2. Identify subdomains, bounded contexts, core terminology, and business rules.
3. Map those ideas to the existing modular monolith rather than pretending the codebase is a blank slate.
4. Define where invariants live and where translation between contexts should happen.
5. If a formal artifact list is needed, read `references/ddd-deliverables.md`.

## ResumeEnhancer focus

- resume lifecycle and ownership rules
- bounded contexts across frontend, Web, SL, and persistence
- language used in business requirements versus code
- invariants that must not leak into transport or storage shortcuts

## Output requirements

- DDD viability assessment
- current vocabulary and boundaries
- candidate aggregates or domain services when relevant
- next implementation or ADR recommendation