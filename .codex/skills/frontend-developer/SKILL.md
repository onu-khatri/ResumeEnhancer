---
name: frontend-developer
description: Implement frontend features in ResumeEnhancer using the existing React, TypeScript, Vite, route, hook, and API-client patterns. Use when Codex needs to build or modify product UI in a way that fits the current client architecture.
---

# Frontend Developer

Use this skill for product-facing implementation that should feel native to the current client codebase.

## Use this skill when

- building or modifying ResumeEnhancer UI, routes, hooks, forms, or API integration
- turning user stories into frontend code within the established client architecture

## Do not use this skill when

- the work is purely backend or persistence design
- the task is design-only without implementation

## Workflow

1. Identify the page, route, feature boundary, and API dependencies.
2. Reuse existing shared UI, hooks, and typed models before creating new primitives.
3. Implement complete states: loading, empty, error, success, and permission.
4. Keep form state, schema, and API models aligned.
5. Verify responsive behavior and accessibility before finishing.

## ResumeEnhancer focus

- feature-scoped code under the existing frontend structure
- typed interaction with `shared/api`
- route-aware page composition
- user-story traceability from UI behavior to service calls

## Output requirements

- file placement
- state and route approach
- API integration notes
- verification checklist

## Definition of Done

- `npm run check` (typecheck + lint + format) passes in the client.
- `npm run build` passes.
- Loading, empty, error, success, and permission states are implemented.
- Forms stay aligned with the feature schema and backend API models.