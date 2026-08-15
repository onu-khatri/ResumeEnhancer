---
name: git-workflows
description: Apply safe, project-consistent Git workflows for ResumeEnhancer: clean history, interactive rebase, cherry-pick, bisect, reflog recovery, and branch hygiene. Use when managing branches, preparing commits for review, or recovering from Git mistakes.
---

# Git Workflows

Use this skill to keep ResumeEnhancer history clean and recover safely from Git mistakes. It complements `git-commit` (message format) and `git-worktrees` (parallel story isolation).

## Use this skill when

- cleaning up a feature branch before opening a pull request
- applying a fix across multiple branches
- locating the commit that introduced a regression
- recovering lost commits or branches
- synchronizing a diverged branch with `main`

## Do not use this skill when

- the task is only to stage and push a single commit (use `git-commit`)
- you need isolated parallel workspaces (use `git-worktrees`)

## ResumeEnhancer conventions

- The primary branch is `main`; never commit directly to it.
- Feature branches use a `codex/<feature-intent>-<timestamp>` shape (see `pr-creator`).
- Prefer `--force-with-lease` over `--force`.
- Rebase only local, unpushed commits; merge for shared branches.

## Core techniques

### Interactive rebase (clean history before PR)

```bash
git rebase -i main                 # squash/fixup/reword/drop local commits
git rebase -i --autosquash main    # auto-order `--fixup` commits
git push --force-with-lease origin HEAD
```

### Cherry-pick a fix to another branch

```bash
git cherry-pick abc123          # single commit
git cherry-pick abc123..def456  # range (exclusive start)
git cherry-pick --continue      # after resolving conflicts
```

### Bisect to find a regression

```bash
git bisect start
git bisect bad HEAD
git bisect good v1.0.0
git bisect run npm run check    # or: git bisect run dotnet test ... (exit 0 = good)
git bisect reset
```

### Recover lost work with reflog

```bash
git reflog                          # find the lost commit hash
git branch recovered-branch abc123   # restore as a branch
git reset --hard abc123             # or move current branch back
```

## Safety rules

- Always branch a backup before a complex rebase: `git branch backup-before-rebase`.
- Verify the build and tests after any history rewrite.
- Do not rebase a branch that other people are working on.
- Abort cleanly when in doubt: `git rebase --abort`, `git merge --abort`, `git cherry-pick --abort`, `git bisect reset`.

## Definition of Done

- History is linear and each commit is a single logical change.
- `dotnet build application\ResumeEnhancerApp.slnx` (backend) and/or `npm run check` (frontend) still pass after any rewrite.
- Force pushes use `--force-with-lease` only on local branches.
