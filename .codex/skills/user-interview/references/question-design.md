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

Explore the repository for each branch before interviewing the user. Use this tree only to select the current branch; reduce its result into compact state and discard completed branches rather than retaining a full tree.

## Compact Interview State

Keep only these durable items:

| State item | Keep | Remove when |
| --- | --- | --- |
| Confirmed facts | Atomic user decisions and authoritative repository facts, each labeled `User-provided`, `evidence-backed`, or `inferred`. | Never remove a fact that can affect the final result. Deduplicate equivalent facts. |
| Important gaps | Missing information that can change the next action or result. | It is answered, becomes irrelevant, or is explicitly deferred with an owner. |
| Unresolved conflicts | Conflicting facts and the decision they prevent. | The governing source is chosen or the conflict becomes immaterial. |
| Current branch | The one material branch and immediate dependencies needed for the next question. | The next question changes branch. |

For an ordinary interview, keep this state only in the rolling context. Create `tmp/user-interview/<session-id>/state.md` from the repository root only when the rolling state is no longer compact, the user explicitly requests persistence, or a real handoff/interruption is imminent. Use the host session ID when available; otherwise generate a timestamp or UUID. Never reuse a prior session directory. Update that one artifact in place only at those persistence checkpoints; do not write a file for ordinary turn-to-turn state. Read only the current branch and its linked facts, gaps, conflicts, and dependencies; do not reload the entire file or reconstruct conversation history. Treat all artifact operations as internal; question messages must not mention file creation, reads, updates, pruning, or paths unless the user asks. A host may visibly report any file write, so minimizing writes is the only reliable way to avoid repeated edit notifications.

## Dependency Order

Ask parent decisions before children. For example, resolve audience and outcome before workflow, workflow before rules and data, and rules/data before integration, security, verification, and rollout. When an answer changes an upstream decision, revisit its dependent branches rather than continuing with stale assumptions.

## Question Formats

| Decision shape | Format | Requirements |
| --- | --- | --- |
| One mutually exclusive choice | Single-select | Label options `A. ◯`, `B. ◯`, and so on; offer 2-6 concrete choices and end with a lettered `◯ Other — type your own answer`. In chat, request one letter. |
| Several independent choices | Multi-select | Label options `A. ☐`, `B. ☐`, and so on; offer 2-6 meaningful choices and end with a lettered `☐ Other — type your own answer`. In chat, request one or more comma-separated letters. |
| Workflow discovery, custom semantics, or unknown solution space | Free-form | Ask for one recent concrete example or explanation. Do not offer choices when they would constrain the answer, including when resolving a mixed or unclear option response. |
| Confirmation or correction | Binary or single-select | State the proposed interpretation, its practical effect, and a custom correction path. |

Each option must be distinct, understandable without hidden jargon, and describe a real consequence when the tradeoff is material. Do not add a false "no preference" option when choosing is necessary; use `Not sure` only when uncertainty is itself actionable.

## Mixed And Unclear Answers

Accept free-text answers whether or not the user selects an option. Extract only the compact facts, gaps, conflicts, and dependencies needed later; discard the raw answer and options after reduction unless exact wording itself is a requirement. Treat only an explicitly selected option as selected. Do not infer a relationship between selections or explanations: in particular, do not invent a sequence, priority, dependency, tradeoff, or approval that the user did not state.

When a reply does not clearly answer the primary question, ask a short plain-text re-question instead of creating new options or proposing an interpretation. State only what is unclear and repeat the original decision. For example:

> I could not determine one delivery model from that reply. Please answer the delivery-model question again with one option, or describe your preferred model in your own words.

Use a question without options whenever a free-form explanation is the more reliable way to discover intent, a real workflow, custom terminology, an `Other` answer, or the meaning of an unclear response. Do not interrupt an answer merely because it includes both letters and explanatory text.

Example:

> Which rollout boundary should govern this change? Select one.
>
> A. ◯ Release with the next planned deployment; no feature flag.
> B. ◯ Release behind a feature flag for staged enablement.
> C. ◯ Deliver the implementation only; defer production rollout.
> D. ◯ Other — type your own answer.
>
> Reply with `A`, `B`, `C`, or `D` plus your preferred boundary.

## Internal Sequence

Before every next question, perform this sequence internally:

1. Reduce the answer to atomic facts with their source and any immediate dependency, gap, or conflict.
2. Update the rolling context, then remove the answered question, old options, reasoning trace, duplicate facts, resolved gaps, and completed branches. At a persistence checkpoint only, update the one existing state artifact with the compact state; otherwise do not write a file.
3. Compare only the current branch and immediate dependencies with repository evidence, constraints, and non-goals.
4. Stop if the compact state is sufficient for the requested result; otherwise select the highest-value remaining gap and ask one focused question.

Periodically prune the state artifact to the four state items above. It is a compact decision record, not a transcript, Q/A log, or reasoning trace.

## Answer Handling

- For workflow discovery, prefer a recent example over a hypothetical preference.
- Keep user statements separate from interpretation. Explain repository evidence that conflicts with an answer, then ask which source governs.
- Treat a mixed or ambiguous reply as an unanswered decision: state what is unclear and re-ask in plain text. Do not restate an inferred interpretation.
- Treat an explicit conflict as two competing facts: state their practical consequence and ask which one governs. Do not average or overwrite them.
- Treat `Not sure` as meaningful uncertainty. Offer a clearly labeled reversible default only when it is safe; otherwise identify the owner or information needed.
- When `Other` is selected, map the explanation to the current branch and follow up only if its scope or consequence remains unclear.
- Periodically check the compact state for consistency, feasibility, missing edge cases, and traceability. Stop when it is sufficient for the request.

## Completion And Summary

The interview is complete when the compact state is sufficient to satisfy the request, no remaining gap or conflict can change the calling workflow's next action, and all `inferred` facts are explicit. Before the approval question, derive this summary from the compact state:

| Section | Include |
| --- | --- |
| Confirmed requirements | Explicit user decisions and evidence-backed facts that govern the work. |
| Assumptions and inferences | Agent interpretations, defaults, and the reason each is safe or necessary. |
| Open questions | Deferred, blocked, or unresolved items, their owner, and the decision each prevents. |

Close with the precise proposed next action and request confirmation. For workflows requiring approval, confirmation must explicitly authorize the plan or decision; it is not implied by answering interview questions.

## Useful Prompts

- "What outcome must this change enable for you?"
- "Tell me about the most recent time you handled this workflow. What triggered it and what happened next?"
- "Which constraint is fixed here: scope, timeline, compatibility, or behavior?"
- "The repository evidence suggests `<fact>`, while your request implies `<need>`. Which should govern this change?"
- "Does this plan accurately reflect your decision and authorize the listed edits?"
- "This decision changes `<dependent branch>`. Before we continue, should that branch follow the same constraint or a different one?"
