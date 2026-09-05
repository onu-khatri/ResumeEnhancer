Use `$us-kickoff` to evaluate the selected ResumeEnhancer user stories and prepare an approved GitHub issue handoff plan. After the issues are verified and stories move to `Move_To_GitHub_Issue`, stop. The user must explicitly invoke `$issues-kickoff` later to search and select implementation issues.

- Verify readiness: each story's `status` must be `Ready_To_Implement` and its `Definition Of Ready For Engineering` met.
- Resolve `Dependency:` fields into a topological order; sequence backend slices before their frontend dependents.
- Classify delivery shape and assign a focused agent per story (backend-implementer, frontend-implementer, knowledge-researcher, story-orchestrator).
- Identify shared files, migration risk, and contract conflicts before parallelizing.
- Stop for a short human approval checkpoint before issue creation with the dependency/pick order, split decisions, and conflict risks.
- Invoke `$create-github-issue` to create and verify the issue set and update source stories to `Move_To_GitHub_Issue`.
- When explicitly invoked, use `$issues-kickoff` for top-10 GitHub issue intake, selection, `codex/<story-id>-<slug>-<timestamp>` branches, `.worktrees/<story-id>-<slug>` worktrees, implementation agents, and PR tracking.
