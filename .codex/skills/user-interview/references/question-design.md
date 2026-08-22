# Question Design

Use this reference to make an interview complete without turning it into an unfocused questionnaire.

## Design Tree

Start from the requested outcome and inspect these branches for applicability:

| Branch | Resolve before proceeding |
| --- | --- |
| Outcome and audience | User value, affected users, and observable success. |
| Scope | Included behavior, non-goals, priorities, and acceptance boundary. |
| Workflow and state | Happy path, alternate paths, empty/loading/error states, and recovery. |
| Rules and data | Validation, permissions, ownership, lifecycle, and integration contracts. |
| Constraints | Security, privacy, accessibility, performance, compatibility, and operations. |
| Delivery | Dependencies, migration/rollout impact, verification, and approval. |

Explore the repository for each branch before interviewing the user. Mark a branch as `evidence-backed`, `user-decided`, `deferred`, or `blocked`; do not leave a material branch unclassified.

## Dependency Order

Ask parent decisions before children. For example, resolve audience and outcome before workflow, workflow before rules and data, and rules/data before integration, security, verification, and rollout. When an answer changes an upstream decision, revisit its dependent branches rather than continuing with stale assumptions.

## Choose The Question Form

| Need | Question shape |
| --- | --- |
| Resolve a product or scope choice | State the decision and options with their concrete tradeoffs. |
| Understand a real workflow | Ask for the most recent example, trigger, actions, workaround, and outcome. |
| Clarify an ambiguous requirement | Ask for the expected behavior, boundary, and observable success condition. |
| Confirm a plan | Ask whether the stated plan is approved, and identify any requested changes. |

## Interview Rules

- Ask open, neutral, one-part questions. Do not ask several decisions in one message.
- Prefer past behavior and specific examples to predictions such as "Would you use this?"
- Separate what the user said from the interpretation or proposed next action.
- Explain relevant evidence that conflicts with the answer, then ask which source should govern rather than silently choosing.
- Continue until every material design-tree branch is evidence-backed, user-decided, deferred with an owner, or blocked.

## Useful Prompts

- "What outcome must this change enable for you?"
- "Tell me about the most recent time you handled this workflow. What triggered it and what happened next?"
- "Which constraint is fixed here: scope, timeline, compatibility, or behavior?"
- "The repository evidence suggests `<fact>`, while your request implies `<need>`. Which should govern this change?"
- "Does this plan accurately reflect your decision and authorize the listed edits?"
- "This decision changes `<dependent branch>`. Before we continue, should that branch follow the same constraint or a different one?"
