---
name: production-ui-generator
description: Generate production-grade UI for ResumeEnhancer with intentional design, accessibility, component reuse, theming, and implementation-ready React patterns. Use when Codex needs to create or improve pages, flows, feature UIs, or polished product surfaces in the React/Vite client.
---

# Production UI Generator

Use this skill when the UI should feel launch-ready, deliberate, and implementable in the current client stack.

## Use this skill when

- building or upgrading a major product surface
- a user flow needs stronger hierarchy, polish, and state handling
- the UI must balance aesthetics with reusable implementation patterns

## Do not use this skill when

- the task is only backend behavior
- a tiny styling tweak does not need a broader UI system pass

## Workflow

1. Read the story and inspect the current page or component context.
2. Define user goals and critical states before drawing the default layout.
3. Reuse shared UI, hooks, and typed models wherever possible.
4. Choose a clear theme direction and interaction language.
5. Hand off implementation notes that fit React, TypeScript, and the current shared primitives.

## UI lenses

- hierarchy, spacing, and rhythm
- themed surfaces and strong contrast
- meaningful motion rather than decoration
- mobile and desktop behavior
- empty, loading, error, success, and entitlement states

## ResumeEnhancer focus

- professional and trustworthy product tone
- resume-centric workflows with clear progress feedback
- compatibility with the existing client architecture, not a separate design system fantasy

## Output requirements

- visual direction
- state plan
- reusable component notes
- implementation constraints and follow-up steps

## Definition of Done

- `npm run check` (typecheck + lint + format) and `npm run build` pass in the client.
- Empty, loading, error, success, and entitlement states are designed.
- The UI reuses existing shared primitives and stays compatible with the current client architecture.