# Story Shaping

Use this reference when selecting a story type, breaking down a capability, or turning requirements into testable acceptance criteria. It supports authoring only; `$us-kickoff` owns delivery sequencing, worktrees, and agent assignment.

## Story Types

| Type | Add to the story pack |
| --- | --- |
| Feature | Persona, desired outcome, user flow, normal and relevant alternate scenarios, measurable success signal if known. |
| Defect | Current behavior, expected behavior, reproducible conditions, affected users, impact, and a regression scenario. |
| Technical debt | Current constraint, concrete risk, scope of remediation, preserved behavior, and verification of the intended improvement. |
| Research/spike | Decision question, evidence plan, boundary, deliverable, unresolved risks, and next decision. |

## Slicing Guide

Prefer a vertical slice that gives a persona a demonstrable outcome across necessary layers. Split only when each slice still has its own value, clear acceptance criteria, and manageable dependency surface.

Useful split dimensions are workflow stage, distinct business rule, user role, data variation, simple-to-complex capability, or separately releasable platform behavior. Do not split solely by developer ownership or technical layer unless an explicit shared contract, migration, or architecture boundary makes a coordinating lane necessary.

When a dependency cannot be avoided, name the prerequisite, its owner, and the reason. If scope is too uncertain to estimate, create a research story before implementation rather than hiding uncertainty in a feature story.

## Acceptance Criteria

Choose the least complex form that makes behavior testable:

- **Given / When / Then**: stateful workflows, permissions, validation, transitions, or recoverable failures.
- **Checklist**: simple independent outcomes such as a rendered control, persisted value, or exported field.
- **Decision table**: combinations of user role, entitlement, request state, input class, or business rule.

Each criterion should name an observable result. Include only relevant cases from this set: successful path, validation failure, empty state, operational failure and recovery, unauthorized or forbidden action, duplicate/concurrent action, accessibility behavior, and a measurable non-functional constraint. Do not use untestable wording such as "works well," "fast," or "user-friendly"; specify the outcome, threshold, or test method when known.

## INVEST Review

Use INVEST as a quality check, not a reason to hide real dependencies:

- **Independent**: independently demonstrable where feasible; otherwise the dependency is explicit and sequenced.
- **Negotiable**: the outcome and acceptance criteria are fixed, while nonessential design and implementation choices remain open.
- **Valuable**: the persona or business value is clear, including risk reduction for non-user-facing work.
- **Estimable**: scope, constraints, and unknowns are sufficient for delivery planning; otherwise create a research slice.
- **Small**: bounded enough to deliver and validate without becoming an epic; split by meaningful value, not by technical task.
- **Testable**: acceptance criteria have objective outcomes and a plausible verification approach.
