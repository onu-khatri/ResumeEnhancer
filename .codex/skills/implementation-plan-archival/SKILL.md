---
name: implementation-plan-archival
description: Consolidate completed implementation-plan iterations into a traceable permanent history document after implementation and required review/validation are complete.
metadata:
  author: ResumeEnhancer
  version: "1.1"
---

# Implementation Plan Archival

Use this skill when a worktree contains multiple implementation-plan drafts, revisions, review notes, approvals, or follow-up plans and the implementation lifecycle has reached its verified completion gate. The output is a durable historical record of why and how the work was implemented; it is not a planning or implementation authorization mechanism.

## Boundaries

- Read plans from `.tmp/ImplementationPlans/<change-or-worktree>/**/*.md`.
- Write the consolidated record under `User-Stories/Implemented-Plans/` using a deterministic story/issue-based filename such as `<story-or-issue-id>-<short-title>.md`.
- Do not edit production code, tests, migrations, configuration, OpenSpec task checkboxes, or generated files.
- Do not archive while required implementation, review, architecture review, or final validation work is incomplete.
- Do not treat a valid plan, passing tests, or completed checkboxes alone as proof that implementation is complete.
- Never silently overwrite an existing archive. Reconcile an existing record only when it is the same lifecycle and the new evidence is additive; otherwise stop and report the collision.

## Required completion gate

Before creating or updating an archive, establish evidence for:

1. implementation is complete for the scoped task/batch;
2. required tests and final validation passed, with failures or limitations recorded;
3. required code, security, and architecture reviews are complete;
4. the change, story/issue identity, branch/worktree, and plan set are known;
5. the source plans are readable and can be accounted for in the history.

If any gate is unknown, return `Blocked` with the missing evidence and the one next safe action. Do not infer completion from an agent saying it is done.

## Source discovery and ordering

Discover Markdown plans recursively below the scoped implementation-plan directory. Include plan drafts, reviewer/architect feedback, approval records, re-plans, deviation plans, and follow-up plans. Exclude unrelated changes and already archived copies unless they are explicitly being reconciled.

Order artifacts deterministically by:

1. explicit `created_at`, `createdAt`, or equivalent timestamp;
2. explicit numeric `plan_version`/version when timestamps tie;
3. filesystem-relative path as the stable final tie-breaker.

Record the selected source paths and the ordering in the archive. If metadata is missing, use the file timestamp only as a fallback and label it as inferred.

## Consolidation rules

Create one document that preserves planning evolution without blindly
concatenating duplicate text. Keep unchanged decisions as a concise carried
forward statement, but never compress away a source plan's unique goal,
scope, decision, risk, review finding, approval boundary, or validation
evidence.

The archive must be useful without reopening every temporary file. For every
source plan, create a plan-level ledger entry containing:

- source path, timestamp, version, status, owner/approver, and supersession;
- exact task or batch identity;
- goal and success criteria;
- current-state evidence and important implementation seams;
- included/excluded scope, dependencies, risks, and stop/replan conditions;
- review findings and the concrete corrections introduced;
- decisions that survived into the final implementation;
- validation commands/results and the implementation outcome.

If a source plan contains material detail that cannot be represented in the
ledger, include a full-fidelity source snapshot appendix in the same archive
or a clearly linked companion artifact. A filename inventory alone is never
an adequate consolidation.

The consolidated history must contain these sections when applicable:

- **Archive metadata**
- **Scope and source-plan inventory**
- **Planning timeline**
- **Initial plan and requirements**
- **Review feedback and revisions**
- **Approval record**
- **Implementation changes**
- **Re-planning and deviations**
- **Originally planned vs actually implemented**
- **Final implemented plan**
- **Validation and review evidence**
- **Traceability**
- **Residual risks and follow-up**
- **Per-plan decision and evidence ledger**
- **Final decision register**
- **Implementation evidence map**
- **Source snapshots or recovery appendix**

The final implemented plan must describe the delivered behavior, not merely
repeat the last draft. Every material deviation must state its reason and
supporting evidence, or be marked as unresolved. Repeated plans may be
summarized only after their unchanged content is mapped to the plan that
introduced it and their unique content is retained.

## Traceability and metadata

Use YAML front matter with machine-readable values. Include known values and use `null` or an empty list for unavailable optional values; do not invent links, IDs, commits, or approvals.

```yaml
kind: implemented-plan-archive
status: Archived
userStory: null
userStoryTitle: null
userStoryReference: null
githubIssue: null
githubIssueTitle: null
githubRepository: null
parentIssue: null
relatedIssues: []
branch: null
worktree: null
pullRequest: null
commits: []
mergeCommit: null
release: null
change: null
planCreatedAt: null
implementationCompletedAt: null
archivedAt: null
planner: null
reviewers: []
relatedADRs: []
knowledgeReferences: []
sourcePlans: []
```

Preserve the chain, when available:

`User Story → GitHub Issue → Implementation Plan → Worktree/Branch → Commits → Pull Request → Tests → Implemented Result`

Capture acceptance criteria, parent/related/dependency issues, relevant ADRs, knowledge authorities, review identities, and test commands/results. Separate agent-reported evidence from filesystem, Git, test, CI, or hosted evidence.

Use a final decision register to answer, for each material decision, when it
was introduced, which plan approved it, whether implementation confirmed or
changed it, and where the supporting code/test evidence lives. Use an
implementation evidence map to connect task groups to changed files, symbols,
migrations, tests, and actual command results. Do not claim a file or symbol
was changed unless current worktree/Git evidence supports it.

## Archive validation and source handling

Before reporting success, verify that:

- the archive exists at the deterministic path and parses as Markdown with valid front matter;
- every discovered source plan is listed and its meaningful changes are
  represented by a non-empty ledger entry;
- every source plan has an outcome classification: `carried-forward`,
  `superseded`, `partially-implemented`, `implemented`, `rejected`, or
  `follow-up`;
- every material final decision maps to at least one source plan and one
  implementation or validation evidence location;
- story/issue identity, planning timeline, final implementation, deviations, validation, review, and available traceability references are present;
- the archive is readable from the current checkout and does not contain secrets or private key material.

Keep temporary source plans by default, because they are useful evidence and the archive must be recoverable. If cleanup is explicitly authorized, move source plans into a clearly named `archived-source/` folder only after archive validation; never delete them as part of ordinary archival. The archive must remain understandable if a future agent reads only the archive and the referenced implementation/test files.

## Knowledge-base use

When future planning should discover implemented history, add the archive location to the appropriate `KnowledgeBase/INDEX.md` route only when the repository's knowledge-maintenance workflow calls for it. Do not bulk-load or duplicate all archived plans into the knowledge base. The archive is the historical source; the index is only the routing entry point.

## Handoff contract

Report:

- archive path and status;
- source-plan inventory and ordering basis;
- story/issue/branch/worktree/PR/commit traceability found;
- implementation, review, and validation evidence used;
- deviations and unresolved gaps;
- whether source plans were retained or moved;
- exactly one next safe action.

Use `Archived` only after all required evidence and archive checks pass.
