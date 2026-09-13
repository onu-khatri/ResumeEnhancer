---
title: Knowledge Base Index
intent: Route an agent to the smallest knowledge topic needed for the next decision.
scope: Discovery index for reusable engineering knowledge, ResumeEnhancer project knowledge, and ADRs. It does not replace the linked artifacts.
audience: Autonomous Codex implementation agents
last_reviewed: 2026-08-23
---

# Knowledge Base Index

## Use This Index First

Before planning, implementing, reviewing, or documenting work, identify the task's decision area below and read only the linked topic or topics that apply. Do not load the entire knowledge base by default.

## .NET Backend

| Need | Read |
| --- | --- |
| API contracts, boundary validation, application flow, error behavior, cancellation, or test-boundary selection | [dotnet-backend-api-delivery.knowledge.md](dotnet-backend-api-delivery.knowledge.md) |
| EF Core model, query, transaction, migration, initialization, or persistence verification decisions | [dotnet-ef-core-persistence.knowledge.md](dotnet-ef-core-persistence.knowledge.md) |
| ResumeEnhancer endpoint, validation, Mediator, mapping, or module-composition adaptation | [resumeenhancer-api-application-delivery.knowledge.md](resumeenhancer-api-application-delivery.knowledge.md) |
| ResumeEnhancer persistence model, repositories, transactions, setup data, seeders, migrations, or persistence test seams | [persistence-project.knowledge.md](persistence-project.knowledge.md) |
| ResumeEnhancer caching behavior or provider changes | [infrastructure-caching-project.knowledge.md](infrastructure-caching-project.knowledge.md) |
| Module boundaries, cross-module rules, setup-data decisions, or cached setup repositories | [ADRs](ADRs/) |

## Architecture And Domain Modeling

| Need | Read |
| --- | --- |
| Architecture design, dependency direction, composition, ownership, integration seams, or architecture verification | [dotnet-modular-architecture.knowledge.md](dotnet-modular-architecture.knowledge.md) |
| Decide whether richer domain modeling is warranted; define language, bounded contexts, invariants, aggregate boundaries, or translation | [domain-modeling.knowledge.md](domain-modeling.knowledge.md) |
| Architecture-sensitive design or diff review with finding severity, structural risk, and verification evidence | [architecture-review skill](../.codex/skills/architecture-review/SKILL.md) |
| Remote boundaries, asynchronous messaging, resilience, eventual consistency, independent deployment, or distributed observability review | [distributed review reference](../.codex/skills/architecture-review/references/distributed-review-guide.md) |
| ResumeEnhancer architecture authority selection without duplicating ADR or project knowledge | [architecture routing reference](../.codex/skills/architecture-review/references/resumeenhancer-architecture-routing.md) |

## Skill System

| Need | Read |
| --- | --- |
| Maintain backend and architecture skill authority boundaries, examples, self-sufficiency, and ADR-routing policy | [skill-system maintenance reference](../.codex/skills/architecture-review/references/skill-system-maintenance.md) |

## Boundary

- This index is a retrieval map, not a knowledge dump.
- Project-specific persistence facts belong only in `persistence-project.knowledge.md` and the relevant ADRs.
