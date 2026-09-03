---
name: frontend-design
description: Design polished, intentional interfaces for ResumeEnhancer that fit the product domain, existing implementation patterns, and accessibility expectations. Use when Codex needs to shape layout, hierarchy, copy, and interaction design before or during frontend implementation.
---

# Frontend Design

Use this skill to shape product-quality experiences with a clear point of view instead of interchangeable component grids. Keep the result distinctive, implementable, and aligned with the existing ResumeEnhancer client.

For a multi-skill task, follow [frontend workflow routing](../frontend-dev-guidelines/references/frontend-workflow-routing.md). Return an implementation handoff to the primary owner; do not implement, delegate, or reselect frontend skills from this design pass.

## Use this skill when

- the task is primarily about layout, hierarchy, motion, copy, or interaction quality
- a page or flow needs a deliberate visual direction before implementation
- you need design guidance that still respects the current React client

## Do not use this skill when

- the work is purely backend or data-layer oriented
- implementation mechanics matter more than design direction
- the user wants a defect-first critique of an existing UI more than new or revised design direction; use `design-review` instead

## Knowledge routing

1. Read `KnowledgeBase/INDEX.md`.
2. Read only the linked project knowledge that materially affects the surface you are designing.
3. Read `references/design-playbook.md` for the deeper design method and anti-generic guardrails.

## Design workflow

1. Clarify the user goal, primary audience, emotional tone, and business outcome.
2. Choose one explicit aesthetic direction and one differentiation anchor before styling details.
3. Work from information hierarchy and task flow before decoration.
4. Define non-default states, responsive behavior, and accessibility constraints intentionally.
5. Hand off decisions in a form that the frontend implementation skills can build without guesswork.

## Non-negotiables

- avoid generic "AI UI" patterns, default dashboard grids, and safe-but-forgettable styling
- choose a named visual direction instead of mixing unrelated aesthetics
- use typography, color, spacing, and motion to reinforce the design thesis
- preserve implementation realism, semantic structure, performance, and accessibility
- match the ambition of the visual direction with code complexity the current stack can support

## Design lenses

- hierarchy and readability
- strong visual identity
- purposeful motion
- empty, loading, and error states
- mobile behavior and accessibility
- implementation fit with existing shared UI and route structure

## ResumeEnhancer focus

- professional but distinctive product surfaces
- resume-centric workflows and credibility cues
- alignment with existing shared UI and route structure

## Output requirements

- design direction summary with the named aesthetic and differentiation anchor
- content hierarchy and task-flow decisions
- state and interaction notes, including empty, loading, error, hover, focus, and mobile behavior where relevant
- design-system signals such as type, color, spacing, and motion guidance when they matter
- implementation constraints or handoff notes that keep the design buildable in the current frontend stack
