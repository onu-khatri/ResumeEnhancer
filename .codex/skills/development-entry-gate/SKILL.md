---
name: development-entry-gate
description: Gate any ResumeEnhancer development-related work before code changes by reconstructing current state, classifying the delivery shape, and routing the minimum backend, frontend, architecture, security, research, and verification skills. Do not use it for documentation-only work.
license: MIT
---

# Development Entry Gate

Act as a resumable, evidence-first state-machine gate for code, tests, configuration, migrations, API changes, React changes, refactoring, and architecture-sensitive implementation in ResumeEnhancer. It is a routing and readiness gate, not an implementation owner.

Do not infer hidden progress. Reconstruct the current state from the request, repository, `AGENTS.md`, `KnowledgeBase/`, story/issue or OpenSpec artifacts when present, Git status, existing branches/worktrees, and prior verification evidence. Continue from the first incomplete or invalid gate; do not repeat a completed gate only because the skill was invoked again.

## Inputs

Accept a plain-language development request, a selected GitHub issue and source story pack, an existing OpenSpec change, a review-fix request, or a continuation of work already in progress. Derive non-sensitive context from local artifacts before asking. Ask only when a required product or engineering decision cannot be resolved safely; route that question through `$user-interview` when applicable.

The gate must finish before an implementation agent, branch, worktree, or code edit is started. It may inspect files and Git state, but it does not modify product code, create branches, create worktrees, invoke implementation agents, or claim that checks passed.

## Gate A: establish the request and evidence

Read `AGENTS.md` and `KnowledgeBase/INDEX.md` first. For issue-driven work, also read the selected GitHub issue and its source story pack. For direct development requests, identify the stated outcome, acceptance behavior, target area, and requested delivery boundary.

Inspect the current implementation and closest tests before routing. Check `git status --short` and preserve unrelated user changes. Resolve the actual project, client, route, module, test-project, and configuration paths from the checkout; do not rely on stale paths or pasted stack assumptions.

For resumed work, also inspect existing change tasks, branch/worktree identity, changed files, review feedback, and the latest checks. Do not create duplicate plans, routes, branches, or worktrees as part of this gate.

Record:

- requested outcome and acceptance evidence;
- affected layers, modules, routes, contracts, persistence, and tests;
- existing behavior and closest implementation pattern;
- dependency, migration, shared-contract, shared-UI, and worktree conflicts;
- verification commands appropriate to the affected area; and
- unresolved product, audience, entitlement, security, contract, architecture, or rollout decisions.

If a material decision cannot be answered from the request, repository, story, or authoritative linked evidence, route `$user-interview` before implementation. If the evidence gap is significant or cross-cutting, route `$deep-research` first. Do not invent a decision merely to pass the gate.

## Gate B: classify the delivery shape

Select exactly one primary shape unless a coordinating full-stack lane is required:

- **backend** — API, request/response contracts, validation, handlers, domain logic, persistence, migrations, seed data, or backend tests.
- **frontend** — React/TypeScript routes, components, forms, hooks, state, client API integration, accessibility, or frontend tests.
- **full-stack** — one outcome crosses the frontend and backend or changes a shared API contract.
- **architecture** — dependency direction, module composition, bounded-context ownership, ADR, or a quality-attribute decision is the main deliverable.
- **research/planning** — implementation cannot safely start until evidence or a plan is produced.

Use the smallest shape that fully covers the requested behavior. A test-only or refactoring request still uses the shape of the production boundary it protects.

## Gate C: route skills by trigger

Return a concrete ordered skill route. The first item is the primary workflow; later items are bounded specialist inputs, not competing implementation owners.

### Backend

For backend shape, route `$dotnet-backend-patterns` and `$backend-feature-development`. Also route:

- `$ef-core-database-architect` when queries, entities, repositories, schema, migrations, seed data, initialization, or transactions change;
- `$backend-security-coder` or `$security-manager` when authentication, authorization, ownership, file handling, exports, privacy, quotas, entitlements, sensitive data, or another trust boundary changes;
- `$architect-review` or `$dotnet-architect` when module boundaries, composition, dependencies, integration, or quality attributes are materially affected;
- `$domain-driven-design` when business language, invariants, aggregate ownership, or bounded contexts are unclear; and
- `$performance-optimization` only for a measured or concrete query, throughput, rendering, bundle, network, or responsiveness problem.

### Frontend

For frontend shape, route `$frontend-developer` and `$frontend-dev-guidelines`. Follow [frontend workflow routing](../frontend-dev-guidelines/references/frontend-workflow-routing.md). Select `$frontend-design` for missing or deliberately changing visual/interaction direction, `$production-ui-generator` only for an explicitly UI-dominant implementation, `$react-patterns` for a concrete unresolved React/TypeScript pattern, `$frontend-security-coder` for auth, redirects, untrusted content, sensitive data, or browser trust boundaries, `$performance-optimization` for a measured performance problem, and `$design-review` for an explicit visual critique or pre-ship review.

There is one frontend implementation owner. Do not route both `$frontend-developer` and `$production-ui-generator` as concurrent primary owners. Specialists return constraints or findings to the owner.

### Full-stack and architecture

For full-stack shape, route `$full-stack-feature-orchestrator` as the coordinating workflow, with the applicable backend and frontend skills as bounded implementation guidance. Keep shared contracts, migrations, composition, and synthesis in one coordinating lane.

For architecture shape, route `$architect-review` and/or `$dotnet-architect`; add `$architecture-decision-records` when a durable decision is required and `$domain-driven-design` when domain boundaries or invariants are part of the decision.

For research/planning shape, route `$deep-research`, `$user-interview`, `$plan-writing`, or the relevant story/OpenSpec workflow only when its trigger is present. Do not start production implementation while the gate is research/planning-only.

## Gate D: decide readiness

Return `PASS` only when the requested behavior, ownership, dependencies, required specialist route, implementation owner, and proportionate verification are clear, with no unresolved blocking decision. Return `BLOCKED` when evidence, approval, dependency, issue/story traceability, or a required capability is missing. Include the exact missing item and the next safe action.

The gate does not replace approval checkpoints owned by `$issues-kickoff`, `$us-kickoff`, `$openspec-workflow`, OpenSpec, or the user. For issue execution, a PASS is necessary but not sufficient: `$openspec-workflow` must still obtain proposal approval and the user's worktree-creation confirmation before a worktree or implementation agent is created.

## Required output

Use this compact handoff:

```text
Gate: PASS | BLOCKED
Outcome: <requested behavior>
Shape: <backend | frontend | full-stack | architecture | research/planning>
Primary workflow: <$skill-name>
Specialist route: <ordered skills, or none>
Affected ownership: <modules, layers, routes, contracts, persistence, tests>
Decisions: <confirmed decisions and explicit assumptions>
Conflicts/dependencies: <none or exact details>
Verification: <commands and boundary coverage>
Next action: <one safe action; for PASS, hand off to the approved implementation workflow>
```

Never report implementation, agent delegation, branch creation, or verification success from this gate. Those belong to the downstream workflow that actually performs them.

## Resume and failure rules

- Preserve resumable local state and leave user changes untouched.
- If the failure is caused by missing evidence, a user-owned decision, unavailable capability, dependency, or permission, return `BLOCKED` with the exact next action; do not bypass it by weakening requirements or checks.
- If a reversible local inspection issue can be fixed without expanding scope, fix it and re-run the affected gate; otherwise stop with the blocker.
- After any downstream mutation is reported back to this gate, re-read enough state to confirm it before routing the next transition.
- A specialist route is advisory input to the primary workflow. Never create a second implementation lane merely because multiple skills are selected.
