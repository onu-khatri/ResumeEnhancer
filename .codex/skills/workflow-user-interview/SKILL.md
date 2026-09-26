---
name: workflow-user-interview
description: Resolve material scope and design decisions through a repository-first interview using Codex's native question tool when available.
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
4. Select the highest-value unresolved branch, then ask one focused question through the native Codex question tool permitted by the active session: prefer `request_user_input`, or use `request_user_input_async` when the synchronous tool is unavailable or disallowed. Follow the tool's current schema and mode/purpose restrictions. Use the lettered chat fallback only when no permitted native tool fits the question; its 2-6 choices and explicit `Other` option do not apply to native tool payloads.
5. Accept free text. Reduce each answer to only the facts, gaps, conflicts, and dependencies that can affect later work; preserve the source as `User-provided`, `evidence-backed`, or `inferred`.
6. Adapt to the updated state, revisit only immediate dependencies, and stop when enough information exists to satisfy the request. Do not retain question history, old options, reasoning traces, duplicate facts, or completed branches.
7. Present confirmed requirements, assumptions and inferences, open questions, and the precise next action. Request confirmation before treating the interview as complete.

Read [question-design.md](references/question-design.md) for branch selection, state-artifact storage and pruning, question formats, and answer handling.

## Codex Question Tools

In the Codex VS Code extension and interactive Codex CLI, use the exposed native question tool rather than drawing radio buttons in chat when that tool is callable and permitted. Check the live tool registry and active mode; a skill cannot enable a missing tool or switch modes. If `request_user_input` requires Plan mode, the user can select Plan mode or `/plan` in a supporting client before the interview. Continue with an available asynchronous tool or chat fallback instead of attempting an invalid call or forcing a mode change.

Ask one question per call by default. Use stable decision IDs and concise, mutually exclusive choices; rely on the UI's built-in free-text entry rather than adding a duplicate `Other` choice. For async questions, keep the decision pending while doing independent work; do not proceed with dependent work before a required answer arrives. Never treat a preselected option, empty response, timeout, or dismissed question as consent.

Use native interview tools only for purposes their tool contract permits. When approval or permission requests are prohibited, present the concrete decision in chat or use the host's dedicated approval mechanism; do not disguise approval as a preference question. See the reference for native payloads, multi-select limitations, fallback behavior, and validation scenarios.

## Completion Gate

End when the compact state is sufficient, all material gaps or conflicts are resolved or disclosed, and all inferences are explicit. If this is impossible, document the blocking unknowns, their owner, and the decision each prevents.

## Approval Gate

When the calling workflow requires approval, ask for explicit approval of the presented plan or decision. Do not infer approval from continued conversation, a preference answer, or a request to explore options.

## Reference

Read [question-design.md](references/question-design.md) when selecting question forms, mapping design branches and dependencies, handling conflicting evidence, or interviewing about a user workflow.
