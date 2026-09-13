# Accessibility And Responsive Behavior

Accessibility and responsive behavior are acceptance behavior, not final polish. Follow existing shared controls and semantic tokens before adding custom interaction.

## Required Checks

- Use semantic elements first: buttons for actions, links for navigation, labels for fields, and headings in a coherent order.
- Every interactive control must be reachable by keyboard and have a visible focus state. Do not replace native controls without preserving their keyboard and ARIA behavior.
- Pair inputs with visible labels or an equivalent accessible name; connect errors and help text to the relevant field.
- Preserve focus during dialogs, destructive confirmations, route changes, and dynamically added form sections. Return focus to the triggering control when a dialog closes where practical.
- Communicate loading, errors, and success in text, not color alone. Keep error messages actionable and place them near the affected work.
- Test at narrow and wide layouts. Ensure controls do not depend on hover, targets remain usable, text wraps without clipping, and actions remain discoverable.
- Use semantic theme variables and existing `dark:` behavior. Do not assume `body.dark` models every theme; `forest` is also supported.
- Respect `prefers-reduced-motion` for non-essential animation and avoid motion that blocks input or obscures state.

## Resume Workflows

- Builder validation must identify the field and section that needs attention.
- Repeating sections need stable labels, logical tab order, and explicit add/remove actions.
- Destructive resume actions require a clear confirmation, pending state, error recovery, and no accidental navigation.
- Preview and dashboard layouts must preserve core read, edit, create, and retry actions at narrow widths.
