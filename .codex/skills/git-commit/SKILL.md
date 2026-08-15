---
name: git-commit
description: Write conventional, traceable commit messages for ResumeEnhancer and stage/push changes safely. Use when committing changes, crafting commit messages, or pushing work to a remote.
---

# Git Commit

Use this skill to write commits that future maintainers and reviewers can follow, and to push work without surprises.

## Use this skill when

- committing or pushing changes
- writing or reviewing a commit message
- deciding how to split a large change into commits

## Do not use this skill when

- you need history rewriting or branch surgery (use `git-workflows`)
- you only need to review a diff (use `production-code-reviewer`)

## Branch safety

- Never commit directly to `main`.
- Confirm the branch before committing:

```bash
git branch --show-current
```

- If on `main`, create a `codex/<feature-intent>-<timestamp>` branch first (see `pr-creator`).

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
- Body explains what and why, not how; contrast with previous behavior when relevant.

## ResumeEnhancer scope

- Use the module as scope when it helps: `feat(resume): ...`, `fix(web): ...`.
- Reference user story IDs and business requirements in the footer (`Refs US-7.2`) so commits stay traceable.

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

## Pushing safely

- Stage only intended files; inspect `git status` and `git diff` first.
- Never commit secrets, connection strings, or generated `bin/obj` output.
- Push with `git push` (and `-u` for a new branch); use `--force-with-lease` only when rewriting an already-pushed local branch.

## Definition of Done

- Commit message follows `<type>(<scope>): <subject>` convention.
- Each commit is one logical, reviewable change.
- No secrets, build artifacts, or unrelated files are included.
