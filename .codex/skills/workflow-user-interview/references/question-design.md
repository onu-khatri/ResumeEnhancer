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

For an ordinary interview, keep this state only in the rolling context. Create `tmp/workflow-user-interview/<session-id>/state.md` from the repository root only when the rolling state is no longer compact, the user explicitly requests persistence, or a real handoff/interruption is imminent. Use the host session ID when available; otherwise generate a timestamp or UUID. Never reuse a prior session directory. Update that one artifact in place only at those persistence checkpoints; do not write a file for ordinary turn-to-turn state. Read only the current branch and its linked facts, gaps, conflicts, and dependencies; do not reload the entire file or reconstruct conversation history. Treat all artifact operations as internal; question messages must not mention file creation, reads, updates, pruning, or paths unless the user asks. A host may visibly report any file write, so minimizing writes is the only reliable way to avoid repeated edit notifications.

## Dependency Order

Ask parent decisions before children. For example, resolve audience and outcome before workflow, workflow before rules and data, and rules/data before integration, security, verification, and rollout. When an answer changes an upstream decision, revisit its dependent branches rather than continuing with stale assumptions.

## Native Codex Question UI

Choose the transport from the live session capabilities, not the editor name:

| Available capability | Action |
| --- | --- |
| `request_user_input`, permitted for this mode and purpose | Use it for the current interview decision; it waits for a structured answer. |
| Only `request_user_input_async` is permitted | Ask through it, retain the pending decision, and process the later user message before dependent work. |
| No suitable native tool, including non-interactive CLI sessions | Ask one concise chat question using the fallback below; do not simulate a tool call. |
| Approval/permission needed but the question tool prohibits it | Use the host approval path or an explicit chat approval question. |

Tool namespaces and schemas vary by host. Check their current definitions.
Where `request_user_input` is Plan-mode-only, use it only in Plan mode. `/plan`
is a user-facing client command, not a shell command or a tool the skill can
invoke to change its mode. Do not block a useful interview just to obtain a UI.
The client renders the native UI; do not automate VS Code clicks or terminal
keypresses, install MCP servers, or call app-server RPC methods to manufacture it.

For the usual synchronous schema, send one item in `questions` with a stable
snake_case `id`, a short `header`, a self-contained `question`, and 2-3 options
with short `label` and consequence-focused `description` fields. Follow the
live limits if they differ. Put an evidence-backed recommendation first and
mark it `(Recommended)` when the schema asks for that. Do not invent a preferred
answer when evidence does not support one. Do not add letter prefixes, checkbox
glyphs, or `Other` when the native UI already supplies free-text input.

Illustrative `request_user_input` arguments (adapt to the live schema):

```json
{
  "questions": [{
    "id": "rollout_scope",
    "header": "Rollout",
    "question": "Which rollout scope should the proposal target?",
    "options": [
      {"label": "Internal pilot (Recommended)", "description": "Validate with internal users before a wider release."},
      {"label": "All users", "description": "Prepare a general release with broader verification."}
    ]
  }]
}
```

Use the recommendation above only when current evidence supports a pilot.
This question selects proposal scope; it does not approve deployment.

For `request_user_input_async`, the currently exposed schema may instead use
`questions: [{title, options}]` with string options. Do not pass synchronous
fields to it. Omit options for free-form questions when supported. It returns
before the user answers: correlate the later answer with the pending question,
do not repeat it while waiting, and continue only independent safe work.

Do not assume either tool supports multi-select. When several independent
choices are needed, split them into focused decisions or request a free-text
list through a supported tool. Use chat checkboxes only as the fallback.
Do not request uploads or screenshots through a text-only question tool.

An empty, cancelled, or timed-out response is unanswered. For optional
preferences, follow the host's no-answer behavior and proceed with a disclosed,
reversible assumption when appropriate; do not repeatedly ask. A required
product decision or explicit approval remains unresolved, and its dependent
work must wait. If the native tool is restricted to optional questions, use
another permitted channel for required decisions. Preserve an actual user's
answer rather than the displayed default; interpret free text on equal terms.

## Chat Fallback Question Formats

The following formatting applies only when no permitted native tool fits. It is
not a schema for native Codex tools.

| Decision shape | Format | Requirements |
| --- | --- | --- |
| One mutually exclusive choice | Single-select | Label options `A. ◯`, `B. ◯`, and so on; offer 2-6 concrete choices and end with a lettered `◯ Other — type your own answer`. In chat, request one letter. |
| Several independent choices | Multi-select | Label options `A. ☐`, `B. ☐`, and so on; offer 2-6 meaningful choices and end with a lettered `☐ Other — type your own answer`. In chat, request one or more comma-separated letters. |
| Workflow discovery, custom semantics, or unknown solution space | Free-form | Ask for one recent concrete example or explanation. Do not offer choices when they would constrain the answer, including when resolving a mixed or unclear option response. |
| Confirmation or correction | Binary or single-select | State the proposed interpretation, its practical effect, and a custom correction path. |

Each option must be distinct, understandable without hidden jargon, and describe a real consequence when the tradeoff is material. Do not add a false "no preference" option when choosing is necessary; use `Not sure` only when uncertainty is itself actionable.

## Mixed And Unclear Answers

Accept free-text answers whether or not the user selects an option. Extract only the compact facts, gaps, conflicts, and dependencies needed later; discard the raw answer and options after reduction unless exact wording itself is a requirement. Treat only an explicitly selected option as selected. Do not infer a relationship between selections or explanations: in particular, do not invent a sequence, priority, dependency, tradeoff, or approval that the user did not state.

When a reply does not clearly answer the primary question, ask a short free-form re-question through a permitted native tool that supports it, or in chat otherwise. Do not invent new options or propose an interpretation. State only what is unclear and repeat the original decision. For example:

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
- Treat a mixed or ambiguous reply as an unanswered decision: state what is unclear and re-ask in free form through a supported native tool or chat fallback. Do not restate an inferred interpretation.
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

## Validation Scenarios

Review these cases whenever the tool routing changes. Structural validation
does not prove that either client rendered a question.

| Case | Expected behavior |
| --- | --- |
| VS Code or CLI exposes a permitted synchronous tool | One native question, valid ID/header/options, no duplicate `Other`. |
| Synchronous tool requires Plan mode but session is Default | No invalid call; use permitted async tool or chat. |
| Async answer arrives after independent work | Reconcile the pending question before dependent work; no duplicate prompt. |
| User types a custom answer | Preserve its meaning without forcing it into an option. |
| Optional question returns no answer | Follow host policy; no retry loop and no claim of user consent. |
| Required decision or approval is unanswered | Keep dependent work pending; never auto-approve. |
| Tool lacks multi-select or free-form support | Adapt to supported fields or use chat fallback. |
| Non-interactive CLI has no question tool | Clear text question/blocker; no invented UI or mode switch. |

Official references, checked 2026-09-26:
- [CLI and extension commands, including Plan mode](https://learn.chatgpt.com/docs/developer-commands)
- [App-server question transport](https://learn.chatgpt.com/docs/app-server#toolrequestuserinput)

The app-server transport documents client plumbing, not a substitute for the
model-visible tool contract. Runtime instructions govern availability, schema,
and permitted purposes.
