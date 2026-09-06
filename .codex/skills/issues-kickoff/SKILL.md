---
name: issues-kickoff
description: When explicitly invoked, find the top 10 matching ResumeEnhancer GitHub issues through MCP, confirm the user’s scope, and coordinate approved implementation work through PR readiness.
---

# Issues Kickoff

Use this skill only when the user explicitly invokes it. It is not an automatic continuation of `$create-github-issue` or `$us-kickoff`. It starts with GitHub issue intake, then executes implementation only for the issues the user selects and approves. It does not author stories or create the initial GitHub issue handoff. OpenSpec coordination is mandatory for implementation; this skill never assigns implementation agents directly.

## Issue intake

### 1. Collect the search brief

Use any issue-selection information supplied in the invocation, such as capability, story ID, issue number, keywords, labels, state, assignee, milestone, or desired delivery scope. The current repository is the default repository selector; verify it locally before calling GitHub.

If the user provides no usable selection criteria, invoke `$user-interview` before GitHub search. Ask one focused question at a time, using the host structured question UI when available and the skill’s chat fallback otherwise. At minimum, resolve the capability/outcome or story/issue identifiers that define which work the user wants to see. Do not guess search terms, labels, priority, state, or implementation scope.

Do not persist interview state for a compact one-question exchange. If the interview becomes multi-step or is interrupted, follow `$user-interview` session-state rules under `tmp/user-interview/<session-id>/state.md`.

### 2. Fetch the top 10 issues through GitHub MCP

After the search brief is known, resolve the current repository to GitHub `owner/name` and call the GitHub MCP `github_search_issues` operation with:

- the user-derived query and repository selector;
- the user-specified state/filter values when provided;
- a user-specified sort when provided, otherwise the most relevant stable sort for the request; and
- `topn: 10`.

Do not substitute local issue files, Git log output, or a guessed GitHub result for the MCP response. If GitHub MCP is unavailable, report the blocker and do not create branches or modify code.

### 3. Present and confirm the issue scope

Present all returned issues with number, title, state, labels, updated/created date, story ID when present, and the issue URL. Explain the applied search brief and whether fewer than 10 results were returned.

If the user named exact issue numbers and the results unambiguously contain them, retain those as the requested scope. Otherwise ask the user to select the issue number(s) to kick off and confirm whether independent issues may run in parallel. Do not begin implementation merely because search returned results.

## Implementation entry criteria

For every selected issue, verify:

- the issue is open and has a stable issue number and URL;
- the corresponding source `.US.md` exists and has `status: Move_To_GitHub_Issue`;
- the source story contains a `## GitHub Issues` row matching the selected issue's canonical URL and number;
- the issue has a stable story ID or slice label and pick-order/dependency references;
- matching `.SI.md` and `.Research.md` files are available when present; and
- the source story meets its `Definition Of Ready For Engineering` with no unresolved blocking decision.

If an entry criterion fails, stop that issue and report the missing evidence. Do not bypass the GitHub handoff status or implement directly from an untracked draft.

## Workflow

### 4. Load implementation context

Read `AGENTS.md`, `KnowledgeBase/INDEX.md`, the selected source story pack, and the linked GitHub issue. Read only the knowledge topics and specialist skill references relevant to the delivery shape. Reconcile the issue URL/number and scope against the story's `## GitHub Issues` register and body; if the register is missing or the issue and local story materially disagree, stop and report the mismatch before changing code.

### 5. Resolve issue order and dependencies

Use the issue `Pick order`, `Depends on`, `Blocks`, and `Related to` sections plus the story `Depends on` fields to build a topological delivery graph. Implement prerequisites before dependents. Keep independent issues in parallel groups only when they do not share contracts, migrations, composition, shared UI primitives, or other conflict-heavy files. `Depends on` is authoritative for prerequisites; `Blocks` must be its inverse, `Related to` never orders work, and contradictory, cyclic, or unavailable references block execution.

If references are missing, contradictory, cyclic, or point to an unavailable issue, stop and report the exact dependency problem. Do not invent an order.

### 6. Resolve OpenSpec coordination before ownership

Before creating branches, worktrees, or implementation handoffs, check for an active matching OpenSpec change. If one exists, reconcile its name, tasks, scope, and branch with the selected issue. If none exists, invoke `$openspec-orchestrator` to establish the required OpenSpec planning state. Pass the issue, source story pack, dependency result, requested scope, and repository evidence already loaded. Require its stateful handoff and `PASS`/`BLOCKED` readiness result.

Do not continue an issue when the orchestrator is `BLOCKED`; report the exact missing decision, evidence, dependency, or capability. A `PASS` does not replace the approval checkpoint in step 9.

Use the OpenSpec orchestrator's delivery shape and ordered specialist route to prepare the implementation handoff. It coordinates the handoff but does not replace the approval checkpoint or the assigned implementation owner's code responsibility.

### 7. Classify implementation ownership for the OpenSpec handoff

- **backend** — Minimal APIs, contracts, handlers, EF, migrations: `backend-implementer`.
- **frontend** — React feature UI, forms, hooks, and API integration: `frontend-implementer`.
- **full-stack** — coordinated frontend/backend with a shared contract: `story-orchestrator` owns the contract lane.
- **architecture** — boundaries, ADRs, composition: `story-orchestrator` with `architect-review`.
- **research** — evidence or knowledge work: `knowledge-researcher`.

Select one primary implementation owner per issue and pass that ownership to `$openspec-orchestrator`. `issues-kickoff` does not invoke the owner or any implementation agent. Load specialist skills only when their trigger applies; they return constraints or findings to the OpenSpec-coordinated owner and do not duplicate implementation.

### 8. Identify conflicts before execution

Check for shared:

- AM contracts, validators, mapping, or API response shapes;
- EF migrations, schema, seed data, or persistence adapters;
- `ResumeEnhancer.WebSolution.ModulesComposition` and module registration;
- shared UI primitives, router, app shell, or theme tokens; and
- tests or fixtures used by multiple slices.

Keep shared-contract and migration ownership in one coordinating lane. Do not parallelize conflicting issues merely because their GitHub issues are separate.

### 9. Approval checkpoint

Before creating branches, worktrees, or handing work to `$openspec-orchestrator`, present and obtain explicit approval for:

- dependency order and parallel groups;
- issue, story ID, delivery shape, owner, and scope per lane;
- branch/worktree names and base branch;
- conflict risks and the coordinating lane; and
- verification commands and remaining readiness gaps.

### 10. Create isolated implementation work

After approval, create one isolated branch or worktree per non-conflicting issue, before the OpenSpec implementation handoff:

- branch: `openspec/gh-<issue-number>-<short-kebab-slug>`;
- worktree: `.worktrees/gh-<issue-number>-<short-kebab-slug>`; and
- base branch: the repository's actual default branch, normalized in the story frontmatter.

Update only the source story frontmatter needed for implementation tracking (`status: In_Progress`, `branch`, `worktree_path`, `base_branch`, `updated`). Preserve the story body and unrelated changes.

### 11. Implement and verify

Hand each prepared worktree, issue, source story pack, OpenSpec change, task state, issue references, ownership boundaries, and relevant skills to `$openspec-orchestrator`. Require the coordinator to report touched areas, acceptance-criteria coverage, verification commands actually run, blockers, and PR readiness.

Use proportionate project checks:

- backend: `dotnet build application\\ResumeEnhancerApp.slnx` and the relevant unit/integration test project;
- frontend: `npm run check` and `npm run build` in the client; and
- architecture or cross-cutting work: applicable architecture guards, focused tests, and review evidence.

Do not claim a check passed unless it ran successfully. Keep source issue references in commits and PR descriptions.

### 12. Track implementation status

- `In_Progress` — implementation is active.
- `Blocked` — a dependency, decision, environment, or approval prevents progress; record the blocker.
- `PR_Open` — branch is pushed and a hosted PR was created; record and verify `pr_url`.
- `Done` — merged and verification is complete.

Update the source story frontmatter at each transition. Do not mark `PR_Open` from a local draft or `Done` before merge and verification.

## Completion report

Report per issue:

- story ID, issue number, and URL;
- dependency order and implementation lane;
- owner, branch, and worktree;
- touched areas and contract/migration conflict status;
- verification commands and actual results;
- blockers or residual risks; and
- PR URL or the remaining work to reach PR readiness.

Read `us-kickoff/references/kickoff-playbook.md` for the shared status vocabulary and handoff conventions. Use `git-worktrees`, `full-stack-feature-orchestrator`, and the relevant implementation skills when their triggers apply. `$create-github-issue` and `$us-kickoff` may hand work to GitHub, but neither automatically invokes this skill.
