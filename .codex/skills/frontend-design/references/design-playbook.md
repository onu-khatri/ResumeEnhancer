# Frontend Design Playbook

Use this playbook when the interface needs a stronger design point of view. It expands the skill entrypoint without replacing repository or implementation authority.

## Intent

Design like a frontend designer-engineer, not a layout generator. The outcome must be distinctive enough to feel authored, but disciplined enough to ship in the current ResumeEnhancer client.

Every proposed direction should satisfy all of these:

- an explicit aesthetic stance
- a clear product or workflow purpose
- at least one memorable differentiation anchor
- cohesive restraint instead of random flourish
- feasible implementation with accessible, performant code

## Design sequence

1. clarify the audience, task outcome, and business goal
2. choose one dominant aesthetic direction and optionally one supporting tone
3. define the differentiation anchor:
   "If the logo were removed, what would still make this screen recognizable?"
4. score the direction for fit, distinctiveness, implementation feasibility, performance safety, and maintainability
5. define hierarchy and task flow before decoration
6. design non-default states intentionally
7. make mobile behavior and accessibility first-class constraints
8. translate the design into buildable guidance or code-ready decisions

## Direction scoring

Use a quick feasibility and impact check before committing to a direction.

Score each dimension from 1 to 5:

- aesthetic impact: how distinctive and memorable the direction is
- context fit: how well it matches the product, audience, and task
- implementation feasibility: how cleanly it can be built with the current stack
- performance safety: whether it stays fast and accessible
- consistency risk: how hard it will be to sustain across related surfaces

Use this formula:

`score = impact + fit + feasibility + performance - consistency risk`

Interpretation:

- `12-15`: execute fully
- `8-11`: strong; proceed with discipline
- `4-7`: reduce scope or visual effects
- `3 or below`: rethink the direction

Default to directions scoring `8` or higher.

## Aesthetic execution rules

### Typography

- avoid default-feeling stacks such as Inter, Roboto, Arial, or plain system fonts unless the existing product surface already depends on them
- prefer one expressive display voice and one restrained body voice when new type choices are appropriate
- use scale, rhythm, weight, and contrast structurally, not decoratively

### Color

- commit to a dominant color story instead of evenly weighted palettes
- define colors through CSS variables or design-token-friendly semantics
- prefer one dominant tone, one accent, and a neutral support system
- avoid purple-on-white SaaS defaults unless the existing product language explicitly requires it

### Layout and spacing

- avoid default symmetric section stacks
- use asymmetry, overlap, negative space, or disciplined density intentionally
- let spacing reinforce hierarchy and reading rhythm

### Motion

- motion must communicate, orient, or reward; never exist only to look busy
- prefer one strong entrance rhythm and a few meaningful interaction states
- respect reduced-motion expectations and keep implementation cost proportionate

### Texture and depth

- use gradients, translucency, dividers, grain, or shadow only when they support the design thesis
- remove any effect that does not improve hierarchy, tone, or recognition

## ResumeEnhancer-specific guidance

- favor trust, clarity, and professional credibility over novelty for its own sake
- let resume, profile, job-search, and career-workflow cues inform the visual language
- align with existing route structure, shared UI primitives, and frontend implementation patterns
- when working inside an established surface, preserve the product's existing language and improve it rather than introducing a disconnected sub-brand

## Required outputs

For substantial design work, provide:

- design direction summary with the named aesthetic and why it fits
- the differentiation anchor
- the quick direction score when you considered multiple directions or the direction is bold
- hierarchy and task-flow notes
- state inventory: default, loading, empty, error, hover, focus, disabled, and mobile behavior when relevant
- design-system signals: type, color, spacing, motion, and standout surface treatments
- implementation handoff notes and constraints

## Anti-patterns

- interchangeable card grids with no visual idea
- generic template dashboards
- motion with no communication purpose
- inaccessible contrast or fragile spacing systems
- decorative layers that weaken clarity
- design-by-component where the visual system never becomes a product point of view
- ambitious visual concepts that the current stack cannot support cleanly

If the result could plausibly be mistaken for a stock template, restart from the direction step.
