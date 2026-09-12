---
name: openspec-repository-policy
description: Apply ResumeEnhancer-specific policy alongside OpenSpec-generated Codex workflows.
license: MIT
metadata:
  author: ResumeEnhancer
  version: "1.0"
---

# ResumeEnhancer OpenSpec Repository Policy

OpenSpec-managed skills under `.agents/skills/` are generated files. Do not
edit them directly to add repository-specific behavior; `openspec update` owns
their contents and may replace them.

Keep project-specific policy in `AGENTS.md` and `.codex/skills/`. Apply this
skill as an overlay whenever an OpenSpec workflow is used:

- Read `AGENTS.md` and `KnowledgeBase/INDEX.md` before planning or delivery.
- Use `$development-entry-gate` before any development edit. Its result is a
  gate, not an implementation handoff by itself.
- Keep proposal artifacts in the main checkout and implementation work in the
  approved canonical worktree. Create or reuse a worktree only after proposal,
  Definition-of-Ready, approval, issue/change identity, base branch, and owner
  evidence are present.
- Keep `$openspec-orchestrator` and `$openspec-workflow` as the lifecycle
  authorities. The generated OpenSpec skill remains responsible for its own
  CLI contract, artifact context, and task checkbox rules.
- Preserve unrelated changes and distinguish local validation, commit, push,
  hosted PR state, and CI state in reports.

When a new OpenSpec release changes a generated workflow, review the generated
diff against this policy. Update this overlay only when the repository policy
itself changes; do not patch generated files to restore local rules.
