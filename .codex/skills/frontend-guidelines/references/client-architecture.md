# Client Architecture

Choose the smallest owner that keeps a user-facing behavior coherent.

| Location | Owns |
| --- | --- |
| `src/features/<feature>/` | Domain UI, feature API service, hooks, models, local state, and feature tests. |
| `src/shared/` | Cross-feature API client, UI primitives, hooks, state, utilities, and styles. |
| `src/app/` | Application providers, router construction, theme, and route metadata. |
| `src/routes/` | Route guards and route-level behavior. |
| `src/pages/` | Page composition that does not clearly belong to one feature. |

Keep a feature flat until a meaningful responsibility earns a subdirectory such as `api`, `components`, `hooks`, `model`, `state`, `layout`, or `pages`. Do not create a global component or store as a speculative reuse point.

Components render and coordinate a narrow interaction. Hooks encapsulate reusable stateful behavior. Controller hooks can compose form, query, mutation, navigation, and draft concerns when that keeps a page component readable; do not make them a second service layer. Keep transport mapping in the feature service or model boundary, not in JSX.

Use kebab-case filenames, named exports, and the `@/` alias for cross-boundary imports. Keep relative imports local to a cohesive feature area. Add a barrel only when it is a deliberate public feature surface, not to conceal ordinary ownership.
