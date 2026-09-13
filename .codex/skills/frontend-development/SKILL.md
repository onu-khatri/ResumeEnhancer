---
name: frontend-development
description: Deliver production-ready ResumeEnhancer React features with correct routes, typed client data, accessible responsive behavior, and focused verification. Use when building or changing product UI, hooks, forms, or client integration.
---

# Frontend Developer

Use this skill to turn an approved frontend behavior into implementation-ready code that fits the existing React/Vite client. It owns delivery sequencing and evidence; `$frontend-guidelines` owns the detailed shared implementation standards.

For multi-skill work, follow [frontend workflow routing](../frontend-guidelines/references/frontend-workflow-routing.md). This is the default primary implementation lane; do not run another primary frontend implementer in parallel.

## Use this skill when

- building or modifying ResumeEnhancer UI, routes, hooks, forms, state, or API integration
- implementing a user story or accepted design in the established client architecture
- fixing an implementation defect whose scope is local to the client

## Do not use this skill when

- the work is purely backend or persistence design
- the task is design-only without implementation
- the task is a visual critique; use `$frontend-design-review`
- the task needs visual direction before code; use `$frontend-design`

## Discovery

1. Read `KnowledgeBase/INDEX.md`, then retrieve only applicable project knowledge.
2. Follow [frontend workflow routing](../frontend-guidelines/references/frontend-workflow-routing.md) to resolve any material evidence or user-decision gate before coding.
3. Inspect the target route, feature, API contract, shared primitive, and closest tests.
4. Read `$frontend-guidelines`, especially [project adaptation](../frontend-guidelines/references/project-adaptation.md), plus the narrowly relevant routed reference.
5. Read [implementation playbook](references/implementation-playbook.md) when the change spans a route, form, remote data, or non-trivial interaction flow.

## Workflow

1. Establish the expected behavior, caller, route, entitlement, and failure behavior. Do not invent a missing API, permission, or product decision.
2. Choose the smallest feature, route, component, hook, and service ownership that matches existing code.
3. Build typed API/service, query/mutation, form/schema, and local-state flow before composing presentation details.
4. Reuse shared UI and semantic theme tokens. Implement pending, empty, error, success, and permission states that are meaningful for the surface.
5. Make keyboard operation, focus behavior, labels, responsive layout, and reduced-motion behavior part of the implementation.
6. Add focused tests for changed behavior and run the smallest meaningful client checks.

## React And Client Boundaries

- Use the installed stack: React 19, React Router 7, TanStack Query, React Hook Form, Zod, Zustand, Tailwind, and the shared API client.
- Do not introduce Next.js, server components/actions, SSR/ISR, a new state library, Storybook, PWA infrastructure, analytics, or real-time transport unless the task and repository evidence require it.
- Use concurrent React APIs, memoization, lazy loading, virtualization, or optimistic updates only when the interaction or measured cost justifies them.
- Escalate auth, authorization, redirect, sensitive-data, or untrusted-content concerns to `$frontend-security`.

## ResumeEnhancer focus

- route composition through `src/app/router.tsx` and guards through `src/routes`
- feature-scoped services, hooks, models, state, and page behavior under `src/features`
- shared API behavior through `src/shared/api/api-client.ts` and reusable controls under `src/shared/ui`
- user-story traceability from visible behavior through client service calls

## Output requirements

- changed file placement and ownership rationale
- route, data, state, form, and interaction approach as applicable
- accessibility and responsive behavior addressed
- tests and checks run, plus any intentionally unrun validation

## Definition of Done

- Feature boundaries, typed contracts, and shared API behavior are preserved.
- User-visible states and accessible responsive behavior are covered.
- Focused tests cover changed behavior or their omission is explicit.
- Run `npm run check`, `npm run test`, and `npm run build` in `application/WebSolution/websolution.client` when proportionate to the change; report exactly what ran.
