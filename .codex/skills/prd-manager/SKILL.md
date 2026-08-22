---
name: prd-manager
description: Create or update ResumeEnhancer PRDs in the root `prd/` folder from `Business-Requirements/*.BR.md`, `User-Stories/*.US.md`, and their linked supporting files. Use when the task is product-requirement authoring or maintenance, not implementation or story creation.
---

# PRD Manager

Create and maintain repository-grounded PRDs under `prd/`. This skill is for
turning ResumeEnhancer business requirements or story packs into a usable
product-requirements document, or for updating an existing PRD when the source
documents change.

## Use this skill when

- the user asks to create a PRD from a BR document
- the user asks to create a PRD from a user story
- the user asks to update, refresh, or reconcile a PRD in `prd/`
- the task is product framing, scope, goals, requirements, risks, or rollout

## Do not use this skill when

- the user wants a business requirement or user story written from scratch
- the task is implementation planning, OpenSpec change work, or code changes
- the request is for a technical design doc or ADR instead of a PRD

## Output location and naming

- Save every PRD in the repository root folder `prd/`.
- Create `prd/` if it does not exist.
- If the request is tied to one source file, name the PRD after that source
  stem and replace the source suffix with `.PRD.md`.
  - `Business-Requirements/resume-platform-foundation.BR.md` ->
    `prd/resume-platform-foundation.PRD.md`
  - `User-Stories/8.3 theme-accessibility-remediation-frontend.US.md` ->
    `prd/8.3 theme-accessibility-remediation-frontend.PRD.md`
- If a matching PRD already exists, update it in place instead of creating a
  duplicate.
- A BR document can contain multiple business-requirement clusters. Keep them
  in one PRD when they share a product outcome, scope boundary, and delivery
  decision. Split them into capability PRDs only when they are independently
  valuable, deliverable, or governed; use
  `<source-stem>--<capability>.PRD.md` and cite the relevant BR sections in
  each file.

## Required grounding

Always work from repository evidence first.

### BR-driven PRDs

Read the requested `Business-Requirements/*.BR.md` file first. Use its problem,
scope, objectives, business rules, constraints, success measures, and linked
downstream documents as the primary source of truth.

Before selecting the output shape, identify the BR's coherent requirement
clusters. A domain BR commonly contains multiple requirements; do not omit or
flatten them into one vague requirement. Preserve related clusters in one PRD
with distinct requirement identifiers, or split independent clusters according
to the output naming rule above.

### User-story-driven PRDs

Read the requested `User-Stories/*.US.md` file first. Also read the sibling
`.SI.md` and `.Research.md` files when they exist. Treat the story file as the
launch boundary, and use supporting files to enrich requirements, risks,
technical implications, and verification expectations.

### Updating an existing PRD

Read the existing PRD and its source documents before editing. Preserve valid
decisions already captured in the PRD, remove drift, and update sections whose
source evidence has changed. Do not duplicate unchanged material just to make
the document longer.

## Authoring workflow

Use the workflow in
[references/prd-authoring-workflow.md](references/prd-authoring-workflow.md).

For template shape, start from
[assets/resumeenhancer-prd-template.md](assets/resumeenhancer-prd-template.md)
and adapt it to the available evidence.

## Local writing guidance and templates

Use only resources kept in this skill folder. Read the smallest relevant
resource for the PRD shape:

- [references/prd-writing-guide.md](references/prd-writing-guide.md) for
  section intent and writing quality
- [assets/lean-prd-template.md](assets/lean-prd-template.md) for small,
  narrow changes
- [assets/resumeenhancer-prd-template.md](assets/resumeenhancer-prd-template.md)
  or [assets/comprehensive-prd-template.md](assets/comprehensive-prd-template.md)
  for standard feature work
- [assets/google-prd-template.md](assets/google-prd-template.md) for
  metric-heavy, cross-team work
- [assets/amazon-pr-faq-template.md](assets/amazon-pr-faq-template.md) only
  when the user explicitly wants a working-backwards or PR/FAQ style

Do not copy template examples, fake evidence, placeholder metrics, or sample
quotes into a real PRD.

## PRD quality rules

- Start with the product problem and business/user impact, not implementation.
- Keep the PRD grounded in the exact source documents; do not invent market
  evidence, analytics, or stakeholder approvals.
- Make every requirement specific enough to review and implement.
- Separate `In Scope`, `Out of Scope`, `Dependencies`, `Risks`, and `Open
  Questions`.
- Keep technical content at implication level unless the source material already
  defines a technical constraint that product stakeholders need to know.
- When information is missing, name it as an assumption, gap, or open question
  instead of filling it from generic PM advice.

## Traceability expectations

- Link the PRD back to the source BR or story pack with repository-relative
  paths.
- If the PRD is story-driven, list the `.US.md`, `.SI.md`, and `.Research.md`
  inputs actually used.
- If the PRD is BR-driven and the BR names related story packs, include only the
  story packs that materially shaped the PRD.

## Update behavior

When updating an existing PRD:

- keep stable identifiers, title, and source links unless the source changed
- refresh `Last Updated`
- revise outdated scope, requirements, risks, metrics, and open questions
- remove contradictions between the PRD and the latest source documents
- preserve useful prior rationale that still matches the evidence

## Final check

Before finishing, complete
[references/prd-review-checklist.md](references/prd-review-checklist.md). Treat
each applicable unchecked item as a defect to fix, an explicit assumption, or
an open question. Do not claim stakeholder review, approval, baseline data, or
launch readiness that the source evidence does not establish.
