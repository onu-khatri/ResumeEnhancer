Use `$us-kickoff` to evaluate the selected ResumeEnhancer user stories and prepare a kickoff plan.

- Verify readiness: each story's `status` must be `Ready_To_Implement` and its `Definition Of Ready For Engineering` met.
- Resolve `Dependency:` fields into a topological order; sequence backend slices before their frontend dependents.
- Classify delivery shape and assign a focused agent per story (backend-implementer, frontend-implementer, knowledge-researcher, story-orchestrator).
- Identify shared files, migration risk, and contract conflicts before parallelizing.
- Stop for a short human approval checkpoint with the dependency order, parallel groups, and conflict risks.
- Recommend `codex/<story-id>-<slug>-<timestamp>` branches or `.worktrees/<story-id>-<slug>` worktrees per story.
- Track each story's frontmatter `status`, `branch`, `worktree_path`, and `pr_url` through to PR-ready.
