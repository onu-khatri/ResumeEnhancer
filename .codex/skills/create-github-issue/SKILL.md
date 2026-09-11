---
name: create-github-issue
description: Create one traceable GitHub issue from an approved local ResumeEnhancer user story, then mark the source story as moved to GitHub only after the issue and source update both succeed.
---

# Create GitHub Issue

Use this skill with a path to a `.US.md` user-story file. It creates one or more traceable GitHub issues from the story when the story requires a split, protects against likely duplicates, and records the handoff in the story frontmatter and a story-level GitHub issue register.

This skill is a handoff workflow, not a story-authoring workflow. Use `user-story-creator` to create or revise the story pack, and only run this skill after the story is approved and ready for issue creation.

## Required inputs and preconditions

1. Treat the supplied path as the authoritative source file. Resolve it from the repository root and read the complete `.US.md` file.
2. Confirm that the file is a user-story file, has frontmatter, and contains a stable `id`, `title`, user story, requirements or scope, and acceptance criteria. If a required element is missing, stop and report the missing item.
3. Read the repository `AGENTS.md` and `KnowledgeBase/INDEX.md` before acting. Use linked story or GitHub knowledge only when the index identifies it as relevant.
4. Check the current worktree and source file status. Preserve unrelated changes; do not reset, stash, or overwrite them.
5. Require `status: Ready_To_Implement`. Do not proceed when `status` is already `Move_To_GitHub_Issue` unless the user explicitly asks to reconcile or recreate the handoff. This prevents accidental duplicate issues.

### GitHub MCP Requirement

GitHub issue operations MUST use the project-configured MCP server named `github`.

Do not use any tools prefixed with:

`mcp__codex_apps__github_*`

In particular, do not use:

`mcp__codex_apps__github_create_issue`

Before creating an issue:

1. Verify that the MCP server `github` is available.
2. Verify that it exposes an issue creation/write tool.
3. Use that tool to create the issue.

If the `github` MCP server is not present in the current tool registry, STOP.

Report:

"The project GitHub MCP server is not available in this Codex session."

Do not fall back to Codex Apps, GitHub connectors, or another integration.

### Approved Publishing Destinations

The following repository is explicitly approved for publishing user stories:

- `onu-khatri/ResumeEnhancer`

When the source story has `status: Ready_To_Implement`, the story's prior approval permits issue creation in this repository without requesting additional publication confirmation.

Do not publish to any other repository without explicit user authorization. The repository must still be resolved from the local Git remote and duplicate-checked before any issue is created.

## Workflow

### 1. Build the issue from the story

Extract, without inventing product behavior:

- `id` and `title` from frontmatter
- persona, user story, outcome, and description
- in-scope requirements and business rules
- observable acceptance criteria
- dependencies, preconditions, validation, security, UX, API, data, and definition-of-done details when present
- technical notes only when supported by the source story or its directly linked supporting information

Keep the issue implementation-ready but concise. Preserve important constraints and failure states. Do not turn `Inferred` or `Recommended` material into an approved requirement; label it as context or an open decision when it must be retained.

### 2. Decide whether the story needs multiple issues

Keep one issue when the story describes one coherent, independently deliverable outcome with one ownership boundary. Split it only when the source evidence supports a real delivery boundary, such as:

- distinct user outcomes that can be accepted and delivered independently;
- separate frontend, backend, infrastructure, or architecture ownership with meaningful sequencing;
- a dependency chain where one slice must land before another can be implemented or verified;
- acceptance criteria that would otherwise produce an issue too large to estimate, review, or release safely; or
- an explicit source-story instruction to create separate slices.

Do not split merely because the story has multiple headings, requirements, layers, or acceptance criteria. Do not invent new scope while splitting. Preserve the original story ID and derive stable slice labels such as `<story-id>-1`, `<story-id>-2` in the issue body and title suffix only when needed to distinguish issues.

For every split, record a small issue plan before any GitHub write:

| Pick order | Slice | Outcome | Depends on | Blocks |
| --- | --- | --- | --- | --- |
| 1..N | stable slice label | source-backed outcome | earlier slice labels or `None` | later slice labels or `None` |

Order the plan topologically: prerequisites first, then dependent slices; retain the source story's stated order when dependencies do not decide it. If two slices are independent, use the clearest product or implementation order and state that they are independent. If the dependency graph contains a cycle or cannot be determined from the source, stop and ask for clarification rather than guessing.

### 3. Determine the repository

Identify the current repository from the local Git remote and normalize it to GitHub's `owner/name` form. Do not expose remote credentials, bearer tokens, environment variables, or full credential-bearing URLs in the issue or final report. If the remote cannot be resolved, stop before any GitHub write.

### 4. Check for duplicates before creating

Use the configured GitHub MCP server's issue-search operation against the resolved repository. Search open issues using:

1. the stable story ID;
2. the exact or normalized title; and
3. each planned slice label and distinctive title/outcome terms when the first searches are inconclusive.

Treat an existing issue as a likely duplicate when it has the same story ID and slice label, or clearly represents the same outcome and scope. Do not create that slice when a likely duplicate exists. If the source story requires multiple slices, stop the entire operation when any planned slice has a likely duplicate; do not create only a partial set. Report the existing issue number, title, URL, and why it matched. Leave the source story unchanged.

### 5. Create the issues in dependency order

Create the planned issues sequentially in the selected order using the issue creation/write tool exposed by the project-configured MCP server named `github`. Use the resolved `repository_full_name`, extracted title, and one source-backed body per slice. Do not create more than the planned number of issues. Do not add labels, assignees, milestones, links, or requirements that are not present in the source or explicitly requested by the user. Labels are governed by the repository-label procedure below.

After each successfully created and read-back-verified issue, persist its canonical number, URL, title, date, labels, slice, and dependency references in the story's `## GitHub Issues` register while leaving the lifecycle status as `Ready_To_Implement`. This creates a resumable partial-handoff record; only the fully reconciled set may transition the story to `Move_To_GitHub_Issue`.

### Repository label discovery and selection

Before the first issue creation, discover the labels that already exist in the resolved repository from its canonical labels page:

`https://github.com/<owner>/<repo>/labels`

Use a repository-label listing operation from the configured GitHub MCP server when available; otherwise inspect that canonical labels page through the available browser/web tool. Do not use a different repository, a search-result page, or a guessed label name. If the repository labels cannot be retrieved, stop before creating any issue and report the discovery failure.

Select only labels that both exist in the retrieved repository label set and are supported by explicit story metadata. Match case-insensitively and normalize spaces, hyphens, and underscores for comparison. Use this evidence order:

1. An explicit label requested in the story or by the user.
2. A label matching the story's delivery layer or type, such as `frontend`, `backend`, `user story`, `task`, `bug`, `enhancement`, `technical-debt`.
3. A label matching an explicit priority only when the repository uses a corresponding priority label.
4. Multiple labels may be applied if supported by the story evidence and the repository's existing label set.

Do not infer labels from the issue title alone, copy labels from another repository, create missing labels, or apply a generic label merely to avoid an unlabeled issue. If no existing repository label is supported by the story evidence, create the issue with no labels and report that no supported label was available. Pass the selected existing label names in the issue creation/write call, and use the same selected labels for every slice unless the source story provides slice-specific evidence.

```markdown
# <Issue title, with `(<slice label>)` only when split>

## User Story

As a <persona>, I want <capability>, so that <outcome>.

## Description

<Concise description derived from the story>

## Scope

<In-scope behavior and explicit out-of-scope boundaries, when present>

## Requirements

- <Requirement>

## Acceptance Criteria

- [ ] <Observable criterion>

## Constraints and Rules

<Business rules, validation, authorization, privacy, accessibility, responsiveness, or other supported constraints>

## Technical Notes

<Only when technical information exists in the source story>

## Dependencies and Open Questions

<Only when present in the source story>

## Delivery Order and References

- **Pick order:** <N> of <total planned issues>
- **Depends on:** <`None`, or links to already-created prerequisite issues such as `#123`>
- **Blocks:** <planned downstream slice labels, or `None` when no downstream issue is known>
- **Related to:** <source story ID and related issue links, or `None`>

## Source

Created from: `<repository-relative-input-file-path>`
Story ID: `<story-id>`
Slice: `<slice label or None>`
```

Omit empty sections except `Delivery Order and References` and `Source`, which are required for traceability. Use actual GitHub issue references (`#<number>` and canonical URLs when available) for created prerequisites. For downstream issues not yet created, use the stable planned slice label until its issue number exists; after all issues are created, update earlier issue bodies if needed so every known dependency and blocking relationship has a GitHub issue reference. Do not copy secrets or sensitive values from the source file.

### 6. Verify the created issues

Require a successful MCP response containing an issue number and canonical URL for every planned issue. Record the current `YYYY-MM-DD` creation date for each verified issue. Verify that every selected label is present on the created issue; if label application is missing or ambiguous, stop and report the handoff as incomplete without retrying issue creation. If any response is ambiguous or missing the issue number, URL, or required labels, stop creating issues and report the exact reconciliation state; do not retry an unknown create operation automatically because GitHub may already contain the issue. Preserve any already-verified register rows and keep the source status `Ready_To_Implement`. If downstream references were initially slice labels, use the issue-update tool exposed by the project-configured MCP server named `github` to replace them with actual issue references before changing the source status.

### 7. Move the source story to the GitHub handoff status

Only after all planned issues are created, all issue numbers and URLs are verified, and any required cross-reference updates succeed, update the source `.US.md` frontmatter and GitHub issue register:

```yaml
status: Move_To_GitHub_Issue
updated: <current YYYY-MM-DD date>
```

`Move_To_GitHub_Issue` is the machine-readable value for the display status **Move to GitHub Issue**. Change only the frontmatter lifecycle fields needed for this transition. Preserve the story body, line endings, formatting, and unrelated user changes. Do not update `.SI.md`, `.Research.md`, business requirements, or application code as part of this workflow.

Add or update this story-body section after the frontmatter. Keep one row per created issue, use the verified canonical URL, and do not duplicate an existing row:

```markdown
## GitHub Issues

| GitHub issue | Created date | Number |
| --- | --- | --- |
| [<issue title>](<canonical issue URL>) | <YYYY-MM-DD> | <issue number> |
```

For split stories, record all created issues in dependency order. Preserve the rest of the story body and any unrelated user changes. This register is the durable link from the local user story to its hosted issue and must be present before the handoff is reported complete.

If any issue has been created but a later issue, cross-reference update, issue-register update, or source update fails, do not retry issue creation automatically. Reconcile by searching for every planned slice, verifying existing issue bodies and labels, completing missing cross-references, and updating the register. Keep status `Ready_To_Implement` until the complete set is reconciled; then transition to `Move_To_GitHub_Issue`. Report every created issue, the remote reconciliation state, and the exact next action.

## Issue body rules

- Derive all content from the supplied story and directly linked evidence; never invent requirements or split scope.
- Keep acceptance criteria testable and retain important success, empty, error, authorization, privacy, and accessibility behavior.
- Preserve traceability with the story ID, slice label when split, repository-relative source path, pick order, and issue references.
- Update the source story's `## GitHub Issues` register with the verified title/link, creation date, and issue number for every created issue.
- Discover labels from the resolved repository's canonical labels page and apply only existing labels supported by explicit story metadata; verify label application before completing the handoff.
- Keep implementation choices in technical notes unless the story states them as constraints.
- Never expose PATs, credentials, environment variables, private tokens, or sensitive personal data.

## Failure and idempotency rules

- Missing or invalid story structure: stop; do not call GitHub and do not change the source.
- The project GitHub MCP server is not available in this Codex session: stop; do not change the source.
- Likely duplicate found: report it; do not create or modify an issue or source file.
- Issue creation failed or cannot be verified: leave the source status unchanged.
- Repository labels cannot be retrieved or a selected label cannot be verified on the created issue: do not create another issue and leave the source status and issue register unchanged.
- Issue created but source frontmatter or GitHub issue-register update failed: do not create another issue; report every issue URL and the local reconciliation needed.
- Existing non-handoff status, including `Ready_To_Implement`, is not evidence that an issue should be recreated. Search for duplicates first and require explicit user direction to override a previously completed handoff.

## Completion report

On success, report:

- story ID and source path
- issue number, title, verified URL, and delivery order for every created issue
- source transition: `Move_To_GitHub_Issue` (Move to GitHub Issue)
- story-level `## GitHub Issues` register containing the verified issue title/link, creation date, and number
- checks performed and any remaining limitations

On duplicate or failure, report the exact stopping point and confirm that the source status was not changed, except for the partial-handoff case described above.
