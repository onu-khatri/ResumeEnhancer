# Data And State

Remote data belongs to React Query; user interaction and transient view state belong close to the consuming component; persistent cross-surface client state belongs in Zustand only when query cache is not the owner.

## API Boundary

- Create typed feature services that receive `createApiClient` from `shared/api/api-client.ts`.
- Keep paths, request payload mapping, response mapping, and API response types out of page JSX.
- Preserve the shared client's authentication refresh, credentials, error conversion, and JSON handling. Do not call `fetch` directly from a feature.
- Query keys must identify the resource and its meaningful parameters. Invalidate the smallest affected key family after a mutation.

## Queries And Mutations

- Use `useQuery` or `useMutation` through feature hooks. Existing code uses explicit `isPending`, `isError`, and retry UI; do not force Suspense into a workflow that needs partial rendering or recoverable inline states.
- Keep query functions deterministic and enabled only when their required identity is available.
- For mutations, define pending disabled state, success transition, error recovery, cache invalidation, and draft cleanup or rollback where relevant.
- Use optimistic updates only when the prior state, rollback, and reconciliation behavior are explicit.

## Forms

- Define form values and Zod schemas in the feature model. Use `zodResolver` and React Hook Form rather than duplicating validation in individual fields.
- Use `useFieldArray` for repeating sections, stable client keys for rows, and map API data at the service/model boundary.
- Keep draft hydration and submit flow in the feature controller. Never overwrite an edited form without an intentional precedence rule.

## State Matrix

Every remote surface chooses pending, error, empty, success, and permission behavior. Reuse `shared/ui/status.tsx` when its semantics fit. Show an actionable retry for recoverable load failures, local inline feedback for mutation failure, and a pending state that prevents duplicate destructive or submit actions.
