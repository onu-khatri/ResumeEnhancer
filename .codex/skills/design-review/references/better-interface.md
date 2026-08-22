# Better Interface Script

Use this for a complete interface review. Treat the interface as one
system: layout establishes hierarchy, writing explains it, typography makes it
legible, and UI polish makes it feel deliberate. Consolidate the result into a
single prioritized verdict rather than producing disconnected domain audits.

Match the project's styling system, component library, tokens, density, and
motion language. Express fixes in the established system; do not introduce a
second styling approach for a local improvement. Numeric values below are
starting points when the product has no usable system of its own. Measure and
report contrast, but do not repaint the product unless asked.

## Scope And Mode

Resolve the screen, flow, feature, or repository scope from the request and
workspace. State the resolved scope. Use `full` if the user does not specify a
mode.

| Mode | Coverage | Finding cap |
| --- | --- | --- |
| `quick` | Primary path and highest-traffic states | 5 (`HIGH` and `MEDIUM` only) |
| `full` | Requested scope across every domain, including present empty, loading, error, and narrow-width states | 15 |

If the requested scope is too large to inspect credibly, narrow it to the
highest-traffic complete flow and state the boundary. Never imply that
uninspected surfaces were reviewed.

Before judging, identify the framework, styling approach, component library,
design tokens, supported viewports, and available preview or test commands.
For copy, inspect nearby text, product terminology, localization conventions,
and any established content style guide.

Review in this order so foundational failures cannot be hidden by polish:

1. Accessibility
2. Layout
3. Writing
4. Typography
5. UI polish

Assign an issue to its root-cause domain and report it once. Each finding must
cite `path/to/file:line` and show the current implementation. When runtime
behavior determines the outcome, inspect the rendered state; do not infer a
visual issue only from source or a code issue only from appearance.

Review requests are read-only unless the user also asks to implement findings.
When implementing, use the approved report as the scope and rerun relevant
verification afterward.

## Evaluation Rules

### Accessibility

- Prefer native controls: use `button` for actions and `a[href]` for
  navigation, never a clickable `div`.
- Keep a verified visible `:focus-visible` indicator. Do not remove outlines
  without a replacement; use a 2px perimeter or equivalent visible area.
- Ensure every pointer interaction has a keyboard path. Use normal DOM order,
  `tabindex="0"` or `-1` only, Escape for overlays, and appropriate composite
  widget keyboard patterns.
- For modals, move focus inside on open, make the background inert, contain
  overscroll, and restore focus to the trigger on close.
- Meet the WCAG 2.5.8 24 by 24 CSS-pixel target baseline. Aim for 44 by 44px
  on touch and 40 by 40px on desktop when density permits; expanded hit areas
  must not overlap.
- Give inputs real labels, meaningful `name`, `autocomplete`, and input type
  or `inputmode`. Place a checkbox and its label in one hit target. Never
  block paste.
- Give icon-only controls descriptive accessible names. Visible text belongs in
  the accessible name; decorative elements can be `aria-hidden` only when not
  focusable.
- Do not communicate status with color alone. Measure the rendered pair and
  report the failed WCAG requirement when contrast does not pass.
- Respect reduced motion. Prefer motion behind
  `prefers-reduced-motion: no-preference`; reduce to opacity changes and remove
  parallax or autoplay under the preference. Autoplaying media needs pause
  controls; actionable or error toasts remain until dismissed.

### Layout

- Group primarily with space: the inter-group gap should be at least twice the
  intra-group gap. Use separators only when spacing cannot communicate the
  relationship.
- Make controls visibly distinct from adjacent content and align related
  elements to shared edges.
- Put the most important content near the top and leading edge. Use logical
  properties for direction-dependent layout.
- Start with 12px between adjacent bordered or filled controls and 24px around
  borderless text or icon controls when no density scale exists.
- Choose breakpoints where content stops fitting, preserve the expanded layout
  until then, and prefer container queries for component-level adaptation.
- Stress-test growth: avoid fixed text dimensions, permit wrapping, and ensure
  critical actions do not clip at narrow widths or longer translations.

### Writing

- Preserve the product's voice and terminology. Be warm for onboarding and
  empty states, neutral for routine work, calm for errors, and explicit for
  destructive or security-sensitive actions.
- Prefer plain, translatable language. Use full pluralized templates instead
  of concatenating fragments around variables.
- Use verb-first labels that name the outcome, especially for destructive
  confirmation actions. Links must name their destination.
- Apply one capitalization policy per element type; sentence case is a safe
  default.
- Put corrective error copy beside the failing field. State how to recover;
  avoid blame, "oops", and exclamation marks.
- Empty states explain the destination and offer one clear next action. Search
  or filter empty states should include an exit such as clearing filters.

### Typography

- Use a restrained type system. Avoid more than three font families and use a
  small semantic scale rather than scattered one-off sizes.
- Keep visual heading levels descending. Use semantic headings independently
  of visual styling.
- Start near `1.1` line-height for short headings and `1.5` to `1.6` for body
  text. Use unitless values; wrapped short text needs at least `1.4`.
- Cap long-form measure around 60 to 75 characters. Balance headings, use
  pretty wrapping for descriptions, and avoid both for long-form body text.
- Use tabular figures for changing numeric values. Truncated meaningful content
  needs a route to the full value.
- Keep mobile input type at least 16px to prevent iOS Safari zooming. Do not
  disable zoom with viewport restrictions.

### UI Polish And Motion

- Use concentric nested radii: outer radius equals inner radius plus padding.
  Correct visually imperfect alignment optically, especially for asymmetric
  icons.
- Use layered transparent shadows for elevation and retain borders for actual
  structure, selection, or focus.
- Prefer CSS transitions for interruptible interactions; reserve keyframes for
  infrequent one-shot sequences. Never use `transition: all`; name exact
  properties.
- For infrequent entrances, stagger semantic groups by about 100ms. Keep exits
  subtler and shorter than entries, usually a small directional movement with
  opacity. Do not animate frequently repeated interactions just for decoration.
- For contextual icon swaps, animate opacity `0` to `1`, scale `0.25` to `1`,
  and blur `4px` to `0px`. With Motion, use a spring of duration `0.3` and
  `bounce: 0`; without it, cross-fade both DOM icons using CSS. Check the
  installed package and nearby imports before choosing an import path.
- Use `scale(0.96)` for eligible button press feedback and provide a way to
  disable it for static or distracting contexts. Use `initial={false}` only
  for state changes that should not animate on initial render.
- Use `will-change` only to resolve observed first-frame stutter and only for
  compositor-friendly properties such as `transform`, `opacity`, and `filter`.
- Use `currentColor` SVGs for stateful icons. Match icon stroke weight to
  adjacent text, use outline as the normal state and fill for active state,
  and test at the smallest render size. Flip only direction-dependent icons in
  RTL.
- Add a subtle 1px image outline when images need consistent depth: pure black
  with low opacity in light mode, pure white with low opacity in dark mode.

## Required Output

Use these sections in order.

### Scope And Coverage

State mode, exact scope, stack and styling conventions, and any boundary.
Include every domain in this table. `Clear` means inspected with no actionable
finding; `Not reviewed` explains why.

| Domain | Evidence inspected | Result |
| --- | --- | --- |
| Accessibility | Files, components, states, or checks | Findings count or `Clear` |
| Layout | Files, components, states, or checks | Findings count or `Clear` |
| Writing | Files, components, states, or checks | Findings count or `Clear` |
| Typography | Files, components, states, or checks | Findings count or `Clear` |
| UI polish | Files, components, states, or checks | Findings count or `Clear` |

### Findings

Use one table ordered by severity, then reach and leverage. Shared tokens or
components outrank the equivalent symptom in one leaf component. Each row is
one root cause; list all confirmed locations in the same row. Do not pad the
table to meet the cap. If there are no findings, state: `No actionable
interface findings.`

| # | Severity | Domain | Location | Before | After | Why |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | HIGH | Accessibility | `src/Dialog.tsx:42` | `<button><XIcon /></button>` | Add `aria-label="Close"` and hide the decorative icon from the accessibility tree | The icon-only control has no accessible name |

- `HIGH`: blocks a task, misleads users, hides content or controls, creates
  data-loss risk, or causes a repeated systemic failure.
- `MEDIUM`: materially harms comprehension, efficiency, adaptability, or
  consistency.
- `LOW`: isolated, limited-impact polish. Include only for `full` reviews.

### Considered But Rejected

Record real candidates rejected because the current convention is intentional,
evidence is insufficient, or the change adds complexity without user benefit.
Include one to three in `quick` mode and two to five in `full` mode when they
exist; do not invent filler.

| Location | Candidate | Rejected because |
| --- | --- | --- |
| `src/Card.tsx:28` | Increase the shadow | The existing depth matches the shared surface token |

### Verification

List safe, relevant commands, browser interactions, or visual checks and their
observed results. Mark missing checks as **Not verified**. A verification gap
is not itself a finding.

### Verdict

End with exactly one verdict:

- `Block`: one or more `HIGH` findings remain.
- `Needs changes`: only `MEDIUM` or `LOW` findings remain.
- `Approve`: no actionable findings remain and the claimed coverage was
  verified.

## Applying Findings

When asked to implement the review, preserve the report as the change scope.
Use existing components, tokens, conventions, and installed dependencies. Make
safe fixes directly when the evidence supports them, including semantic native
controls, labels and accessible names, focus treatment, reduced motion,
spacing, text wrapping, and state styles. Keep subjective brand direction,
information architecture, and unverified visual restructuring as
recommendations until the user approves them. Re-check rendered behavior and
measurements after edits.
