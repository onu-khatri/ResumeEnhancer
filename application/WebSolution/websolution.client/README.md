# ResumeEnhancer Client

The client is a React, TypeScript, Vite application organized by feature.

## Frontend Foundation

New work must reuse the shared frontend foundation before creating feature-local controls or styles:

- Use `src/shared/ui` for token-driven, accessible presentation primitives.
- Use `src/shared/lib` only for generic cross-feature helpers.
- Keep module API clients, response mapping, authorization, entitlements, and workflow state inside the owning feature.
- Use route contexts for public, authenticated, and transient task surfaces.
- Use semantic design tokens from `src/index.css`; document exceptions before adding new shared values.
- Add templates to the resume template registry. Renderers change presentation only and share the same resume information model.

See `src/shared/README.md` for the dependency boundary.

## Quality Gates

Run these commands from this directory:

```powershell
npm run check
npm run test
npm run test:coverage
npm run build
```

`test:coverage` measures executable `src` code and requires at least 92% lines, branches, functions, and statements. Generated coverage output is local-only and ignored by Git.
