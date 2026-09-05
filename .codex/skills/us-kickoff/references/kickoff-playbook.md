# Kickoff Playbook

Operational reference for running `us-kickoff`. Use these templates and conventions to keep story-to-GitHub handoff consistent and traceable. Implementation execution belongs to `issues-kickoff`.

## Story status lifecycle

| Status | Meaning | Frontmatter |
| --- | --- | --- |
| `Ready_To_Implement` | DoR met, dependencies resolved, approved to start | `status: Ready_To_Implement` |
| `Move_To_GitHub_Issue` | Story has been handed off to a verified GitHub issue and is ready for implementation intake | `status: Move_To_GitHub_Issue` |
| `In_Progress` | Workstream active on an isolated branch/worktree | `status: In_Progress` |
| `Blocked` | Waiting on a dependency or an unresolved decision | `status: Blocked` |
| `PR_Open` | Branch pushed, PR created for review | `status: PR_Open` |
| `Done` | Merged and verified | `status: Done` |

`us-kickoff` may transition `Ready_To_Implement` to `Move_To_GitHub_Issue` only through a successful `$create-github-issue` handoff. `issues-kickoff` owns later status transitions and branch/worktree fields.

## Dependency rules

- Frontend slices (`.1`) usually declare the backend slice (`.2`) as `Dependency:`.
- Backend slices declare their own upstream dependency (e.g., `AUTH-BE-001`).
- A slice is only parallelizable when its full dependency chain is resolved or owned in a sequenced lane.

## Approval summary template

```markdown
## Kickoff plan

**Ready for GitHub handoff:** <count> of <total> selected stories.

### Sequenced order
1. <story-id> — <title> (issue 1; backend)
2. <story-id> — <title> (issue 2; frontend, depends on 1)
...

### Issue split
- <story-id> — one issue | split into <slice labels>

### Conflict risk
- <shared surface> touched by <stories> — kept in one lane

### Not ready
- <story-id> — <reason (missing DoR, unresolved dependency)>

### Verification
- backend: dotnet build + dotnet test
- frontend: npm run check + npm run build

Approve to create the GitHub issue handoff?
```

After handoff, stop. Use `issues-kickoff` for implementation workstream reporting and verification only after the user explicitly invokes it.
