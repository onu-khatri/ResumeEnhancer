---
name: git-worktrees
description: Safely create, validate, operate, and clean up isolated Git worktrees for scalable ResumeEnhancer issue delivery. Use when parallel work, a clean checkout, or branch isolation is required.
---

# Git Worktrees

Use this skill to create reproducible isolated workspaces without stashing or switching the primary checkout. It is the worktree authority for `$issues-kickoff`; use `$git-workflows` for history surgery and `$git-commit` for commits.

## Use this skill when

- implementing multiple handed-off GitHub issues in parallel (see `issues-kickoff` and `story-orchestrator`)
- a feature needs a clean checkout separate from your current working tree
- you want to keep an in-progress change while starting unrelated work

## Do not use this skill when

- a single branch checkout is sufficient
- the work is a one-off edit that does not need isolation

## Preflight

Before creating or removing anything, capture:

```bash
git rev-parse --show-toplevel
git status --short
git branch --show-current
git worktree list --porcelain
git remote -v
```

Confirm the requested issue/story ID, target base branch, worktree path, branch name, and whether the target worktree or branch already exists. Never reuse a worktree for a different issue. Stop if the primary checkout has an unfinished merge, rebase, cherry-pick, or bisect.

## Create workflow

1. Confirm the repository and branch with the preflight checks above. Resolve the repository root before interpreting relative paths.

```bash
git rev-parse --show-toplevel
git branch --show-current
```

2. Choose the canonical project-local `.worktrees/gh-<issue-number>-<short-kebab-slug>` directory and verify that the directory itself is ignored:

```bash
git check-ignore -q .worktrees
```

If it is not ignored, stop and request or apply the repository-approved ignore change before creating a worktree. Do not silently append to `.gitignore` while the worktree operation is in progress.

3. Verify the base branch exists and is current enough for the issue plan, then create the worktree with a new branch:

```bash
git worktree add .worktrees/gh-<issue-number>-<short-kebab-slug> -b openspec/gh-<issue-number>-<short-kebab-slug>
```

Use `--` for path boundaries where applicable. Do not use `-B` or force an existing branch unless the user explicitly requests recovery and the target has been verified.

4. Validate the new worktree before implementation:

```bash
   git -C .worktrees/gh-<issue-number>-<short-kebab-slug> status --short
   git -C .worktrees/gh-<issue-number>-<short-kebab-slug> branch --show-current
   git -C .worktrees/gh-<issue-number>-<short-kebab-slug> log -1 --oneline
```

Then run the smallest relevant baseline checks:
   - Backend: `dotnet build application\ResumeEnhancerApp.slnx`
   - Frontend: `npm install` then `npm run check`
5. Record the worktree path and branch in the source story frontmatter, then work inside the worktree. The original checkout stays untouched.

## Parallel delivery rules

- Use one worktree per GitHub issue or independently owned slice.
- Do not parallelize changes to the same migration, shared contract, composition root, shared UI primitive, or generated file without an explicit coordinating owner.
- Use deterministic names containing the stable story ID; do not use generic names such as `feature` or `work`.
- Each worktree must have one owning agent and one stated base branch.
- Do not run cleanup while an agent, terminal, test process, or editor still has the worktree open.

## Cleanup and recovery

```bash
git status --short
git worktree list --porcelain
git worktree remove .worktrees/<feature>   # only after changes are committed or intentionally discarded
git worktree prune                          # reconcile stale administrative entries
```

If removal reports uncommitted changes, stop and show the path and status. Do not force-remove or delete the directory to bypass the warning. If a worktree directory was manually removed, run `git worktree prune` only after confirming no valid worktree still uses that path.

## Safety rules

- Always verify `.worktrees/` is gitignored before creating a worktree, or its contents will pollute `git status`.
- Do not proceed past a failing baseline build/test without investigating and recording the failure.
- Never use destructive filesystem deletion to clean a worktree.
- Do not force-update or delete branches as part of ordinary setup or cleanup.
- One worktree per issue keeps independently owned work isolated; shared-contract and migration work still requires one coordinating lane.

## Definition of Done

- Repository, branch, path, and existing worktree state were verified before mutation.
- Worktree was created under an ignored directory with a fresh deterministic `openspec/` branch.
- Baseline checks and the initial branch/worktree identity were recorded before implementation starts.
- Worktree is removed only after branch completion and clean status, or the remaining changes are explicitly handed off.
