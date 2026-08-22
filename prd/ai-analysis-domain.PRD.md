# PRD: AI And Analysis Domain

**Status:** Draft  
**Last Updated:** 2026-08-22  
**Source Documents:** `Business-Requirements/ai-analysis-domain.BR.md`

## Executive Summary

Job seekers need help both writing a stronger resume and assessing its fit for a
target role. This domain provides contextual AI assistance, ATS-style review,
job-specific matching, and an improvement loop while protecting user control,
honesty, privacy, and trust.

## Users And Stakeholders

- Job seekers who need writing help, quality feedback, or role-specific advice.
- Product, support, and operations teams that need explainable results and
  diagnosable entitlement and provider states.

## Scope

### In Scope

- Section-level AI writing and tailoring assistance.
- Resume-quality, ATS-style, and job-description-specific analysis.
- Actionable recommendations and rerun/improvement loops.
- Quota, premium-boundary, privacy, and safety communication.

### Out Of Scope

- Recruiter-side hiring products, autonomous applications, model-training
  platform design, and enterprise AI-governance tooling.

## Requirements

- **REQ-001: Contextual AI assistance.** Provide named help for summary,
  bullets, skills, objective, rewrite, and job tailoring rather than a generic
  AI surface.
- **REQ-002: User-controlled suggestions.** Do not overwrite user content;
  suggestions remain reviewable until accepted and users can keep the original.
- **REQ-003: Honest and safe output.** Do not invent qualifications, roles,
  metrics, or experience; handle moderation, quota, and provider failures
  gracefully.
- **REQ-004: Explainable resume analysis.** Provide categorized findings,
  sub-scores where used, and concrete fixes rather than a top-line score alone.
- **REQ-005: Distinct job matching.** Evaluate a resume against a specific job
  description through differentiated feedback such as requirements, skills,
  seniority, industry, and trajectory fit.
- **REQ-006: Actionable improvement loop.** Map recommendations to a resume
  section, wording pattern, or missing signal and direct users back to editing.
- **REQ-007: Traceable results.** Link suggestions and reports to the content
  context or version evaluated; reruns create fresh records and stale results
  are not presented as current.
- **REQ-008: Clear access states.** Explain whether an unavailable action is a
  plan boundary, quota exhaustion, content-validation issue, or service issue.
- **REQ-009: Standalone-tool privacy.** Clearly state retention, deletion, and
  protection of uploaded content for no-signup AI or analysis surfaces.

## Success Signals

- Users complete AI-assisted writing and return to improve drafts.
- Analysis identifies understandable strengths, gaps, and next actions.
- Support can distinguish quota, entitlement, provider, and content states.
- **Needs validation:** metrics, baselines, targets, and the free/premium depth
  boundary are not established by the source BR.

## Technical And Operational Implications

- Context/version-linked result records, typed suggestion contracts, quotas,
  entitlement checks, provider abstraction, cost controls, and generation audit
  data are required product implications.

## Risks And Mitigations

| Risk | Mitigation |
| --- | --- |
| Generic AI is ignored | Attach named actions to editing tasks. |
| Scores feel arbitrary | Show categorized findings and concrete fixes. |
| AI fabricates content | Reviewable suggestions and no-fabrication guardrails. |
| Quota confusion or variable cost | Visible credit state and usage/cost controls. |

## Open Questions

- Which AI actions and analysis depth belong in MVP and which are premium?
- Which analysis work is synchronous versus queued?
- How will no-fabrication validation be enforced?
- What quota model and score-explanation depth will be used?

## Traceability

- REQ-001 to REQ-009 <- `ai-analysis-domain.BR.md`, Sections 8-9.
- Scope, risks, and MVP boundary <- Sections 5, 12-14.
- Related delivery packs <- `User-Stories/11.1`, `11.2`, `12.1`, and `12.2`.
