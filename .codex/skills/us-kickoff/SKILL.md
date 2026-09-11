---
name: us-kickoff
description: Prepare approved ResumeEnhancer user stories for GitHub issue handoff. Assess readiness, resolve dependencies, classify delivery shape, and invoke create-github-issue; use issues-kickoff for implementation execution.
---

# US Kickoff

Use this skill to validate approved story slices and hand them off to GitHub in a traceable order without losing readiness discipline or architectural boundaries. It does not create branches, worktrees, implementation agents, or PRs; `issues-kickoff` owns that implementation phase.

## Story file anatomy

Stories live in `User-Stories/` as a trio per slice:

- `<epic>.<n> <slug>.US.md` — the story (acceptance criteria, rules, DoD).
- `<epic>.<n> <slug>.SI.md` — supporting information (components, wireframes, state matrix, edge cases, test focus).
- `<epic>.<n> <slug>.Research.md` — competitor/product research evidence.

After GitHub handoff, the `.US.md` also contains a `## GitHub Issues` register with one row per created issue: issue title and canonical link, created date, and issue number. This is the durable story-to-issue traceability record.

The `.US.md` frontmatter carries the delivery state and must be kept current:

```yaml
id: RES-BE-001
title: <story title>
status:            # Ready_To_Implement | Move_To_GitHub_Issue | In_Progress | Blocked | PR_Open | Done
branch:            # openspec/gh-<issue-number>-<short-kebab-slug>
worktree_path:     # .worktrees/gh-<issue-number>-<short-kebab-slug>
base_branch: main  # normalize from `master` to the repo's actual default
pr_url:
is_architectural:  # true when the story changes module boundaries/contracts
approach_summary:
created:
updated:
```

## Readiness assessment

Before planning, verify each candidate story against its `Definition Of Ready For Engineering` section and these gates:

- `status` is `Ready_To_Implement`; `approved` is narrative readiness only, not a lifecycle value.
- Every `Depends on` reference is either resolved or has a sequenced owner.
- Cross-layer contracts and schemas referenced by the story are approved.
- Acceptance criteria are concrete and testable.
- The story has a clear delivery shape (frontend, backend, full-stack, architecture, or research).

If a story fails a gate, do not force it into a workstream; report it as not ready with the missing prerequisite.

## Workflow

### 1. Load context

Read the selected `.US.md`, `.SI.md`, and `.Research.md` files plus any linked `Business-Requirements/*.BR.md`.

### 2. Resolve dependencies and handoff order

Build a dependency graph from each story's `Depends on` field (legacy `Dependency:` is accepted only as an input alias). Topologically order the slices; mark cross-layer frontend/backend pairs so the backend slice is handed off first. Preserve `Depends on`, `Blocks`, `Related to`, and `Pick order` references in the resulting GitHub issues.

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
- `ResumeEnhancer.WebSolution.ModulesComposition` and module registration

### 5. Prepare the handoff plan

State which stories remain one issue and which require multiple issues. For a split, document pick order, dependencies, blocking relationships, and related references. Keep shared-contract or migration work in one sequenced issue when the story evidence requires it.

### 6. Human approval checkpoint

Stop and present a short plan for explicit approval before any GitHub issue creation:

- dependency/pick order and proposed issue splits
- per-story or per-slice scope and delivery shape
- conflict risk and which stories are sequenced (not parallel)
- any story that is not ready and why

### 7. Create the GitHub issue handoff

After approval, invoke `$create-github-issue` for each approved story in dependency/pick order. That skill decides whether a story must be split, creates and verifies the issue set, writes the `## GitHub Issues` register with each issue's title/link, created date, and number, and changes the source status to `Move_To_GitHub_Issue` only after successful handoff.

If issue creation, duplicate detection, reference reconciliation, or source status update fails, stop and report the exact handoff state. Do not start implementation from this skill.

### 8. Report and continue

Report each story's readiness result, issue number/URL set, pick order, dependencies, source status, and confirmation that the source story's `## GitHub Issues` register was updated. Once the stories are in `Move_To_GitHub_Issue`, stop after the handoff; implementation begins only when the user explicitly invokes `$issues-kickoff`.

## Boundary with issues-kickoff

`us-kickoff` owns story readiness, dependency analysis, issue splitting guidance, approval, and GitHub handoff. `issues-kickoff` owns explicit user-invoked issue intake, top-10 GitHub search, scope confirmation, branch/worktree creation, implementation-agent assignment, code changes, tests/builds, PR creation, and implementation status transitions.

## Reference

Read `references/kickoff-playbook.md` for the handoff status model and approval-summary template. Use `$create-github-issue` for issue content, split decisions, issue references, and source-status updates.

