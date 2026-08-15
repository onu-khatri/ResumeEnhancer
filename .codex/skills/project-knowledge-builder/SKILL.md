---
name: project-knowledge-builder
description: Build durable, evidence-grounded knowledge artifacts for ResumeEnhancer through an approval-driven workflow with sequential quality gates A–H, including a mandatory user-interview gate. Use when an agent needs reusable knowledge about architecture, feature flows, conventions, responsibilities, or requirement-to-implementation traceability.
---

# Project Knowledge Builder

Turns ResumeEnhancer investigation into reusable `KnowledgeBase/` artifacts. Two rules drive everything:

1. **Evidence first** — every claim is `Observed` or `Inferred` from real files; generic framework advice is never presented as repo fact.
2. **Approval + no assumptions** — the user approves the plan and the draft, and the agent must interview the user (Gate D) instead of guessing.

Default authoring preferences for this repository's knowledge artifacts:

- Prefer **short code snippets** as the primary evidence style for important claims.
- Keep **every included section self-sufficient**: an agent should understand the section's main behavior without reopening the code for first-pass comprehension.
- Use raw file-path references sparingly. When references are needed, prefer **interface or type names linked to their files** over general file inventories.
- If a section cannot be explained clearly with a snippet, either add the snippet or narrow the section; do not leave the agent dependent on "go read the code" as the main path.
- Keep the artifact template lightweight by default: start from a small core set of sections, then add extra sections only when the topic genuinely needs them and the user approves them.

## Use when

"Build knowledge for X flow", "document how validation works", "capture conventions for adding an endpoint", onboarding/review/planning knowledge.

## Don't use when

A one-off answer is enough and no saved artifact is needed.

## Workflow (do in order)

| # | Step | Gate |
|---|------|------|
| 1 | Check `KnowledgeBase/` for an existing artifact; extend it or start fresh | — |
| 2 | Frame the topic and investigate, keeping an evidence map | A |
| 3 | Interview the user (framing: objective, audience, depth of knowledge, expected structure, applicability, scope, core-section selection, evidence style, extra-section approval) via the `question` tool | — |
| 4 | Write `KnowledgeBase/<topic-name>.kb_plan.md`; **wait for approval** | — |
| 5 | Compose `KnowledgeBase/<topic-name>.pre-knowledge.md` with snippet-first, self-sufficient sections | B–C |
| 6 | **Interview the user again** (resolve ambiguities, preferences, options; confirm core sections and any discovered extra sections) | D |
| 7 | Validate: consistency, boundary, currency, record | E–H |
| 8 | Cross-examine the artifact; fix defects; **wait for draft approval** | — |
| 9 | Save `KnowledgeBase/<topic-name>.knowledge.md`, report, and ask whether to delete `*.kb_plan.md` and `*.pre-knowledge.md` | H |

Gates A–H (defined in `references/knowledge-quality-gates.md`):

- **A Grounding** — evidence-backed, not generic.
- **B Specificity** — names this repo's modules, symbols, paths.
- **C Reproducibility** — a cold-start agent can follow every step.
- **D User interview** — ask the user, don't assume; confirm core sections and get approval for any extra sections you discovered.
- **E Consistency** — frontmatter, refs, body, clarifications agree.
- **F Boundary** — limits and anti-patterns stated.
- **G Currency** — dated; high-risk claims re-checked this session.
- **H Record** — `validation` block lists A–H results.

## Non-negotiables

- Read files before concluding; label claims `Observed` (file-cited) / `Inferred` (combined evidence) / `Recommended` (fit, not yet fact).
- Never touch app code, tests, packages, migrations, or runtime config.
- Never save `*.knowledge.md` until the user approves `*.pre-knowledge.md`.
- After saving `*.knowledge.md`, explicitly ask the user whether the intermediate `*.kb_plan.md` and `*.pre-knowledge.md` files should be deleted. Do not delete them without approval.
- State evidence gaps explicitly instead of filling them from generic knowledge.
- Save artifacts under `KnowledgeBase/`.
- Do not skip the user interview just because the initial prompt already contains some scope details; confirm unresolved preferences explicitly.
- If the prompt does not explicitly provide `Objective`, `Audience`, `Depth of knowledge`, `Expected structure`, or `Applicability`, ask for them and do not assume them.
- Do not rely on path-only evidence where a short snippet would better ground the claim.
- Do not leave a kept section too thin; every included section should contain enough concrete snippet material that a cold-start agent can reason from the artifact itself.

## Artifact status

- `draft` — plan, or a draft that failed a gate.
- `reviewed` — passed A–H + cross-examination, user-approved.
- `stable` — `reviewed` + re-verified against current code this session.

## Evidence sources (in order)

`README.md` → `Business-Requirements/` → `User-Stories/` → `application/` → `test/`.

## References

- `references/repository-topography.md` — where to find evidence.
- `references/knowledge-quality-gates.md` — gates A–H in full.
- `references/knowledge-plan-template.md` — `*.kb_plan.md` shape.
- `references/knowledge-artifact-template.md` — `*.pre-knowledge.md` / `*.knowledge.md` shape.
- `references/artifact-validator-checklist.md` — granular companion checks.
- `references/artifact-cross-examination.md` — self-adversarial review.
