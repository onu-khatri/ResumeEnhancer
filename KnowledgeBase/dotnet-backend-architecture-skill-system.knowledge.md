---
title: .NET Backend And Architecture Skill System
intent: Maintain backend and architecture skills so they route agents to the smallest authoritative knowledge, preserve reusable boundaries, and remain synchronized with their source bodies.
scope: Backend and architecture skill maintenance, including authority routing, examples, self-sufficiency, ADR retrieval, and structural verification. Excludes project implementation rules, ADR contents, and application-code changes.
audience: Codex skill maintainers and agents maintaining backend and architecture knowledge
last_reviewed: 2026-08-23
---

# .NET Backend And Architecture Skill System

## Intent

Use this topic when maintaining a backend or architecture skill, its knowledge routing, or its linked source body. It establishes what belongs in the skill, what belongs in reusable knowledge, and when project facts must be retrieved from routing or ADR authorities.

**Observed:** `AGENTS.md` requires index-first, selective knowledge retrieval and separates skills, custom agents, OpenSpec workflows, and root guidance. The active backend and architecture skills implement that model by routing to `KnowledgeBase/INDEX.md` before choosing detailed authorities.

## When To Use This Knowledge

Read this before changing any of these backend and architecture skills:

- `backend-feature-development`, `dotnet-backend-patterns`, or `ef-core-database-architect`
- `dotnet-architect`, `domain-driven-design`, or `architect-review`
- a source body maintained by `.codex/tools/sync-linked-skill-bodies.ps1` or related synchronization tooling
- the index entry or routed authority used by one of those skills

Do not use this topic to implement a product feature, restate ResumeEnhancer module rules, or replace the API, persistence, architecture, or ADR authorities selected through the index.

## Core Concepts

### Authority Separation

**Observed:** A skill owns its trigger, workflow, specialist gates, and required output. A knowledge topic owns reusable decision guidance. `AGENTS.md`, project knowledge, and ADRs own repository-specific constraints and facts.

```text
Skill: select the task workflow and required gates
Knowledge: explain a reusable decision method
Project authority: provide ResumeEnhancer-specific rules and current facts
ADR: govern accepted durable architecture decisions
```

The active backend-feature skill shows the intended entrypoint shape: it directs agents to the index, API/application guidance, EF Core guidance, and project adaptation without embedding implementation-specific repository details.

```md
## Knowledge routing

1. Check `KnowledgeBase/INDEX.md`.
2. Read the API/application delivery topic for contract and validation decisions.
3. Read the EF Core persistence topic when data access or migrations are affected.
4. Read a project adaptation topic only when local conventions affect the change.
```

### Self-Sufficiency

**Observed:** Active operational knowledge topics such as `dotnet-backend-api-delivery.knowledge.md`, `dotnet-modular-architecture.knowledge.md`, and `architecture-review.knowledge.md` contain an implementation or review procedure, verification evidence, explicit boundaries, and a `Discover Locally Only When` section.

For backend and architecture skill maintenance, each operational topic must let an agent:

1. Select a stable pattern or review method.
2. Follow a concrete procedure.
3. Understand a high-risk boundary from concise prose or an approved example.
4. Choose the evidence that proves the decision.
5. Identify which changed or volatile facts still require local discovery.

Self-sufficiency reduces routine rediscovery; it never permits an agent to skip inspection of changed contracts, registrations, consumers, migrations, requirements, or ADR status.

### Examples

**Observed:** Existing generic knowledge uses concise good/bad examples for high-risk boundary decisions. `dotnet-backend-api-delivery.knowledge.md` distinguishes transport validation from reusable application behavior, and `dotnet-modular-architecture.knowledge.md` distinguishes a narrow application contract from a cross-module repository dependency.

```csharp
// Good: application collaboration depends on a narrow contract.
public interface I<Capability>LookupService
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}

// Bad: application orchestration depends on another module's persistence detail.
public Create<ModuleName>Handler(IOtherModuleRepository repository)
{
}
```

Add an example only when prose alone could produce an incorrect implementation or review decision. Use `<ModuleName>` for reusable patterns, ground the example in verified repository symbols, and keep index, routing, and maintenance topics snippet-free unless the snippet is necessary to preserve the rule.

## Main Workflow

1. Start with `KnowledgeBase/INDEX.md`; identify the decision trigger and the existing authoritative topic before changing a skill or knowledge artifact.
2. Assign ownership: keep triggers, workflow, gates, and outputs in the skill; keep reusable decision guidance in knowledge; route project facts to `AGENTS.md`, project knowledge, or ADRs.
3. Update the smallest authority. Do not duplicate API/application guidance, EF Core guidance, project persistence facts, or ADR rules in a generic architecture skill or topic.
4. Preserve self-sufficiency: retain the procedure, high-risk boundary, verification evidence, and local-discovery boundary needed for first-pass use.
5. Add or retain an example only where it prevents a concrete boundary error. Keep it concise, source-grounded, and reusable.
6. When module ownership or cross-module behavior matters, retrieve `resumeenhancer-architecture-routing.knowledge.md`, then the governing ADR. State the ADR status; do not create a competing summary.
7. If a linked skill has a source template, update the active skill and source body together, or record the mismatch as a blocker. Then verify every link, reference, and generated-body expectation.

## Rules And Invariants

- **Index first:** every backend or architecture skill change preserves selective retrieval through `KnowledgeBase/INDEX.md`.
- **One authority per decision:** generic knowledge does not restate project APIs, persistence conventions, or ADR decisions.
- **ADR retrieval, not duplication:** a skill or routing topic names the governing ADR and preserves its status instead of copying its rules.
- **Examples are exceptional:** use good/bad pairs only for a high-risk boundary mistake; do not turn maintenance knowledge into a code inventory.
- **Local discovery stays explicit:** inspect changed or volatile facts, including contracts, handlers, entities, queries, registrations, tests, consumers, migration state, authority status, and requirement conflicts.
- **Source bodies remain aligned:** a synchronization source must not silently reintroduce retired references or replace an active skill with a stale workflow.

## Extension Pattern

Use this sequence when adding or revising a backend or architecture skill:

1. Read the active `SKILL.md`, its `agents/openai.yaml` metadata when present, and its routed knowledge authorities.
2. Classify each proposed instruction as a trigger/workflow/gate/output, reusable guidance, project fact, or ADR rule.
3. Put it in the owning artifact. Prefer linking an existing knowledge authority over copying it.
4. Add a concise example only if an observed high-risk mistake would otherwise remain ambiguous.
5. Update `KnowledgeBase/INDEX.md` only when the revised topic is authoritative for a distinct decision trigger.
6. Inspect `.codex/tools/sync-linked-skill-bodies.ps1`, `seed-project-skills.ps1`, and `complete-linked-skills.ps1` when the skill has a generated or synchronized source body.
7. Verify the active body, source body, links, and index entries agree before packaging the change.

## Verification And Testing

This topic governs documentation and instruction artifacts. Use structural verification rather than claiming application runtime behavior:

```powershell
Get-ChildItem -Recurse .codex, Prompts
git diff -- AGENTS.md .codex Prompts
rg -n "<changed knowledge filename>" KnowledgeBase .codex AGENTS.md
git diff --check
```

For an active skill with synchronized source bodies, compare the generated or source text to the active `SKILL.md`. Run the provided synchronization or completion script only when its documented behavior and write scope have been reviewed.

## Pitfalls And Boundaries

- Do not restore project-specific types, commands, helpers, or retired references to reusable skills.
- Do not use a generic topic as a parallel source of truth for `resumeenhancer-api-application-delivery.knowledge.md`, `persistence-project.knowledge.md`, or an ADR.
- Do not remove an authority without first moving its unique policy and updating every index or skill reference.
- Do not assume a source template is current merely because it exists; compare it to the active body before relying on synchronization.
- Do not apply distributed-architecture review merely because a change is broad. Preserve the `architect-review` review-mode gate for remote boundaries, asynchronous messaging, independent deployment, eventual consistency, resilience, or distributed observability.

## Clarifications

- This topic applies only while maintaining backend and architecture skills.
- It is intentionally one compact policy document, not a group of linked maintenance topics.
- It uses concise policy prose and high-risk good/bad code examples.
- Each retained section must be self-sufficient for first-pass use.
