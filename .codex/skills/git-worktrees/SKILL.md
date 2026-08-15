---
name: git-worktrees
description: Create isolated Git worktrees for parallel ResumeEnhancer story delivery without switching branches. Use when starting feature work that needs workspace isolation or before executing a multi-story plan.
---

# Git Worktrees

Use this skill to spin up isolated workspaces so multiple stories can be worked on in parallel without stashing or branch-switching.

## Use this skill when

- kicking off multiple user stories in parallel (see `us-kickoff` and `story-orchestrator`)
- a feature needs a clean checkout separate from your current working tree
- you want to keep an in-progress change while starting unrelated work

## Do not use this skill when

- a single branch checkout is sufficient
- the work is a one-off edit that does not need isolation

## Workflow

1. Confirm the repository and branch:

```bash
git rev-parse --show-toplevel
git branch --show-current
```

2. Choose a worktree directory (project-local `.worktrees/` is preferred) and verify it is ignored:

```bash
git check-ignore -q .worktrees || echo ".worktrees/" >> .gitignore
```

3. Create the worktree with a new branch:

```bash
git worktree add .worktrees/<feature> -b codex/<feature>-<timestamp>
```

4. Restore the baseline in the new worktree:
   - Backend: `dotnet build application\ResumeEnhancerApp.slnx`
   - Frontend: `npm install` then `npm run check`
5. Work inside the worktree; the original checkout stays untouched.

## Cleanup

```bash
git worktree remove .worktrees/<feature>   # after branch is merged
git worktree prune                          # drop stale entries
```

## Safety rules

- Always verify `.worktrees/` is gitignored before creating a worktree, or its contents will pollute `git status`.
- Do not proceed past a failing baseline build/test without investigating.
- One worktree per story keeps shared-contract and migration work from colliding.

## Definition of Done

- Worktree created under an ignored directory with a fresh `codex/` branch.
- Baseline build and checks pass before implementation starts.
- Worktree removed after the branch is merged.
