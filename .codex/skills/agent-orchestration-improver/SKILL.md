---
name: agent-orchestration-improver
description: Improve how Codex decomposes and coordinates multi-agent work for ResumeEnhancer, especially for parallel story execution, research, review, and implementation. Use when a task is large enough to benefit from structured delegation, validation, and synthesis.
---

# Agent Orchestration Improver

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
4. Add short approval checkpoints before destructive or branch-shaping actions.
5. Validate the revised orchestration on realistic repository tasks before treating it as the new default.

## Review lenses

- context size and prompt clarity
- duplicated work across agents
- unsafe parallel edits to shared contracts, migrations, or cross-cutting UI state
- unclear synthesis ownership
- missing validation or rollback points

## ResumeEnhancer focus

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