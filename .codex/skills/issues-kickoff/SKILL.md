---
name: issues-kickoff
description: When explicitly invoked, find up to five ready-to-implement ResumeEnhancer GitHub issues through MCP, collect an optional search brief, and coordinate selected issue implementation through PR readiness.
---

# Issues Kickoff

Use this skill only when the user explicitly invokes it. It is not an automatic continuation of `$create-github-issue` or `$us-kickoff`. It starts with GitHub issue intake, then executes implementation only for the issue number(s) the user selects. It does not author stories or create the initial GitHub issue handoff. OpenSpec coordination is mandatory for implementation; this skill never assigns implementation agents directly. Hand approved issue work to `$openspec-workflow` for proposal, approval, worktree implementation, verification, and closeout.

## Issue intake

### 1. Collect the search brief

Use any issue-selection information supplied in the invocation, such as capability, story ID, issue number, keywords, labels, state, assignee, milestone, or desired delivery scope. The current repository is the default repository selector; verify it locally before calling GitHub.

If no search brief was supplied, ask one focused question before showing any issue list: request the issue number(s), or offer the default discovery search. The issue number is a blocker for implementation, not something to infer from a result or replace with a title. If the user chooses discovery, use the default filter below. If the user provides search criteria, use those criteria instead of guessing terms, labels, priority, state, or implementation scope. The current repository remains the default repository selector.

The default discovery search must find only open issues tagged `ready-to-implement`, excluding issues tagged `on-hold`, `qa-testing`, `blocked`, `rejected`, `done`, or `closed`, and return at most 5 issues. Offer the user the option to provide narrower criteria such as capability, story ID, keywords, labels, assignee, milestone, state, or desired delivery scope for a more accurate search. Invoke `$user-interview` only when the custom brief requires a material multi-step decision; otherwise ask one focused question at a time using the host structured question UI when available and the chat fallback otherwise.

Do not persist interview state for a compact one-question exchange. If the interview becomes multi-step or is interrupted, follow `$user-interview` session-state rules under `tmp/user-interview/<session-id>/state.md`.

### 2. Fetch the top 5 issues through GitHub MCP

After the search brief is known, resolve the current repository to GitHub `owner/name` and call the GitHub MCP `github_search_issues` operation with `topn: 5`:

- the user-derived query and repository selector;
- the user-specified state/filter values when provided;
- a user-specified sort when provided, otherwise the most relevant stable sort for the request; and
- the default `ready-to-implement` filter when the user did not provide custom criteria.

Do not substitute local issue files, Git log output, or a guessed GitHub result for the MCP response. If GitHub MCP is unavailable, report the blocker and do not create branches or modify code.

### 3. Present the issue list and require issue-number selection

Present all returned issues with number, title, state, labels, updated/created date, story ID when present, and the issue URL. Explain the applied search brief and whether fewer than 5 results were returned.

If the user supplied exact issue numbers and the results unambiguously contain them, retain those as the requested scope. Otherwise ask the user for the issue number(s) to kick off. Do not begin implementation, create branches, create worktrees, invoke `$openspec-orchestrator`, or update story status until at least one valid issue number is explicitly supplied. Do not ask for a separate approval; issue-number selection is the scope gate.

## Implementation entry criteria

For every selected issue, verify:

- the issue is open and has a stable issue number and URL;
- the corresponding source `.US.md` exists and has `status: Move_To_GitHub_Issue`;
- the source story contains a `## GitHub Issues` row matching the selected issue's canonical URL and number;
- the issue has a stable story ID or slice label and pick-order/dependency references;
- matching `.SI.md` and `.Research.md` files are available when present; and
- the source story meets its `Definition Of Ready For Engineering` with no unresolved blocking decision. If any Definition-of-Ready requirement is unclear, unapproved, or not evidenced by the story pack or repository, invoke `$user-interview` before deciding whether the issue is blocked or ready for handoff.

If an entry criterion fails, stop that issue and report the missing evidence. Do not bypass the GitHub handoff status or implement directly from an untracked draft.

### Definition-of-Ready interview

When the Definition of Ready is unclear or its approval/evidence cannot be established from repository sources, hand the unresolved decisions to `$user-interview` before stopping the issue. The interview must:

- frame the known story, repository evidence, missing approval, and implementation decision;
- ask one focused question at a time using the host structured question UI when available;
- distinguish user-provided decisions from repository evidence and inferences;
- record or update only the compact decision state required for this kickoff when the interview becomes multi-step or is interrupted; and
- return the confirmed decision, remaining gaps, and exact next action to `issues-kickoff`.

Do not infer approval from the `ready-to-implement` label, issue selection, continued conversation, or the user's request to retry. If the interview resolves the gap, continue entry checks and pass the decision evidence to `$openspec-workflow`; if it does not, report the unresolved owner and blocking decision without creating a branch, worktree, OpenSpec change, or implementation handoff.

## Workflow

### 4. Load implementation context

Read `AGENTS.md`, `KnowledgeBase/INDEX.md`, the selected source story pack, and the linked GitHub issue. Read only the knowledge topics and specialist skill references relevant to the delivery shape. Reconcile the issue URL/number and scope against the story's `## GitHub Issues` register and body; if the register is missing or the issue and local story materially disagree, stop and report the mismatch before changing code.

### 5. Resolve issue order and dependencies

Use the issue `Pick order`, `Depends on`, `Blocks`, and `Related to` sections plus the story `Depends on` fields to build a topological delivery graph. Implement prerequisites before dependents. Keep independent issues in parallel groups only when they do not share contracts, migrations, composition, shared UI primitives, or other conflict-heavy files. `Depends on` is authoritative for prerequisites; `Blocks` must be its inverse, `Related to` never orders work, and contradictory, cyclic, or unavailable references block execution.

If references are missing, contradictory, cyclic, or point to an unavailable issue, stop and report the exact dependency problem. Do not invent an order.

### 6. Prepare the OpenSpecWorkflow handoff

Do not create a branch, worktree, OpenSpec change, or implementation handoff until the issue-number gate has passed. Then pass the selected issue, canonical URL, source story pack, dependency graph, requested scope, repository evidence, and current OpenSpec/branch/worktree state to `$openspec-workflow`.

`$openspec-workflow` owns proposal creation/update in the main checkout, strict validation, explicit user approval, worktree creation immediately before implementation, worktree-local OpenSpec coordination, implementation routing, verification, PR delivery, and closeout. It must not create or reuse a worktree during issue intake, interview, readiness review, or proposal-only work. It must pass explicit proposal, Definition-of-Ready, approval, issue/change, base-branch, and owner evidence to `$git-worktrees`, and must ask whether to create one at the implementation handoff unless the user already confirmed that complete handoff.

Do not continue when OpenSpecWorkflow reports `BLOCKED`; report the exact missing decision, evidence, dependency, approval, or capability.

### 7. Classify implementation ownership for the OpenSpecWorkflow handoff

- **backend** — Minimal APIs, contracts, handlers, EF, migrations: `backend-implementer`.
- **frontend** — React feature UI, forms, hooks, and API integration: `frontend-implementer`.
- **full-stack** — coordinated frontend/backend with a shared contract: `story-orchestrator` owns the contract lane.
- **architecture** — boundaries, ADRs, composition: `story-orchestrator` with `architect-review`.
- **research** — evidence or knowledge work: `knowledge-researcher`.

Select one primary implementation owner per issue and pass that ownership to `$openspec-workflow`. `issues-kickoff` does not invoke the owner or any implementation agent. Load specialist skills only when their trigger applies; they return constraints or findings to the OpenSpec-coordinated owner and do not duplicate implementation.

### 8. Identify conflicts before execution

Check for shared:

- AM contracts, validators, mapping, or API response shapes;
- EF migrations, schema, seed data, or persistence adapters;
- `ResumeEnhancer.WebSolution.ModulesComposition` and module registration;
- shared UI primitives, router, app shell, or theme tokens; and
- tests or fixtures used by multiple slices.

Keep shared-contract and migration ownership in one coordinating lane. Do not parallelize conflicting issues merely because their GitHub issues are separate.

### 9. Issue-number gate before handoff

Before handing off, verify that the user explicitly supplied at least one valid issue number from the presented results. Resolve dependency order, delivery shape, ownership, conflict risks, and verification commands from the selected issue and repository evidence without asking for a separate approval. Do not ask the user to create a worktree here; OpenSpecWorkflow asks at the implementation handoff after proposal approval.

### 10. Hand off the selected issue

Hand the selected issue, source story pack, dependency order, implementation ownership, conflict assessment, requested scope, and repository evidence to `$openspec-workflow`. That skill owns proposal approval, worktree creation, worktree-local OpenSpec coordination, implementation, verification, PR delivery, and closeout. It must report status transitions (`In_Progress`, `Blocked`, `PR_Open`, `Done`) and update story tracking in the appropriate existing worktree.

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
