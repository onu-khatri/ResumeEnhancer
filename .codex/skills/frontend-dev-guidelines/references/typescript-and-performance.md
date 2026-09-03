# TypeScript And Performance

## TypeScript

- Use the strict compiler settings already enforced by `tsconfig.app.json`; do not use `any` to cross a type boundary.
- Use `import type` for type-only imports and define request, response, form, and view types deliberately.
- Narrow `unknown` at runtime when data is untrusted. Convert nullable API fields to UI-safe form values in named mapping functions.
- Prefer discriminated state and small domain types over boolean combinations that permit impossible UI states.
- Add explicit annotations when inference hides a public contract or allows an unsafe boundary, not mechanically on every local expression.

## Performance

- Start with a correct, readable render path. Use `useMemo`, `useCallback`, `React.memo`, `useDeferredValue`, or `startTransition` only when a changing input, expensive derivation, or measured render boundary benefits.
- Debounce user-driven remote search and ensure a stale response cannot produce misleading UI.
- Avoid serial query waterfalls when independent resources can load in parallel.
- Virtualize only genuinely large lists and lazy-load only meaningful route or optional-feature bundles.
- Keep effects for synchronization with external systems. Clean up subscriptions, timers, object URLs, and browser listeners.
- Treat layout shift, blocked input, redundant requests, and unbounded rendering as user-visible defects. Confirm suspected regressions with browser or profiler evidence before adding complexity.
