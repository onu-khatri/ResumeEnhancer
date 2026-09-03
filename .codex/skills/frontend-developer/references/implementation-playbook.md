# Frontend Implementation Playbook

Use this note for a route, form, remote-data, or multi-state feature. It complements `$frontend-dev-guidelines`; current source and requirements remain authoritative.

## Delivery sequence

1. Confirm the user outcome, route entry, entitlement, API contract, and acceptance behavior.
2. Inspect the nearest feature, shared UI, service, hook/store, and test. Extend the current owner before creating a new abstraction.
3. Define request/response, form, and view models; keep mapping at the feature service or model boundary.
4. Implement service, query/mutation, controller or local state, then page composition. Keep remote cache in React Query and local interaction state close to its consumer.
5. Cover initial pending, empty, error with recovery, successful mutation, disabled submission, and permission behavior that apply to the flow.
6. Validate keyboard, labels, focus, semantic controls, narrow layouts, theme behavior, and reduced motion.
7. Add focused tests for the changed behavioral seam, then run proportional client checks.

## Common checks

- Shared UI is reused only when it fits the behavior; feature-specific UI stays local.
- API calls use `shared/api/api-client.ts`; no component makes an inline `fetch` call.
- Form values, Zod schemas, API payloads, and response mapping remain intentionally aligned.
- Query keys and mutation invalidation have a defined scope.
- Destructive or submit actions cannot be triggered twice while pending.
- Tests assert user-visible behavior with Vitest and Testing Library rather than implementation details.
