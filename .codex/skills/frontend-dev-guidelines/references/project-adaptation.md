# ResumeEnhancer Frontend Adaptation

The upstream `frontend-dev-guidelines` skill assumes a different frontend stack. Use these project-specific adaptations:

- ResumeEnhancer uses React + Vite + TypeScript, not Next.js.
- Routing is currently based on React Router via `src/app/router.tsx`, not TanStack Router.
- Reuse the current `shared/api/api-client.ts` and feature service files.
- Align forms with `src/features/resume/model/schema.ts`, `form-values.ts`, and current feature hooks.
- Reuse existing `shared/ui` and feature-level sections before introducing new primitives.
- Treat the upstream resource files as idea sources, not stack-exact rules.