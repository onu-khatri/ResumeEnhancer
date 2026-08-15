---
name: us-kickoff
description: Turn approved ResumeEnhancer user stories into coordinated, review-ready implementation work. Assess readiness, resolve dependencies, classify delivery shape, plan isolated branches or worktrees, assign focused agents, gate on human approval, and track each story to PR-ready. Use when the user asks to kick off, start, or parallelize user stories that are sliced and ready to build.
---

# US Kickoff

Use this skill to convert approved story slices into sequenced, isolated, verifiable workstreams without losing readiness discipline or architectural boundaries. It is the entry point that `story-orchestrator` and parallel workstreams run through.

## Story file anatomy

Stories live in `User-Stories/` as a trio per slice:

- `<epic>.<n> <slug>.US.md` — the story (acceptance criteria, rules, DoD).
- `<epic>.<n> <slug>.SI.md` — supporting information (components, wireframes, state matrix, edge cases, test focus).
- `<epic>.<n> <slug>.Research.md` — competitor/product research evidence.

The `.US.md` frontmatter carries the delivery state and must be kept current:

```yaml
id: RES-BE-001
title: <story title>
status:            # Ready_To_Implement | In_Progress | Blocked | PR_Open | Done
branch:            # codex/<story-id>-<slug>-<timestamp>
worktree_path:     # .worktrees/<story-id>-<slug>
base_branch: main  # normalize from `master` to the repo's actual default
pr_url:
is_architectural:  # true when the story changes module boundaries/contracts
approach_summary:
created:
updated:
```

## Readiness assessment

Before planning, verify each candidate story against its `Definition Of Ready For Engineering` section and these gates:

- `status` is `Ready_To_Implement` (or approved equivalent).
- Every `Dependency:` reference is either resolved or has a sequenced owner.
- Cross-layer contracts and schemas referenced by the story are approved.
- Acceptance criteria are concrete and testable.
- The story has a clear delivery shape (frontend, backend, full-stack, architecture, or research).

If a story fails a gate, do not force it into a workstream; report it as not ready with the missing prerequisite.

## Workflow

### 1. Load context

Read the selected `.US.md`, `.SI.md`, and `.Research.md` files plus any linked `Business-Requirements/*.BR.md`.

### 2. Resolve dependencies

Build a dependency graph from each story's `Dependency:` field (e.g., frontend `RES-FE-001` depends on backend `RES-BE-001`). Topologically order the slices; mark cross-layer frontend/backend pairs so the backend slice lands first.

### 3. Classify delivery shape

- **backend** — Minimal APIs, contracts, handlers, EF, migrations (agent: `backend-implementer`).
- **frontend** — React feature UIs, forms, hooks, API integration (agent: `frontend-implementer`).
- **full-stack** — coordinated frontend + backend with a shared contract (agent: `story-orchestrator` owning the contract lane).
- **architecture** — boundaries, ADRs, composition changes (`is_architectural: true`).
- **research** — evidence gathering / knowledge building (agent: `knowledge-researcher`).

### 4. Analyze conflict risk

Identify shared files and cross-cutting surfaces before parallelizing:
- AM request/response contracts and validators
- EF migrations and seed data
- shared UI primitives, router, and app shell
- `ModulesComposition` and module registration

### 5. Plan parallel groups

Group independent, non-conflicting slices into parallel lanes. Each lane gets an isolated branch or worktree and one focused agent. Keep any shared-contract or migration work in a single coordinating lane.

### 6. Human approval checkpoint

Stop and present a short plan for explicit approval before any branch, worktree, or parallel execution:

- dependency order and parallel groups
- per-story agent, branch/worktree, and scope
- conflict risk and which stories are sequenced (not parallel)
- any story that is not ready and why

### 7. Execute and track

After approval:
- create a `codex/<story-id>-<slug>-<timestamp>` branch or `.worktrees/<story-id>-<slug>` worktree (see `git-worktrees`)
- update the story frontmatter (`status`, `branch`, `worktree_path`, `updated`)
- assign the focused agent and hand it the story file(s) plus the relevant skills

### 8. Report and close

Each workstream reports back: touched areas, verification commands run, blockers, PR readiness. Update the frontmatter (`status` -> `PR_Open`, `pr_url`) when a PR is opened, and `Done` only after merge and verification.

## Definition of Done per workstream

- Backend: `dotnet build application\ResumeEnhancerApp.slnx` and the relevant `dotnet test` project pass.
- Frontend: `npm run check` and `npm run build` pass in the client.
- Contract, migration, or shared-file conflicts are reported before merge.
- Each story ends PR-ready with story traceability and verification notes in the PR.

## When to simplify

Do not parallelize when:
- stories share the same contract, migration, or shared UI surface
- the dependency chain is strictly serial
- the overhead of isolation outweighs the parallelism benefit

For a serial chain, drive one story at a time through `full-stack-feature-orchestrator`.

## Reference

Read `references/kickoff-playbook.md` for the status model, approval-summary template, and workstream reporting format.
