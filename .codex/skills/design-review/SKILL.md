---
name: design-review
description: Run a sharp, prioritized design critique of the current design, covering hierarchy, typography, spacing, contrast, motion, states, responsiveness, accessibility, and brand consistency. Returns findings ranked from blocking to polish with concrete fixes, and can apply clear safe fixes directly. Use when asked to review, critique, polish, or pre-ship check a design, screen, or component.
---

# Design Review

Use this skill to deliver a senior-level craft critique, not a generic checklist dump.

## Better-interface script

For a rigorous, evidence-backed interface review, use the
[better-interface script](references/better-interface.md). It defines review
modes, required evidence, the five-domain review order, reporting format, and
implementation guardrails. Use it when the user asks for a comprehensive,
pre-ship, or system-level review; use the concise workflow below for a focused
component critique.

## Use this skill when

- reviewing a UI, screen, component, or flow for design quality before shipping
- critiquing visual hierarchy, typography, spacing, color, motion, states, or responsiveness
- checking accessibility and interaction craft alongside visual polish
- applying clear, low-risk design fixes after the critique

## Do not use this skill when

- the request is mainly about product requirements, backend behavior, or architecture
- no actual design surface, screenshot, page, or component is available to inspect
- the user wants implementation only and does not want critique or polish guidance

## Evaluation rubric

Review the design against the dimensions below. Prefer measured evidence over vibes when possible, such as contrast ratios, tap-target sizes, line length, line-height, and animation durations.

### Visual hierarchy and layout

- Ensure there is one clear focal point and an obvious primary action.
- Check scan order, grouping, alignment, shared edges, and whitespace as structure.
- Flag near-alignments, crowded sections, or layout decisions that weaken clarity.

### Typography

- Check that the type scale has a limited number of distinct steps.
- Review line-height, measure, letter-spacing, font pairing, and weight consistency.
- Flag widows, weak headline wrapping, and misaligned numeric treatment where relevant.

### Spacing and rhythm

- Look for a consistent spacing scale instead of arbitrary gaps.
- Check section rhythm, internal padding symmetry, and density fit for the surface.

### Color and contrast

- Validate WCAG AA expectations: body text at least 4.5:1, large text and UI/icons at least 3:1.
- Ensure meaning is not conveyed by color alone.
- Check accent restraint, placeholder legibility, and disabled-state differentiation.

### Motion

- Prefer purposeful motion that guides attention or continuity.
- Check that common UI transitions stay fast, usually around 150 to 250 ms, with larger entrances around 300 to 500 ms.
- Ensure `prefers-reduced-motion` is respected and no animation blocks interaction or causes layout shift.

### Component states

- Verify hover, focus-visible, active, and disabled states for interactive elements.
- Check visible focus treatment, loading/empty/error states, and touch target sizing.
- Ensure buttons, links, and other controls visually match their role and priority.

### Accessibility

- Review semantic structure, heading order, landmarks, image alt handling, and form labeling.
- Check keyboard operability, logical focus order, visible focus, contrast, zoom resilience, and reduced-motion support.

### Responsiveness

- Check fluid sizing or sensible breakpoints, overflow risks, clipping, overlap, and horizontal scroll.
- Review common widths from narrow mobile through desktop and confirm images keep their aspect ratio.

### Content and copy

- Look for clear headlines, specific CTA labels, and useful helper or error copy.
- Flag misleading success/error/loading states and vague action labels.

### Consistency and brand

- Check token reuse, radius/shadow/icon consistency, and fit with the product's visual language.

## Reporting format

Lead with the few issues that matter most. Rank findings by severity:

- `Blocking`: inaccessible, broken, WCAG-failing, or unusable on target devices
- `Important`: materially harms hierarchy, readability, usability, or consistency
- `Polish`: refinements that sharpen craft without changing the core flow

For each finding, provide:

- `What`: the exact element, area, or layer with the issue
- `Why`: the craft or usability reason it matters
- `Fix`: a concrete change, preferably with explicit values or implementation direction

After findings, include:

- `Strengths`: two to four things the design already does well
- `Highest-leverage change`: the single improvement worth making first

For a better-interface review, the script's `HIGH`, `MEDIUM`, and `LOW`
severity scale and its required Scope and Coverage, Findings, Considered but
Rejected, Verification, and Verdict sections replace this abbreviated format.

## Applying fixes

When asked to apply fixes, make only the clear, safe changes directly:

- contrast improvements
- spacing corrections
- focus-visible and keyboard-state fixes
- reduced-motion support
- semantic structure fixes
- alt text improvements

Leave subjective visual restructuring, broader information-architecture changes, or brand-direction changes as recommendations unless the user confirms them. Re-check measured values after editing.

## Tone

Be direct, respectful, and specific. Every critique should explain the issue and pair it with a concrete fix.
