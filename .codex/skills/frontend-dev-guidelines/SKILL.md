---
name: frontend-dev-guidelines
description: Apply ResumeEnhancer frontend development guidelines for React, TypeScript, Vite, feature boundaries, forms, data fetching, routing, and UI states. Use when Codex needs project-consistent frontend implementation guidance or review criteria.
---

# Frontend Development Guidelines

Use this skill to keep ResumeEnhancer frontend work aligned with the current client architecture while still benefiting from stronger upstream patterns.

## Use this skill when

- implementing or reviewing frontend code in ResumeEnhancer
- deciding where files belong and how data, routes, and state should flow
- improving UI states, forms, or API integration quality

## Do not use this skill when

- the task is backend-only
- you only need visual design direction without implementation standards

## Workflow

1. Start from the target route, feature, or component behavior.
2. Prefer existing feature boundaries and shared primitives over new ones.
3. Keep data access typed and centralized.
4. Design loading, empty, error, success, and permission states intentionally.
5. Use the local adaptation note before following an upstream resource literally.

## Read these references as needed

- `references/project-adaptation.md`
- `references/file-organization.md`
- `references/data-fetching.md`
- `references/loading-and-error-states.md`
- `references/component-patterns.md`
- `references/routing-guide.md`
- `references/styling-guide.md`
- `references/typescript-standards.md`
- `references/common-patterns.md`
- `references/complete-examples.md`

## Core rules

- preserve feature boundaries
- align forms with current schemas and hooks
- keep API interaction typed and centralized
- avoid importing stack assumptions that do not fit React Router plus Vite

## Output requirements

- recommended file placement
- route, state, and data flow notes
- loading and error state plan
- type alignment notes

## Definition of Done

- `npm run check` (typecheck + lint + format) passes in the client.
- `npm run build` passes.
- Feature boundaries are preserved and API interaction stays typed and centralized.