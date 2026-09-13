---
name: frontend-guidelines
description: Build and review ResumeEnhancer React frontend code with current architecture, typed data flows, accessible UI states, and proportionate performance practices. Use for feature, component, form, route, or client-data changes.
---

# Frontend Development Guidelines

Use this skill to make frontend changes that are reliable, maintainable, and native to the existing React/Vite client. Preserve verified local patterns; do not import a framework doctrine from another application.

Read [frontend workflow routing](references/frontend-workflow-routing.md) when selecting a frontend workflow or coordinating with another frontend skill or agent. This skill provides standards and does not delegate implementation.

## Use this skill when

- implementing or reviewing frontend code in ResumeEnhancer
- deciding where files belong and how route, state, and data flows should work
- improving forms, UI states, accessibility, performance, or API integration quality

## Do not use this skill when

- the task is backend-only
- you only need visual direction without implementation standards; use `$frontend-design`
- you need a defect-first visual critique; use `$frontend-design-review`

## Knowledge and project discovery

1. Read `KnowledgeBase/INDEX.md`, then only linked project knowledge that materially affects the change.
2. Inspect the target route, feature, shared primitive, and nearest tests before choosing a pattern.
3. Read [project adaptation](references/project-adaptation.md) for verified client conventions.
4. Use the routed references below only for the decision at hand. They are guidance, not a substitute for local evidence.

## Workflow

1. Establish the user-visible behavior, owner, route, entitlement, and API contract; surface material gaps instead of inventing them.
2. Score the approach with [decision triage](references/decision-triage.md) when the work is non-trivial; simplify or split a weak design before coding.
3. Place domain behavior in its feature, reuse a shared primitive only when it is truly cross-feature, and keep routes as composition boundaries.
4. Keep network access in typed feature services built on the shared API client. Model query keys, invalidation, and pending/error/empty/success states deliberately.
5. Keep forms schema-led and controller-driven. Make validation, draft behavior, retry, and mutation outcomes clear to users.
6. Implement responsive, keyboard-accessible behavior and preserve the product's semantic theme tokens.
7. Add focused tests at the behavior boundary, then run the smallest relevant client checks.

## Reference routing

- [decision-triage.md](references/decision-triage.md): assess scope and choose the smallest sound implementation.
- [client-architecture.md](references/client-architecture.md): feature, shared, route, component, hook, and controller ownership.
- [data-and-state.md](references/data-and-state.md): React Query, services, forms, client state, and all user-visible async states.
- [routing-and-styling.md](references/routing-and-styling.md): React Router composition, route guards, Tailwind, shared UI, and theme tokens.
- [accessibility-and-responsive.md](references/accessibility-and-responsive.md): keyboard, semantic, responsive, and motion requirements.
- [typescript-and-performance.md](references/typescript-and-performance.md): strict typing, mapping, rendering, loading, and bundle guidance.
- [testing-guide.md](references/testing-guide.md): Vitest and Testing Library coverage.

## Core rules

- Do not add a dependency, global store, shared primitive, cache strategy, or architecture layer without a concrete local need.
- Do not use `any`, inline `fetch`, duplicated API mapping, or a component-local substitute for an existing feature hook/service.
- Do not treat Suspense, lazy loading, `useMemo`, `useCallback`, or `React.memo` as defaults. Apply them when measured or structural benefit justifies them.
- Do not hide failures behind indefinite spinners, generic alerts, or silent mutation errors.
- Do not reduce accessibility to visual appearance; keyboard flow, semantic controls, focus, labels, and contrast are part of the behavior.

## Output requirements

- implementation and file-placement rationale
- route, state, service, and data-flow notes when relevant
- loading, empty, error, success, and permission-state plan
- type, validation, responsive, and accessibility notes
- tests run and any checks not run, stated precisely

## Definition of Done

- Feature boundaries and typed API interaction are preserved.
- User-visible states, responsive behavior, and accessible interaction are intentionally covered.
- Focused tests are added or the reason they are unnecessary is explicit.
- Run `npm run check` and `npm run build` in `application/WebSolution/websolution.client` when the change scope permits; report exactly what ran.
