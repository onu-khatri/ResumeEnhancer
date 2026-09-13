---
name: architecture-domain-modeling
description: Apply pragmatic domain modeling to clarify business language, context boundaries, invariants, and model responsibilities. Use when business complexity materially affects architecture or behavior.
---

# Domain Modeling

Use this skill to resolve business-model uncertainty. It produces modeling decisions and constraints; it does not implement features or perform a general architecture review.

## Use this skill when

- business concepts, rules, ownership boundaries, or terminology are ambiguous
- lifecycle or state-transition invariants affect the design
- a bounded-context, aggregate, value-object, domain-service, or translation decision is needed

## Do not use this skill when

- the task is straightforward data maintenance with no meaningful invariant or divergent context
- the issue is purely technical and has no business-model consequence
- the request is implementation or an independent architecture review

## Authority discovery

1. Start with the request, story, requirements, and affected code to identify the business decision.
2. When present, use `KnowledgeBase/INDEX.md` as a retrieval map. Select the current domain-modeling or project authority whose scope matches the decision; do not assume a fixed knowledge filename.
3. Retrieve applicable ADRs through the repository's current index/registry or targeted search. Verify status and applicability before relying on one.
4. Read [ResumeEnhancer architecture routing](../architecture-review/references/resumeenhancer-architecture-routing.md) only when local module ownership, integration, or authority selection is material.
5. If authorities conflict or do not answer the question, report the gap; do not invent a project rule.

## Workflow

1. Extract language used by users, requirements, code, and tests; identify synonyms and overloaded terms.
2. Assess whether a meaningful invariant, lifecycle, policy, ownership boundary, or divergent context exists. Stop with a viability assessment when it does not.
3. Define only the modeling depth justified by evidence: vocabulary, boundary, responsibility, invariant, lifecycle, or translation.
4. Identify where rules are enforced and which component owns state, decisions, and integration mapping.
5. Test the model against commands, queries, failure paths, persistence, integration, and observable user outcomes.
6. Hand resulting constraints to `$backend-dotnet-architecture` when dependency/composition choices remain, or to the owning feature workflow when the design is settled.

## Boundaries with related skills

- `$architecture-review` independently evaluates structural risk in an existing design or diff; this skill supplies domain constraints when requested.
- `$backend-dotnet-architecture` decides .NET ownership, dependency, composition, and integration structure; this skill does not select project layers.
- `$product-user-story` owns requirement/story authoring; this skill may expose unresolved language or invariant questions.
- `$architecture-adr` records a durable decision after the model is agreed; it is not a substitute for modeling evidence.

## Workflow gate

Stop after the viability assessment when the task has no meaningful invariant, divergent context, or lifecycle rule. Do not introduce tactical DDD patterns for simple CRUD.

## Output contract

- viability assessment and reason for the selected modeling depth
- glossary and ownership decisions for terms that affect behavior
- bounded-context, invariant, lifecycle, aggregate/value-object/service, and translation decisions only when justified
- evidence and applicable authority status
- unresolved questions, conflicts, and a precise handoff to the next owning skill
