---
name: git-commit
description: Create small, conventional, traceable ResumeEnhancer commits with safe staging, validation, and push handoffs. Use when selecting commit boundaries, writing commit messages, committing, or pushing.
---

# Git Commit

Use this skill to turn an inspected diff into small, reviewable, traceable commits and a verified push. It complements `git-workflows` for history surgery and `git-worktrees` for isolation.

## Use this skill when

- committing or pushing changes
- writing or reviewing a commit message
- deciding how to split a large change into commits

## Do not use this skill when

- you need history rewriting or branch surgery (use `git-workflows`)
- you only need to review a diff (use `quality-production-code-review`)

## Preflight and branch safety

Before staging or committing:

```bash
git status --short
git branch --show-current
git diff --stat
git diff --cached --stat
```

- Never commit directly to `main`.
- Confirm the branch before committing:

```bash
git branch --show-current
```

- If on `main`, use `$delivery-issues-kickoff` and `$git-worktrees` to create the canonical `openspec/...` branch first.
- Confirm the issue/story ID and intended files before staging. Preserve unrelated staged or unstaged changes.
- Stop if a merge, rebase, cherry-pick, or bisect is active until its state is understood.

## Commit boundaries

Create one commit per coherent, independently reviewable change. Split commits when changes have different runtime behavior, ownership, rollback risk, or verification paths. Keep contract/schema changes with their required implementation or make the dependency explicit; do not split a migration from the code that must consume it unless the repository workflow requires it.

Before committing, inspect both unstaged and staged diffs:

```bash
git diff -- .
git diff --cached -- .
```

Stage deliberately with pathspecs, not broad `git add .`, when unrelated changes are present:

```bash
git add -- <intended-file> <another-intended-file>
git diff --cached --check
```

## Commit message format

```
<type>(<scope>): <subject>

<body>

<footer>
```

| Type | Purpose |
| --- | --- |
| `feat` | New feature |
| `fix` | Bug fix |
| `refactor` | Restructure without behavior change |
| `perf` | Performance improvement |
| `docs` | Documentation only |
| `test` | Test additions or corrections |
| `build` | Build system or dependencies |
| `ci` | CI configuration |
| `chore` | Maintenance tasks |

## Subject and body rules

- Imperative present tense: "Add resume export", not "Added resume export".
- Capitalize the first word; no trailing period; keep the subject under 70 characters.
- Body explains what and why, not every implementation step; mention behavior, migration, compatibility, or risk when relevant.
- Keep the subject specific enough to identify the slice without embedding volatile issue URLs.

## ResumeEnhancer scope

- Use the module as scope when it helps: `feat(resume): ...`, `fix(web): ...`.
- Reference GitHub issue and user story IDs in the footer (`Refs #123`, `Refs US-7.2`) so commits stay traceable. Preserve multiple references when a coordinated change serves more than one issue.
- Do not claim tests, builds, or review completion in the commit message unless they actually ran; report verification in the handoff.

## Examples

```text
feat(resume): Add search and paging to resume list

Implements the search flow from US-7.2 with page size caps,
deterministic ordering, and split-query includes.

Refs US-7.2
```

```text
fix(web): Return 404 instead of 500 for missing resume

Map KeyNotFoundException to NotFound in the endpoint executor so a
deleted resume no longer produces a server error.
```

## Commit and push verification

After committing, verify the resulting commit and that no unintended files were included:

```bash
git show --stat --oneline HEAD
git status --short
```

Run the smallest meaningful checks for the touched slice before pushing. If the commit is wrong, use a new corrective commit unless the branch is explicitly in a local history-cleanup workflow owned by `git-workflows`.

## Pushing safely

- Stage only intended files; inspect `git status`, `git diff`, and `git diff --cached` first.
- Never commit secrets, connection strings, or generated `bin/obj` output.
- Confirm the remote and upstream before pushing: `git remote -v` and `git branch -vv`.
- Push with `git push -u origin HEAD` for a new branch; use `--force-with-lease` only when rewriting an already-pushed private/local branch under `git-workflows`.
- Report the pushed branch and commit SHA; a successful push is not a hosted PR.

## Definition of Done

- Commit message follows `<type>(<scope>): <subject>` convention and includes issue/story traceability when available.
- Each commit is one logical, reviewable change with an inspected staged diff.
- Relevant checks were run and reported accurately.
- No secrets, build artifacts, or unrelated files are included.
- Push status is distinguished from hosted PR creation and URL verification.
