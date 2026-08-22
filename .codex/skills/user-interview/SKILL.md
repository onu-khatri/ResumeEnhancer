---
name: user-interview
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
- the task needs external participant research; use `$deep-research` to plan and synthesize that work

## Interview Workflow

1. Frame the plan: state the intended outcome, known facts, constraints, and the decision the plan must support.
2. Build a design tree of every material branch: user outcome, scope and non-goals, workflows and states, rules and edge cases, data and integration contracts, security/privacy, non-functional expectations, dependencies, rollout/verification, and approval. Omit a branch only when it demonstrably does not apply.
3. Explore the repository before asking about each branch. Read the relevant requirements, stories, code, tests, configuration, and established conventions. Mark the branch `evidence-backed` when the answer is available; do not ask the user to rediscover repository facts.
4. Order remaining branches by dependency. Resolve prerequisites before dependent decisions, then ask one focused question at a time. Prefer the available structured question UI; otherwise ask in chat and wait for the answer before continuing.
5. Use neutral, concrete wording. For past behavior or workflow discovery, ask for a recent example, trigger, actions, alternatives, constraints, and outcome rather than hypothetical preference. Probe when an answer remains ambiguous, conflicts with evidence, or opens a new material branch.
6. After each answer, update the design tree and revisit affected dependent branches. Continue until every material branch is evidence-backed, explicitly decided by the user, intentionally deferred with an owner, or documented as a blocking unknown.
7. Present a shared-understanding summary: decisions, evidence, assumptions, deferred items, blockers, and the precise effect on the calling skill's next step. Ask for confirmation before treating the interview as complete.

Record user answers as `User-provided`; do not relabel them as repository observations or externally validated research. Preserve uncertainty when the user is unsure. Do not replace a repository exploration with a question merely because asking is faster.

## Completion Gate

Do not end the interview after one answer or a superficial summary. End only after the design tree has been reviewed and the user confirms the shared-understanding summary, or after documenting the exact blocking unknowns that prevent that confirmation.

## Approval Gate

When the calling workflow requires approval, ask for explicit approval of the presented plan or decision. Do not infer approval from continued conversation, a preference answer, or a request to explore options.

## Reference

Read [question-design.md](references/question-design.md) when selecting question forms, mapping design branches and dependencies, handling conflicting evidence, or interviewing about a user workflow.
