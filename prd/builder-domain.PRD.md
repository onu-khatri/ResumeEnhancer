# PRD: Resume Builder Domain

**Status:** Draft  
**Last Updated:** 2026-08-22  
**Source Documents:** `Business-Requirements/builder-domain.BR.md`

## Executive Summary

The builder is the primary workspace where a job seeker turns intent into a
trustworthy, reusable resume. It must support fast draft creation, structured
authoring, ATS-safe presentation, reliable saves and previews, and adaptation
for multiple roles without losing content.

## Users And Stakeholders

- New users, existing-resume owners, and returning job seekers tailoring work.
- Product, design, engineering, QA, and support teams.

## Scope

### In Scope

- Template/import/default draft creation, structured sections, guided authoring,
  section management, appearance, validation, save, preview, duplicate/tailor,
  and document organization.

### Out Of Scope

- AI-provider orchestration, ATS scoring rules, billing logic, cover-letter
  domain behavior, and template-administration publishing.

## Requirements

- **REQ-001: Draft creation.** Create a usable draft from a selected template,
  approved import result, or default starter flow.
- **REQ-002: Structured content.** Store and edit typed sections/items instead
  of one opaque text block.
- **REQ-003: Guided authoring.** Provide section-oriented progression, helpful
  empty states, and example guidance.
- **REQ-004: Section management.** Add, edit, remove, reorder, hide/show, and
  preserve content for standard and specialized resume sections.
- **REQ-005: Safe template switching.** Preserve content when templates change
  and clearly communicate permitted reflow or pagination changes.
- **REQ-006: Tokenized appearance.** Change fonts, colors, accents, and spacing
  separately from content while enforcing or warning on ATS-safety constraints.
- **REQ-007: Meaningful validation.** Distinguish export/analysis blockers from
  non-blocking quality warnings.
- **REQ-008: Save confidence.** Communicate saving, saved, failed, and conflict
  states.
- **REQ-009: Trusted preview.** Keep preview sufficiently faithful to document
  layout and apply the same entitlement rules as export.
- **REQ-010: Duplicate and tailor.** Allow branching from an existing draft for
  role-specific adaptation and future reusable-content support.
- **REQ-011: Document organization.** Provide type-based views and labels/tags
  for role, language, or market organization.

## Success Signals

- Users reach a real draft quickly and understand resume structure.
- Content is preserved through template changes and tailoring.
- Users trust save state and preview/export parity.
- **Needs validation:** timing, adoption, and reliability thresholds.

## Technical And Operational Implications

- Structured partial updates, version/conflict handling, validation payloads,
  reliable retrieval, organization metadata, and preview/export parity are
  required implications.

## Risks And Mitigations

| Risk | Mitigation |
| --- | --- |
| Builder feels generic | Guided authoring and structured capability model. |
| Preview or save is not trusted | Explicit save states and preview/export parity. |
| Import fails user expectations | Review before finalizing imported content. |
| Design hurts ATS parsing | Enforce or warn on ATS-safe appearance constraints. |

## Open Questions

- What customization and optional sections belong in MVP?
- Is duplicate/tailor a primary action, and does MVP include a content library?
- What document and page limits apply?

## Traceability

- REQ-001 to REQ-011 <- `builder-domain.BR.md`, Sections 8-9.
- Delivery, QA, risks, and MVP boundaries <- Sections 10-13.
- Related delivery packs <- `User-Stories/5.1`, `5.2`, `7.1`, `7.2`, `8.1`,
  `8.2`, and `8.3`.
