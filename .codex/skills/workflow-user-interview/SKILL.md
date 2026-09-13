---
name: workflow-user-interview
description: Systematically resolve a plan's material design decisions through a repository-first, dependency-aware interview with the active user. Use when another skill needs shared understanding of scope, behavior, constraints, tradeoffs, or approval before proceeding.
---

# User Interview

Use this skill to establish shared understanding of a plan before the calling skill proceeds. It owns the interview interaction and decision record; the calling skill owns its artifact, plan, research, or implementation workflow.

## Use When

- repository evidence and the request leave a material decision unresolved
- the task depends on the user's goal, audience, priority, constraint, definition of success, or approval
- an existing requirement, research finding, or design choice needs confirmation from the user

## Do Not Use When

- the answer is available from the request, repository, or authoritative source
- a question would not change scope, design, verification, or the next action
- the task needs external participant research; use `$research-deep` to plan and synthesize that work

## Interview Workflow

1. Frame the outcome, known facts, constraints, and decision the calling workflow needs.
2. Explore relevant repository evidence before selecting a material branch. Do not ask the user for an answer already available from an authoritative source.
3. Keep only the compact state needed for the next question. Use one session-scoped local state artifact under root `tmp/` only when the rolling state is no longer compact, the user explicitly asks to preserve it, or a real handoff/interruption requires persistence. Batch artifact updates at those checkpoints, not after every answer. Artifact operations are internal: do not announce file creation, reads, updates, pruning, or paths unless the user asks.
4. Select the highest-value unresolved branch, then ask one focused question. Prefer the host's structured UI; otherwise use a lettered chat prompt with `◯` single-select or `☐` multi-select options. Offer 2-6 relevant choices and end with `Other — type your own answer` whenever options are appropriate.
5. Accept free text. Reduce each answer to only the facts, gaps, conflicts, and dependencies that can affect later work; preserve the source as `User-provided`, `evidence-backed`, or `inferred`.
6. Adapt to the updated state, revisit only immediate dependencies, and stop when enough information exists to satisfy the request. Do not retain question history, old options, reasoning traces, duplicate facts, or completed branches.
7. Present confirmed requirements, assumptions and inferences, open questions, and the precise next action. Request confirmation before treating the interview as complete.

Read [question-design.md](references/question-design.md) for branch selection, state-artifact storage and pruning, question formats, and answer handling.

## Completion Gate

End when the compact state is sufficient, all material gaps or conflicts are resolved or disclosed, and all inferences are explicit. If this is impossible, document the blocking unknowns, their owner, and the decision each prevents.

## Approval Gate

When the calling workflow requires approval, ask for explicit approval of the presented plan or decision. Do not infer approval from continued conversation, a preference answer, or a request to explore options.

## Reference

Read [question-design.md](references/question-design.md) when selecting question forms, mapping design branches and dependencies, handling conflicting evidence, or interviewing about a user workflow.
