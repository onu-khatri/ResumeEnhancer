---
name: orchestration-agent-improvement
description: Diagnose and improve delegation, ownership, checkpoints, and handoffs in existing ResumeEnhancer agent workflows.
---

# Agent Orchestration Improver

Use [the delegated-work communication protocol](references/delegation-protocol.md)
when assessing or improving parent/sub-agent status, lifecycle, observability,
or handoff behavior. It is the canonical state and event contract.

Use this skill to strengthen existing ResumeEnhancer orchestration flows instead of inventing clever delegation for its own sake.

## Use this skill when

- a story or initiative is large enough to split across specialized agents
- an existing kickoff or background-agent flow is producing overlap, rework, or missed context
- you need a safer pattern for branch isolation, sequencing, or synthesis

## Do not use this skill when

- one focused agent can complete the work safely end to end
- there is no baseline workflow, failure mode, or evaluation target to improve
- delegation would add ceremony without reducing risk

## Improvement workflow

1. Establish the baseline workflow, examples, and failure symptoms.
2. Separate the work into roles such as research, architecture, backend, frontend, review, and packaging.
3. Define what context each agent truly needs and what should stay with the parent coordinator.
4. Preserve explicit gates and existing user authorization. Ask only for destructive, externally consequential, out-of-scope, or unresolved material decisions; do not add routine approval pauses.
5. Validate the revised orchestration on realistic repository tasks before treating it as the new default.

## Review lenses

- context size and prompt clarity
- duplicated work across agents
- unsafe parallel edits to shared contracts, migrations, or cross-cutting UI state
- unclear synthesis ownership
- missing validation or rollback points

## ResumeEnhancer focus

- Use a subagent only for a bounded lane that materially improves the result. Prefer one agent for tightly coupled work.
- Reuse a suitable idle agent before creating another. Keep recursive delegation off unless the assignment explicitly permits it.
- Pass a focused manifest, not the entire conversation: identity, objective, owned/excluded paths, required authorities, plan reference, worktree, and expected evidence.
- Use read-only review lanes and exactly one writer per shared boundary. Wait through host status tools; a timeout alone is not failure.
- Keep lifecycle hooks observational until their behavior is tested in the active runtime. See the repository `.codex/README.md` for setup and the canonical protocol for lifecycle semantics.
- user-story kickoff with readiness checks
- isolated branches or worktrees per story
- frontend and backend split only after shared contract risks are known
- final synthesis that reports touched layers, verification, and blockers

## Output requirements

- baseline issues
- recommended delegation boundaries
- required approval checkpoints
- validation plan
- simplification guidance if orchestration becomes heavier than the task
