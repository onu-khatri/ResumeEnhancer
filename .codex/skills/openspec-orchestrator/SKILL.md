---
name: openspec-orchestrator
description: >-
  Orchestrates OpenSpec spec-driven development with GitHub. Use when a GitHub
  issue, repository task, pull request, review thread, or requested code change
  should be driven through the appropriate OpenSpec skills automatically from
  discovery and proposal through implementation, verification, spec sync,
  archiving, and pull-request handling. Resumes safely from existing OpenSpec,
  branch, issue, PR, and CI state instead of duplicating work.
compatibility: >-
  Requires OpenSpec with its workflow skills installed. Designed for agents
  with GitHub repository, issue, branch, file, pull-request, review, and CI
  capabilities. Explicit sub-skill invocation is preferred when supported;
  otherwise load and follow the installed OpenSpec skill instructions.
---

# OpenSpec GitHub Orchestrator

Act as a resumable state-machine coordinator between OpenSpec and GitHub. For issue execution, `$issues-kickoff` must provide the approved issue, canonical branch/worktree, dependency handoff, and implementation owner before this coordinator starts implementation.
OpenSpec is the source of truth for planned behavior; GitHub is the source of
truth for repository, issue, branch, pull-request, review, and CI state.

Do not invent hidden workflow state. Reconstruct state on every run from the
repository, `openspec/`, GitHub objects, task checkboxes, and CI results.

## Inputs

Accept any of the following:

- a GitHub repository (`owner/repo` or repository URL),
- an issue number or issue URL,
- a pull request number or PR URL,
- a plain-language change request,
- an existing OpenSpec change name,
- a request such as "implement this issue", "continue this PR", "fix review
  feedback", "verify this change", or "finish the OpenSpec workflow".

Derive missing non-sensitive context from GitHub and the repository before
asking the user. Ask only when a required decision cannot be resolved safely.

## OpenSpec skills to orchestrate

Use the installed OpenSpec workflows rather than reimplementing their internal
logic:

- `openspec-explore` — investigate ambiguity; never code.
- `openspec-propose` — create a complete proposal and planning artifacts.
- `openspec-update-change` — revise existing planning artifacts when the plan
  or requirements change.
- `openspec-apply-change` — implement tasks and update task checkboxes.
- `openspec-sync-specs` — merge delta specs into main specs when needed.
- `openspec-archive-change` — archive a completed change.

If the environment supports explicit skill invocation, invoke the OpenSpec
skill by name. Otherwise, read/load its installed `SKILL.md` and follow it.
Do not silently replace an OpenSpec workflow with ad-hoc planning.

## GitHub capability adapter

Prefer the provided GitHub capabilities. Map intent to the closest available
operation rather than assuming a specific API wrapper exists.

Typical operations include:

- repository discovery/read: `get_repo`, `search`, `fetch_file`, `fetch`,
- issue read/write: `fetch_issue`, `search_issues`, `fetch_issue_comments`,
  `create_issue`, `update_issue`, `add_issue_labels`,
- branch operations: `search_branches`, `create_branch`,
- file operations: `create_file`, `update_file`, `delete_file`,
- PR discovery/read: `search_prs`, `get_pr_info`, `fetch_pr`,
  `list_pr_changed_filenames`, `fetch_pr_file_patch`, `fetch_pr_patch`,
- PR write/review: `create_pull_request`, `update_pull_request`,
  `request_pull_request_reviewers`, `add_review_to_pr`,
  `reply_to_review_comment`, `resolve_review_thread`,
- CI/status: `fetch_commit_workflow_runs`,
- merge: `merge_pull_request` or `enable_auto_merge`, but only when explicitly
  authorized by the user.

If a named operation is unavailable, use an equivalent provided GitHub tool.
Never claim an action was performed unless the tool result confirms it.

## Canonical identifiers

When a GitHub issue is the origin of a new change, prefer:

- OpenSpec change: `gh-<issue-number>-<short-kebab-slug>`
- branch: `openspec/gh-<issue-number>-<short-kebab-slug>`

If an existing change, branch, or PR already clearly corresponds to the work,
reuse it. Never create a second change merely to normalize its name.

For work without an issue, use a concise kebab-case change name and branch
`openspec/<change-name>`.

## State discovery

Before changing anything, reconstruct the workflow state.

1. Resolve the repository and default/base branch.
2. If an issue or PR is provided, fetch it and relevant comments/reviews.
3. Inspect the repository for:
   - `openspec/config.yaml`,
   - `openspec/changes/`,
   - matching active or archived change folders,
   - planning artifacts and `tasks.md`,
   - project instructions such as `AGENTS.md`, `CONTRIBUTING.md`, and relevant
     test/build configuration.
4. Search for an existing matching branch and PR.
5. If a PR exists, inspect changed files, review threads, and CI/workflow state.
6. Classify the next safe state transition. Do not repeat completed transitions.

## Routing policy

Choose the OpenSpec workflow from observed state and user intent.

### A. Ambiguous problem, no settled implementation direction

Use `openspec-explore`.

Exit when the problem, scope, constraints, and plausible approach are clear.
If the user asked for end-to-end execution, continue automatically into the
proposal state once ambiguity is sufficiently resolved.

### B. No matching active change exists

Use `openspec-propose` for the normal one-shot planning path.

For issue-driven work, incorporate the issue's requirements, acceptance
criteria, constraints, relevant discussion, and links/references into the
proposal context.

Stop before coding only if the user requested planning/review only. Otherwise
continue to implementation after the planning artifacts are coherent and the
applicable implementation approval is recorded.

### C. Matching active change exists but requirements changed

Use `openspec-update-change` before coding.

Requirement-changing signals include:

- issue scope changed,
- review feedback changes externally observable behavior,
- acceptance criteria changed,
- implementation reveals a design/spec contradiction,
- the PR diverges materially from the current OpenSpec artifacts.

Do not patch code first and leave the specification stale.

### D. Planning is complete and tasks remain

Ensure the canonical feature branch/worktree exists. For issue work, validate the branch created by `$issues-kickoff`; for direct OpenSpec work, create it through `$git-worktrees` only after the applicable approval gate. Then use `openspec-apply-change`.

Apply the smallest code changes required by the tasks. Preserve OpenSpec's task
checkboxes as the implementation progress record. Run relevant tests, lint,
type checks, builds, or project validation after meaningful increments and at
completion.

If blocked because artifacts are missing, use `openspec status` and
`openspec instructions` to identify and create the next required artifact, then
re-run the planning state checks before resuming apply.

### E. Implementation is complete

Also inspect GitHub-visible implementation evidence:

- PR diff/change set,
- test/build results,
- CI/workflow runs,
- unresolved review threads,
- issue acceptance criteria.

Treat any OpenSpec CRITICAL finding, failing required CI, incomplete task, or
unresolved requirement-level review feedback as not ready.

Perform verification against the proposal/spec/design/tasks and the actual
implementation evidence. Report OpenSpec verification as `ready`, `blocked`, or
`warnings`; this built-in verification is the coordinator's responsibility.

### F. Verification finds a plan problem

Use `openspec-update-change`, then `openspec-apply-change`, then verify again.

### G. Verification finds only an implementation problem

Use `openspec-apply-change` to finish/fix the remaining tasks, then verify again.

### H. Specs need promotion

Use `openspec-sync-specs` when main specs should be updated independently, or as
required by the archive workflow.

### I. Change is archive-ready

Use `openspec-archive-change` on the feature branch. Permit its normal spec-sync
step when needed.

Archive only when planning artifacts are complete, tasks are complete, and
verification has no blocking findings unless the user explicitly accepts the
warnings.

### J. Publish/update pull request

Create or update the PR only after reconstructing whether one already exists.
The PR should make the OpenSpec relationship easy to audit.

Recommended PR body sections:

- Summary
- GitHub issue / motivation
- OpenSpec change name and path
- What changed
- Verification performed
- CI/test status
- Remaining warnings or follow-ups

Link the originating issue using GitHub's normal closing syntax only when the PR
actually satisfies the issue and closing it on merge is intended.

If a PR already exists, update it rather than creating a duplicate.

### K. Review feedback arrives

Classify each unresolved thread:

1. requirement/spec change → `openspec-update-change` → apply → verify,
2. implementation bug within current plan → apply → verify,
3. question/no code change → answer with evidence,
4. stale/already addressed → explain and resolve only when justified.

Never change the implementation in a way that materially changes behavior
without first updating OpenSpec.

### L. Merge

Never merge, enable auto-merge, close the issue, or perform an equivalent final
GitHub action merely because CI is green.

Merge only when the user explicitly requests or has explicitly authorized that
action, and immediately before merging re-check:

- PR is open and mergeable enough for the selected method,
- expected head SHA has not moved when the tool supports an expected SHA,
- required CI is passing,
- blocking review threads/findings are resolved,
- OpenSpec change is archive-ready/archived according to the repository's
  chosen workflow.

## Idempotency and resume rules

On every invocation:

- prefer existing issue, change, branch, and PR objects,
- derive progress from `tasks.md`, files, PR state, reviews, and CI,
- never create duplicate proposals, branches, issues, or PRs when a matching
  object exists,
- never rerun completed OpenSpec transitions solely because the skill was
  invoked again,
- continue from the first incomplete or invalid state,
- after every mutation, re-read enough state to verify the mutation succeeded.

## Failure handling

When a step fails:

1. Preserve the current resumable state.
2. Identify whether the failure is specification, implementation, GitHub,
   permission, CI, or tool availability related.
3. Fix reversible local problems automatically when the requested task permits.
4. Do not bypass failing validation by weakening tests, deleting checks, or
   editing requirements merely to make the workflow green.
5. If blocked by permission or a genuinely user-owned decision, stop with the
   exact blocker and the next action needed.

## Output format

At the end of each run, report:

### Workflow state
- Repository: `<owner/repo>`
- Issue: `#<n>` or `N/A`
- OpenSpec change: `<name>`
- Branch: `<branch>`
- PR: `#<n>` or `not created`
- OpenSpec phase: `<explore|plan|apply|verify|sync|archive|complete>`

### Actions completed
A concise list of the OpenSpec skills and GitHub operations completed in this
run.

### Validation
- OpenSpec tasks: `<done>/<total>`
- OpenSpec verification: `<ready|blocked|warnings|unavailable>`
- Tests/CI: `<passing|failing|pending|not available>`
- Review threads: `<resolved/unresolved summary>`

### Next safe action
State exactly one next safe action. If no further action is needed, say
`Complete`.

## Rules

- OpenSpec artifacts govern intended behavior; GitHub state governs delivery.
- Never skip proposal/update work when behavior or requirements change.
- Never use `openspec-explore`, `openspec-propose`, or
  `openspec-update-change` to write production code.
- Never manually mark OpenSpec tasks complete without implementing and
  validating the task.
- Never archive a knowingly incomplete change without explicit user approval.
- Never merge or enable auto-merge without explicit user authorization.
- Never fabricate CI, review, issue, branch, PR, or OpenSpec state.
- Prefer safe, resumable, idempotent transitions over one-shot assumptions.
