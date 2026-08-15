# Kickoff Playbook

Operational reference for running `us-kickoff`. Use these templates and conventions to keep story delivery consistent and traceable.

## Story status lifecycle

| Status | Meaning | Frontmatter |
| --- | --- | --- |
| `Ready_To_Implement` | DoR met, dependencies resolved, approved to start | `status: Ready_To_Implement` |
| `In_Progress` | Workstream active on an isolated branch/worktree | `status: In_Progress` |
| `Blocked` | Waiting on a dependency or an unresolved decision | `status: Blocked` |
| `PR_Open` | Branch pushed, PR created for review | `status: PR_Open` |
| `Done` | Merged and verified | `status: Done` |

Advance `status` at each transition and stamp `updated` with the current date.

## Branch and worktree naming

- Branch: `codex/<story-id>-<slug>-<timestamp>` (e.g., `codex/RES-BE-001-resume-builder-backend-20260815`).
- Worktree: `.worktrees/<story-id>-<slug>`.
- Use `main` as `base_branch` (stories are seeded with `master`; normalize at kickoff).

## Dependency rules

- Frontend slices (`.1`) usually declare the backend slice (`.2`) as `Dependency:`.
- Backend slices declare their own upstream dependency (e.g., `AUTH-BE-001`).
- A slice is only parallelizable when its full dependency chain is resolved or owned in a sequenced lane.

## Agent assignment

| Delivery shape | Agent | Loads |
| --- | --- | --- |
| backend | `backend-implementer` | `$dotnet-backend-patterns`, `$backend-feature-development` |
| frontend | `frontend-implementer` | `$frontend-dev-guidelines`, `$frontend-developer`, `$react-patterns` |
| full-stack | `story-orchestrator` | `$us-kickoff`, `$full-stack-feature-orchestrator` |
| architecture | `story-orchestrator` + `architect-review` | `$architect-review`, `$architecture-decision-records` |
| research | `knowledge-researcher` | `$deep-research`, `$project-knowledge-builder` |

## Approval summary template

```markdown
## Kickoff plan

**Ready:** <count> of <total> selected stories.

### Sequenced order
1. <story-id> — <title> (backend) — agent: backend-implementer
2. <story-id> — <title> (frontend, depends on 1) — agent: frontend-implementer
...

### Parallel groups
- Group A: <story-id>, <story-id> — independent, non-conflicting
- Group B: <story-id> — holds the shared contract/migration lane (not parallel)

### Conflict risk
- <shared surface> touched by <stories> — kept in one lane

### Not ready
- <story-id> — <reason (missing DoR, unresolved dependency)>

### Verification
- backend: dotnet build + dotnet test
- frontend: npm run check + npm run build

Approve to create branches/worktrees and start?
```

## Workstream report template

```markdown
## Workstream report — <story-id>

- **Touched areas:** <layers/files>
- **Verification:** <commands run and results>
- **Contract/migration impact:** <yes/no + detail>
- **Blockers:** <none | list>
- **PR readiness:** <branch | pr_url | remaining work>
```

## Definition of Done checklist per workstream

- [ ] Backend build and tests pass (or frontend `npm run check` + `npm run build`).
- [ ] Story acceptance criteria are demonstrably met.
- [ ] Contract, migration, or shared-file conflicts are reported.
- [ ] PR is opened with story traceability (`Refs <story-id>`) and verification notes.
- [ ] Story frontmatter is updated (`status`, `branch`, `worktree_path`, `pr_url`).
