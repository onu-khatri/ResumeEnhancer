# ResumeEnhancer Frontend Adaptation

Use this reference to distinguish verified local conventions from generic React advice. Current source is authoritative when this guide and the client differ.

## Verified Baseline

- The client is React 19, TypeScript, Vite, React Router 7, TanStack Query 5, React Hook Form, Zod, Zustand, and Tailwind CSS 4.
- The route table is `src/app/router.tsx`; guarded application routes use `src/routes/protected-route.tsx` and route metadata uses `src/app/route-context.ts`.
- `src/app/providers.tsx` composes theme, query, and auth providers. Do not add provider nesting without a real application-wide concern.
- `src/shared/api/api-client.ts` centralizes fetch behavior, authentication, refresh, `ApiError`, and JSON handling. Feature services accept that client instead of calling `fetch` directly.
- Feature code belongs under `src/features`; `src/shared` owns cross-feature UI, API, hooks, state, and utilities. `src/pages` contains page-level composition where a feature does not own the surface.
- Forms use schemas in `features/<feature>/model`, React Hook Form controllers/hooks, and Zod resolvers. Resume builder behavior is the closest current example.
- UI uses Tailwind utilities, semantic CSS variables, `shared/ui` primitives, Heroicons, and `cn`. There is no MUI or project-wide `~` alias.
- Tests use Vitest, Testing Library, and jsdom. The client scripts are in `package.json`.

## Local Discovery

Before changing a behavior, inspect its nearest route, feature, service, state hook/store, shared primitive, and test. Do not infer a convention from a stale reference, another application, or an unused dependency.
