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
| ResumeEnhancer backend security review patterns (ownership headers, audit pipeline, config keys, error mapping) | [backend-security-review-guide.knowledge.md](backend-security-review-guide.knowledge.md) |

## Frontend

| Need | Read |
| --- | --- |
| ResumeEnhancer client stack, file layout, and verified conventions | [frontend-client-adaptation.knowledge.md](frontend-client-adaptation.knowledge.md) |

## Repository Topography

| Need | Read |
| --- | --- |
| Fast map of where to gather evidence across repository layers | [repository-topography.knowledge.md](repository-topography.knowledge.md) |

## Architecture And Domain Modeling

| Need | Read |
| --- | --- |
| Architecture design, dependency direction, composition, ownership, integration seams, or architecture verification | [dotnet-modular-architecture.knowledge.md](dotnet-modular-architecture.knowledge.md) |
| Decide whether richer domain modeling is warranted; define language, bounded contexts, invariants, aggregate boundaries, or translation | [domain-modeling.knowledge.md](domain-modeling.knowledge.md) |

## Boundary

- This index is a retrieval map, not a knowledge dump.
- Project-specific persistence facts belong only in `persistence-project.knowledge.md` and the relevant ADRs.
