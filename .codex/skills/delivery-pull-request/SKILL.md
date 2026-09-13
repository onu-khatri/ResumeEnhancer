---
name: delivery-pull-request
description: Raise high-quality ResumeEnhancer pull requests from the current worktree with safe branch, commit, push, hosted-PR, story-traceability, and verification gates. Use when Codex needs to finish delivery, not only draft PR text.
---

# PR Creator

Use this skill to finish implementation work safely and raise it for review with the right branch, commit, push, hosted PR, story references, architectural context, and verification notes. Use the repository Git skills for their owned operations: `$git-worktrees` for branch/worktree isolation, `$git-commit` for staging and commits, and `$git-workflows` for synchronization or history operations.

## Preconditions and authority

- Read `AGENTS.md`, the relevant user story, linked business requirements, and the complete current diff before mutating Git state.
- Confirm the repository, current branch, worktree, base branch, and whether a merge, rebase, cherry-pick, or bisect is active.
- Preserve unrelated staged and unstaged changes. Do not reset, stash, clean, force-update, or overwrite work to make the PR appear clean.
- Do not commit directly to `main`.
- Use only the configured project GitHub MCP server named `github` for hosted PR lookup, creation, and verification. Verify that it exposes the required PR read/write operation before attempting hosted mutation. If it is unavailable, stop after any safe local preparation and report the exact blocker; do not fall back to an unapproved connector.

## Delivery workflow

1. Inspect status and history with the preflight commands from `$git-commit`, then review both `git diff -- .` and `git diff --cached -- .`.
2. Establish the branch:
    - If on `main` with a clean worktree, use the already-approved `$delivery-issues-kickoff` worktree, or `$git-worktrees` for a direct OpenSpec workflow, using the canonical `openspec/...` branch/worktree.
   - If on `main` with uncommitted changes, stop and report the user-owned changes; do not move them implicitly.
   - If already on a non-main feature branch/worktree, keep using it after validating its base and ownership.
3. Confirm the diff is one coherent delivery scope. If unrelated files or changes are mixed in, stop and identify the exact separation needed; do not silently discard or split user work.
4. Resolve commit state:
   - If intended changes are unstaged or staged but uncommitted, use `$git-commit` to choose boundaries, stage deliberate pathspecs, run its required checks, and create a traceable commit.
   - If the branch already has the intended commit, do not create an empty or duplicate commit.
   - If the branch has no commits ahead of its base and no intended changes, stop: there is nothing to raise as a PR.
5. Synchronize only when needed. Use `$git-workflows` to inspect divergence and safely update from the verified base; never rewrite published/shared history or force-push without explicit authorization and verified recovery conditions.
6. Push the feature branch through `$git-commit`, verify the upstream branch and pushed commit SHA, and distinguish a successful push from a hosted PR.
7. Check for an existing open PR for the same head/base branch and story or issue references. If one exists, update/reconcile that PR only when the user requested it; do not create a duplicate.
8. Create the hosted PR through the configured `github` MCP PR-write operation using the verified repository, head branch, base branch, title, and reviewer-ready body. Include issue/story references and actual verification results. Do not claim merge, approval, or hosted enforcement.
9. Verify the MCP response contains the PR number and canonical URL, then read the hosted PR to confirm the title, body, base, head, state, and source issue/story references. Only after this succeeds may the source story frontmatter move to `PR_Open` and record `pr_url`.
10. Report the commit SHA, pushed branch, PR number/URL, story transition, checks run, and remaining limitations.

## ResumeEnhancer Rules

- Mention module boundaries explicitly when they matter.
- Call out changes to `ResumeEnhancer.<ModuleName>.Web`, `ResumeEnhancer.<ModuleName>.SL`, `ResumeEnhancer.<ModuleName>.PL`, and frontend `features/resume` separately when touched.
- Mention schema or migration impact whenever persistence changes.
- Mention validation, mapping, and test coverage when relevant.
- Preserve the story's `## GitHub Issues` register and reference the corresponding issue in the PR body, title when useful, and commit footer.
- For documentation, skill, agent, prompt, or configuration-only changes, state explicitly that application runtime behavior was not tested.

## Verification

- State which verification commands were run, not just that code changed.
- Backend: `dotnet build application\ResumeEnhancerApp.slnx` and the relevant `dotnet test` project from `test/`.
- Frontend: `npm run check` and `npm run build` in the client.
- Separate verified behavior from untested or risky areas in the PR description.
- Local commit and push verification do not prove that a hosted PR exists; require the hosted MCP response and a follow-up read before recording `PR_Open`.
- A local workflow file does not prove hosted merge enforcement; report repository ruleset or branch-protection state only when separately verified.

## PR body template

Use this structure, omitting empty sections:

```markdown
## Summary

<Reviewer-readable outcome and why it matters>

## Changes

- <coherent change>

## Traceability

- Story: `<story-id>` — `<repository-relative story path>`
- Issue: #<number> — <canonical issue URL>
- Requirements: <BR or requirement references, when present>

## Verification

- `<command>` — <actual result>
- Hosted PR verification — <number, URL, base, head, and state>

## Remaining Limitations

- <unverified behavior, follow-up, or hosted limitation>
```
