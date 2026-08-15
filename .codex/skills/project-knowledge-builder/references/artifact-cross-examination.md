# Artifact Cross-Examination

This is a self-adversarial cross-examination of the **artifact**, run after Gate H (Validation Record) completes. It is NOT the user interview — Gate D in `knowledge-quality-gates.md` is the user interview, which uses the `question` tool to ask the user and runs earlier (after Gate C). This check interrogates the finished artifact as if a skeptical new agent had never seen the originating conversation.

Run this after the validator checklist and quality gates pass. An artifact that cannot answer a question must either be fixed or explicitly flag the question as out of scope in its `validation` block.

Work through each lens. For each unanswered question, decide: fix now, or record as a known gap. Silence is a defect.

## Lens 1 — Onboarding (a brand-new agent)

- "What is this artifact for, in one sentence?" — must be answerable from `intent` alone.
- "Where do I start reading the code for this topic?" — the artifact must name a concrete first file or entry point.
- "What terms do I need defined before I can proceed?" — every repo-specific acronym must be defined at first use.
- "Can I trust this, or is some of it guesswork?" — `Observed`/`Inferred`/`Recommended` labels must be visibly applied.
- "Do I need to open the repo immediately, or can I understand the first-pass model from this artifact?" — each major section should carry enough snippet-level grounding to stand on its own.
- "Were these sections chosen by the user?" — the `Clarifications` section must show the section multi-select outcome from Gate D; the agent must not have decided which sections to include.

## Lens 2 — Implementation (an agent about to write code)

- "If I add a new X, exactly which files do I touch and in what order?" — the extension pattern must be a numbered sequence of concrete touch points.
- "What will break if I get this wrong?" — the artifact must name a concrete failure mode, not a vague warning.
- "Which existing code can I copy as a reference?" — the artifact must point to a real, existing example.
- "Can I see the copy shape without hunting through the repo?" — the artifact should embed the most important example snippets instead of only naming files.
- "Where are the contracts I must not violate?" — the artifact must name the contract type and its owning module (e.g., request/response contracts in `<ModuleName>ModuleAM`).

## Lens 3 — Review (an agent reviewing a diff)

- "How do I tell a correct change from an incorrect one?" — the artifact must state a verifiable rule, not a preference.
- "Which layers must this change not cross?" — boundary rules must be explicit (e.g., HTTP stays in `<ModuleName>ModuleWeb`).
- "What tests prove this still works?" — the artifact must name a concrete test project or command.

## Lens 4 — Planning (an agent sequencing work)

- "What must exist before this can be built?" — dependencies/preconditions must be named.
- "Which changes are serial versus parallel?" — if relevant, conflict surfaces must be called out.
- "What is the requirement-to-code traceability path?" — the artifact must link a product doc or user story to the implementation.

## Lens 5 — Maintenance (an agent asked to change it later)

- "Why was this decision made?" — design rationale must be traceable to a requirement or an observed constraint.
- "What is the least risky way to extend this?" — the extension pattern must describe the safe path.
- "What is now stale, and how would I know?" — the `last_reviewed` date and any known drift must be stated.

## Cross-examination verdict

Count the questions that required a fix or a recorded gap:

- 0 unresolved — the artifact is cross-examination-clean; it is ready to present for user approval of the `.pre-knowledge.md` draft (`reviewed` after approval, or `stable` if re-verified against current code this session).
- 1–2 resolved gaps recorded in `validation` — acceptable, but note the gaps before presenting for approval.
- 3+ unresolved — the artifact is not ready; keep it `draft`, fix the defects, and re-run the cross-examination.

Record the verdict (`clean` / `acceptable-with-gaps` / `not-ready`) in the `validation.cross_examination` field. This verdict is a readiness signal for the draft-approval gate; the final `*.knowledge.md` save still requires explicit user approval.
