# Frontend Testing Guide

Use Vitest and Testing Library to prove user-visible behavior at the smallest useful boundary. Tests live next to the source they cover and use the `@` alias.

## Test Selection

| Change | Focused evidence |
| --- | --- |
| Pure mapping, formatting, or schema logic | Unit test inputs, outputs, edge cases, and invalid values. |
| Hook, query, mutation, or store | Test lifecycle behavior with an isolated `QueryClient` or rendered hook. |
| Form, dialog, shared UI, or page state | Render with Testing Library and assert accessible user behavior. |
| Route guard or navigation | Use `MemoryRouter` or the route-level test pattern. |
| API client or feature service | Mock `fetch` and verify request shape, success mapping, and failure behavior. |

## Rules

- Test behavior and contracts, not implementation details or Tailwind class strings.
- Use user-facing queries such as role, label, and text before test IDs.
- Give React Query tests a fresh `QueryClient`; do not share cache state between cases.
- Cover at least the changed happy path and meaningful failure or boundary path. For user-facing async work, include pending, error, and recovery behavior where applicable.
- Keep fixtures explicit and immutable when separate create/update assertions need different inputs.
- Run `npm run test` for a focused frontend change when feasible. `npm run test:coverage` is the coverage-gate check; do not imply it passed unless it ran.

## Baseline Setup

`src/test/setup.ts` provides Testing Library cleanup and browser API shims. Add only narrowly scoped setup when a behavior cannot be modeled in the test itself.
