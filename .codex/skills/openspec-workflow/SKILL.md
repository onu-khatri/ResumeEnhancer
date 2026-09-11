---
name: openspec-workflow
description: Coordinate ResumeEnhancer OpenSpec proposal, approval, worktree implementation, verification, and closeout with proposal work in the main checkout and code work isolated to the approved worktree.
---

# OpenSpecWorkflow

Use this skill for OpenSpec proposal work, proposal approval, implementation handoff, implementation continuation, verification, synchronization, archiving, and change closeout. It is the OpenSpec lifecycle authority. `$issues-kickoff` supplies issue intake and readiness evidence; `$openspec-orchestrator` and the OpenSpec skills supply the stateful planning and implementation mechanics.

## When to Use

- More than one active OpenSpec change exists and the correct change must be selected or coordinated.
- `$openspec-apply-change` was invoked without an explicit change target.
- Multiple OpenSpec changes need to be implemented in parallel and their worktrees, ownership, or shared-contract conflicts must be coordinated.

## Do Not Use

Do not use this skill when `$openspec-apply-change` explicitly targets one change. In that case, let the targeted apply workflow own change selection and task execution, while still following its required approval, worktree, gate, and repository instructions.

## Governing rules

- Proposal artifacts are created or updated in the main checkout.
- Production code, tests, migrations, configuration, and implementation task updates are performed in the issue/change worktree only.
- A worktree is created only after the proposal is validated, the user has approved it, and implementation is about to begin.
- The worktree-creation handoff must carry explicit evidence of proposal validation, Definition-of-Ready completion, implementation approval, canonical issue/change identity, base branch, and implementation owner. Issue labels, issue selection, a valid proposal, or a retry request are not sufficient evidence.
- The repository-level `AGENTS.md` is the strongest proactive workflow trigger. Re-read it before every transition and ensure the implementation worktree contains the same approved `AGENTS.md` before coding.
- Do not create a second worktree when the user is merging, rebasing, cherry-picking, cleaning up, archiving, or otherwise closing out an existing change. Reuse the existing branch/worktree or operate on the explicitly supplied checkout.
- Preserve unrelated user changes in every checkout. Never reset, stash, overwrite, or delete them to make the workflow convenient.

## State discovery

At the start of every invocation, determine the requested lifecycle action and reconstruct state from:

- `AGENTS.md`, `KnowledgeBase/INDEX.md`, and relevant repository instructions;
- the issue/story pack and GitHub state when issue-driven;
- `openspec/config.yaml`, active changes, artifacts, and task checkboxes;
- current branch, worktree list, status, and any existing PR/CI/review evidence.

Prefer an existing matching OpenSpec change, branch, and worktree. Never create duplicates merely because the skill was invoked again. A prunable, broken, incomplete, untracked, or pre-approval worktree is not reusable; report it and stop without repairing, deleting, or creating a replacement automatically.

## Proposal mode — main checkout only

Use proposal mode when the user asks to propose, plan, explore, or otherwise establish an OpenSpec change without starting implementation.

1. Read the repository instructions, issue/story context, and relevant existing specs from the main checkout.
2. Run `$openspec-propose` from the main checkout when no matching change exists. If a matching change exists, use the appropriate update/explore workflow instead of creating a duplicate.
3. Run OpenSpec status, instructions, and strict validation from the main checkout.
4. Present the proposal location, validation result, scope, assumptions, open decisions, and implementation impact.
5. Stop and request explicit user approval before creating a worktree or editing production code.

Proposal validation is not implementation approval. Do not infer approval from a valid proposal, an issue label, or the user's original request to plan.

## Implementation handoff mode

When an approved proposal exists and the user asks to implement, write code, continue implementation, apply tasks, or otherwise begin development, proactively use this flow without waiting for the user to name this skill:

1. Re-read `AGENTS.md` and confirm the approved proposal, issue/story, current OpenSpec status, pending tasks, base branch, and any existing branch/worktree.
2. If an existing matching worktree is available, validate that it is a live Git worktree on the canonical branch, has the expected issue/change identity, and was created or explicitly approved for the current implementation handoff. A prunable, broken, incomplete, untracked, or pre-approval worktree is not reusable; report it and stop without repairing, deleting, or creating a replacement automatically.
3. If no valid matching worktree exists, explicitly ask whether to create the canonical worktree now. Skip this question only when the user already confirmed worktree creation in the current request or an existing workflow checkpoint that includes proposal validation, Definition-of-Ready completion, implementation approval, and owner/base-branch evidence.
4. After confirmation, use `$git-worktrees` to create and validate `openspec/gh-<issue-number>-<short-kebab-slug>` at `.worktrees/gh-<issue-number>-<short-kebab-slug>` from the actual default branch. For issue-less changes use the canonical `openspec/<change-name>` branch.
5. Re-read `AGENTS.md`, `KnowledgeBase/INDEX.md`, the source story pack, and all OpenSpec context files from inside the worktree. If `AGENTS.md` differs from the approved main-checkout version, synchronize the approved file before coding and verify the diff; do not silently discard either version.
6. Update only implementation tracking metadata in the worktree story (`status: In_Progress`, branch, worktree path, base branch, updated), preserving body and unrelated edits.
7. From inside the worktree, invoke `$openspec-orchestrator`, then `$openspec-apply-change` for pending tasks. The apply workflow MUST run `$development-entry-gate` before the first code edit and use the routed implementation/security/review skills.
8. Keep OpenSpec task checkboxes, implementation files, tests, migrations, and verification evidence in the worktree. Report actual commands and results; do not claim success from planned commands.

## Verification, PR, and closeout

When implementation is complete, continue from the existing worktree through review, verification, spec synchronization, archive, commit, push, and PR handling as requested. Before each transition re-read current state and avoid repeating completed transitions.

If the user asks to merge, rebase, cherry-pick, clean up, archive, or otherwise finish the change:

- operate from the existing worktree/branch or explicitly supplied checkout;
- do not ask to create a new worktree;
- use `$git-workflows` for history operations, `$git-commit` for commit/push boundaries, `$pr-creator` for PR delivery, and `$git-worktrees` for cleanup;
- clean up only after the branch is merged or the change is intentionally abandoned, the worktree has no required uncommitted changes, no process/editor still uses it, and the user-authorized closeout is complete;
- after cleanup, verify `git worktree list`, branch/PR state, story status, and OpenSpec archive state.

Never merge, enable auto-merge, close an issue, or discard a worktree with uncommitted changes without explicit authorization. Preserve a resumable state and report the exact next action when blocked.

## Bulk apply mode

Use bulk mode only when at least two active changes remain after applying any explicit change-name filter. If fewer than two candidates remain, fall back to the normal single-change apply workflow. Do not use bulk mode when `$openspec-apply-change` explicitly targets one change.

1. Run `openspec list --json`. If the request named change(s), limit candidates to those names and verify each exists.
2. For every candidate, run `openspec status --change "<name>" --json` and `openspec instructions apply --change "<name>" --json`; read every listed `contextFiles`, including the tasks artifact, and summarize pending work and delivery shape.
3. Resolve dependencies and shared-contract, migration, composition, and shared-UI conflicts before parallelizing. Create one validated worktree under `.worktrees/<change>` for each independent candidate only after each candidate has its validated proposal, completed Definition of Ready, explicit implementation approval, and owner/base-branch handoff. Reuse only a valid matching worktree; quarantine broken, prunable, incomplete, or pre-approval paths and do not create replacements automatically.
4. Keep the parent agent out of implementation. Dispatch one implementation subagent per candidate worktree through the available coordinator/agent capability. If delegation is unavailable, stop bulk mode and report the capability blocker rather than applying changes directly in the parent checkout.
5. Require each subagent to run the targeted `$openspec-apply-change <change>` flow from its worktree and then perform the orchestrator's verification phase (OpenSpec status/validation, acceptance-criteria checks, focused tests, and review evidence). The repository has no separate `/opsx-verify` command; do not invent one.
6. Normalize one report per candidate with worktree path, apply status (`complete`, `paused`, or `failed`), verification status (`ready`, `warnings`, `critical`, or `failed`), changed-files summary, blockers, and unresolved warnings.
7. Report the bulk result without merging or auto-archiving. State which changes are ready for human review and that explicit user approval is required before merge.

## Handoff output

Report:

- OpenSpec change and phase;
- proposal checkout and approval state;
- issue, branch, worktree, and base branch;
- tasks complete/remaining;
- skills/coordinator used;
- verification actually run and results;
- PR/CI/review state; and
- exactly one next safe action.
