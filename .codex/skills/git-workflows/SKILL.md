---
name: git-workflows
description: Apply safe, scalable Git workflows for ResumeEnhancer, including branch synchronization, history cleanup, cherry-pick, bisect, reflog recovery, conflict handling, and branch hygiene.
---

# Git Workflows

Use this skill to choose and execute branch/history operations with explicit checkpoints and recovery paths. It complements `git-commit` (commit boundaries and messages) and `git-worktrees` (isolated parallel workspaces). Do not use it for ordinary file staging or commit creation.

## Use this skill when

- cleaning up a feature branch before opening a pull request
- applying a fix across multiple branches
- locating the commit that introduced a regression
- recovering lost commits or branches
- synchronizing a diverged branch with `main`

## Do not use this skill when

- the task is only to stage and push a single commit (use `git-commit`)
- you need isolated parallel workspaces (use `git-worktrees`)

## Universal preflight

Before any branch or history mutation, inspect:

```bash
git rev-parse --show-toplevel
git status --short
git branch --show-current
git log -5 --oneline --decorate
git worktree list --porcelain
```

Identify whether the branch is local, shared, or pushed, record the target commit or branch, and create a recovery point before complex operations. Never proceed through an existing merge, rebase, cherry-pick, or bisect state without resolving or aborting it deliberately.

## ResumeEnhancer conventions

- The primary branch is `main`; never commit directly to it.
- Feature branches use a `codex/<feature-intent>-<timestamp>` shape (see `pr-creator`).
- Prefer `--force-with-lease` over `--force`.
- Rebase only local, unpushed commits; merge or update from the remote for shared branches.
- Use stable story/issue IDs in branch names, commits, and PR references.
- Prefer small, independently verifiable commits so parallel issue work can be reviewed or cherry-picked safely.

## Core techniques

### Interactive rebase (local history before PR)

```bash
git branch backup-before-rebase
git rebase -i main                 # squash/fixup/reword/drop local commits
git rebase -i --autosquash main    # auto-order `--fixup` commits
git push --force-with-lease origin HEAD
```

Before pushing, inspect `git diff main...HEAD`, verify the branch is yours, and confirm no unrelated commits or worktree changes are included. If conflicts occur, inspect each file, run focused checks, then use `git rebase --continue`; abort with `git rebase --abort` when the intended history cannot be preserved.

### Cherry-pick a fix to another branch

```bash
git cherry-pick abc123          # single commit
git cherry-pick abc123..def456  # range (exclusive start)
git cherry-pick --continue      # after resolving conflicts
```

Cherry-pick in dependency order, one logical commit at a time when possible. Confirm the source commit, destination branch, and clean status first. After conflict resolution, inspect the staged diff and run focused checks before continuing. Abort with `git cherry-pick --abort` if the result is not equivalent to the intended change.

### Bisect to find a regression

```bash
git bisect start
git bisect bad HEAD
git bisect good v1.0.0
git bisect run npm run check    # or: git bisect run dotnet test ... (exit 0 = good)
git bisect reset
```

Define a deterministic good/bad command before starting. Keep the test command read-only and stable; record the first bad commit and evidence, then return to the original branch state with `git bisect reset`.

### Recover lost work with reflog

```bash
git reflog                          # find the lost commit hash
git branch recovered-branch abc123   # restore as a branch
git show abc123                     # inspect before restoring
```

Treat `git reset --hard`, branch deletion, and force updates as recovery-only operations requiring explicit target verification and a recoverable reference. Do not use them to discard unknown user work.

### Synchronize a feature branch

For a local unpushed branch, prefer rebasing onto the verified current `main`. For a pushed/shared branch, fetch and merge or use the repository-approved update strategy. Before either path, inspect divergence:

```bash
git fetch origin
git log --oneline --left-right HEAD...origin/main
git diff --stat HEAD...origin/main
```

Never assume the remote default branch, force-push permission, or that a branch is private. Verify those facts from the current repository state.

## Safety and scaling rules

- Always create a clearly named backup reference before a complex local rewrite, and verify it exists.
- Verify the build/tests relevant to the changed slice after any history rewrite or conflict resolution.
- Do not rebase a branch that other people are working on or that has been published unless explicitly coordinated.
- Keep operations scoped with `--` path separators and avoid broad globs when selecting files.
- Abort cleanly when in doubt: `git rebase --abort`, `git merge --abort`, `git cherry-pick --abort`, `git bisect reset`.
- Never use `git reset --hard`, `git clean`, `git branch -D`, or force-push to solve uncertainty; verify targets and obtain explicit direction first.

## Definition of Done

- The operation's before/after refs and affected commits are known.
- History is reviewable and each commit remains a single logical change.
- Relevant build/tests pass after rewrite or conflict resolution, or the exact gap is reported.
- Force pushes use `--force-with-lease` only on a verified private/local branch.
- Recovery references are retained until the result is verified.
