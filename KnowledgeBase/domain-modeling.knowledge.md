---
title: Domain Modeling
intent: Help an agent decide when richer domain modeling is justified and define language, context, invariants, and model responsibilities pragmatically.
scope: Reusable domain-modeling and DDD decision guidance. Excludes project entities, setup-data implementation, module maps, and persistence details.
audience: Autonomous Codex implementation agents, architecture planners, and reviewers
last_reviewed: 2026-08-23
---

# Domain Modeling

## When To Use This Knowledge

Read this after `KnowledgeBase/INDEX.md` when a task has ambiguous business vocabulary, lifecycle rules, policy-sensitive ownership, interdependent state transitions, or a possible bounded-context change. Do not require it for straightforward data maintenance with no meaningful invariants.

## Pragmatic DDD Workflow

1. Assess modeling value: identify behavior that could become incorrect if represented only as fields and CRUD operations.
2. Establish ubiquitous language from requirements, existing behavior, and stakeholder terms. Resolve aliases and overloaded terms before proposing types or APIs.
3. Identify the context that owns each concept, rule, and lifecycle. A bounded context is justified by divergent meaning, policy, data ownership, or rate of change, not directory layout.
4. Define invariants, allowed transitions, ownership constraints, and invalid states in the model that can enforce them consistently.
5. Choose the smallest construct that protects the rule: entity, value object, aggregate boundary, domain service, application workflow, or policy contract.
6. Make translation explicit at domain, application, persistence, and transport boundaries.
7. Define proof: invariant and transition tests, ownership evidence, and an ADR when a context, integration, or consistency rule becomes durable.

## Implementation Procedure

1. Run viability assessment: name the invariant, lifecycle rule, or divergent meaning that ordinary CRUD cannot protect.
2. Establish language and owner for each concept before choosing types.
3. Define allowed transitions, invalid states, and the consistency boundary.
4. Select the smallest model construct and make translation explicit at boundaries.
5. Test invariants and illegal transitions; use an ADR for a durable context or consistency decision.

## Modeling Lenses

- **Language:** A term has one intended meaning in its context and expresses domain intent rather than storage or UI mechanics.
- **Ownership:** The model that controls a rule owns its lifecycle; a shared-looking concept may need different representations in different contexts.
- **Consistency:** Immediate consistency belongs inside an aggregate; wider coordination belongs in an application workflow or integration contract.
- **Identity and value:** Use identity where continuity matters; use immutable value semantics where equality comes from attributes.
- **Change pressure:** Separate contexts because policy or evolution differs materially, not because a future split sounds plausible.
- **Translation:** Treat mapping as a boundary decision, not as a way for external representations to dictate model semantics.

## Ownership Example

Derived from the explicit relationships established by `Resume`, `PersonalInformation`, and `ResumeModelMapper`.

**Good:** aggregate ownership is explicit while mapping creates the child relationship.

```csharp
public class Aggregate : BusinessEntity
{
    public ChildDetails? Details { get; set; }
    public ICollection<ChildItem> Items { get; } = new List<ChildItem>();
}

var details = new ChildDetails { Aggregate = aggregate };
aggregate.Details = details;
```

**Bad:** a child has no established aggregate relationship and relies on later persistence fix-up.

```csharp
aggregate.Items.Add(new ChildItem());
```

## Boundaries

- Do not declare an aggregate because a database table exists. Aggregate boundaries protect invariants and transactional consistency, not schema grouping.
- Do not make a value object mutable or identity-bearing only to simplify persistence mapping.
- Do not put HTTP state, serialization shape, repository mechanics, or workflow orchestration into a domain object unless it is genuine domain policy.
- Do not adopt domain events, sagas, or event sourcing without a need for asynchronous coordination, temporal reconstruction, or durable event-driven integration.
- Do not split bounded contexts for every noun; split only when meaning, ownership, policy, or evolution differs.
- Do not hide unresolved business rules behind an anemic model and scattered conditionals. Name the rule, locate it, or record it as unresolved.

## Verification Evidence

- Test each claimed invariant and illegal transition.
- Trace key terms from requirements through contracts and model types; record intentional translations and reject accidental aliases.
- Check cross-context interactions use narrow stable data needs, not private model or persistence internals.
- Require an ADR or explicit design record for a new bounded context, shared kernel, integration style, or consistency rule with durable cross-team consequences.

## Project Adaptation Boundary

This topic is generic. Retrieve `resumeenhancer-architecture-routing.knowledge.md` before applying it to ResumeEnhancer. Its module map and cross-module rules remain in ADR-001 and ADR-002.

## Discover Locally Only When

Inspect current requirement vocabulary, lifecycle behavior, and existing model behavior. Do not add tactical DDD merely because an entity exists.
