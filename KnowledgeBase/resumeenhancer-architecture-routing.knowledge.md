---
title: ResumeEnhancer Architecture Routing
intent: Route an agent to the current ResumeEnhancer authority for a project-specific architecture decision without duplicating that authority.
scope: Retrieval order and authority boundaries for ResumeEnhancer architecture work. Excludes restatement of ADRs, API/application flow, and persistence facts.
audience: Autonomous Codex implementation agents, architecture planners, and reviewers
last_reviewed: 2026-08-23
---

# ResumeEnhancer Architecture Routing

## Retrieval Order

1. Read `KnowledgeBase/INDEX.md`.
2. Read the generic architecture, domain-modeling, or architecture-review topic that matches the decision.
3. Retrieve the governing project authority below before proposing or reviewing a ResumeEnhancer-specific decision.
4. If authorities conflict or are incomplete, report the conflict and recommend the smallest requirement clarification or ADR. Do not invent a local exception.

## Governing Authorities

| Decision area | Read | Boundary |
| --- | --- | --- |
| Backend business-module ownership, Identity/profiling scope, and backlog-to-module mapping | `ADRs/ADR-001-backlog-driven-module-boundaries.md` | This routing topic does not restate the module map. |
| Cross-module relationships, data loading, lookup, snapshot, and integration-contract choices | `ADRs/ADR-002-cross-module-integration-rules.md` | This routing topic does not restate integration rules. |
| Endpoint, AM, SL, validation, Mediator, mapping, and host-facing composition adaptation | `resumeenhancer-api-application-delivery.knowledge.md` | Do not duplicate API/application flow here. |
| EF Core model, repositories, transactions, setup data, seeding, migrations, and persistence test seams | `persistence-project.knowledge.md` and applicable ADRs | This topic contains no persistence implementation facts. |
| Generic API/application or EF Core decision methods | Group 1 topics through `INDEX.md` | Retrieve selectively; do not load by default. |
| A durable new architecture decision | `architecture-adr` skill and affected authority | This topic does not prescribe ADR content. |

## Boundaries

- `AGENTS.md` requires index-first selective retrieval before planning, implementing, reviewing, or documenting. It does not authorize a full knowledge-base read by default.
- ADR-001 and ADR-002 remain the authority for existing module and cross-module decisions. Skills and knowledge topics may route to them but must not copy their rules.
- Group 1 API and persistence knowledge remains authoritative for its stated scope. Architecture guidance identifies a decision area and then hands off to that authority for project facts.
- When an authority is proposed rather than accepted, report that status if it affects the recommendation.
