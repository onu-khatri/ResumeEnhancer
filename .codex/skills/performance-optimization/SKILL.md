---
name: performance-optimization
description: Identify and fix performance issues in ResumeEnhancer across the React/Vite client, EF Core persistence, and the ASP.NET Core API. Use when diagnosing slow queries, render bottlenecks, or capacity concerns.
---

# Performance Optimization

Use this skill to make ResumeEnhancer fast without sacrificing clarity or correctness. Apply it in proportion to the actual bottleneck.

## Use this skill when

- a query, page, or endpoint is observably slow
- reviewing changes for N+1 queries, over-fetching, or heavy re-renders
- preparing capacity or indexing decisions

## Do not use this skill when

- there is no measured or visible performance problem
- the change is unrelated to runtime behavior

## Backend (EF Core + SQL Server)

- Profile first: use `AsNoTracking()` for read paths and `AsSplitQuery()` for wide graphs.
- Avoid N+1: load related data with `Include`/`ThenInclude` or explicit batching instead of per-row queries.
- Project to DTOs (`Select`) instead of materializing full entities when only some columns are needed.
- Cap and validate paging (`MaxPageSize`) and apply deterministic ordering with an `Id` tiebreaker.
- Add indexes for common filters (`UserId`, search columns); review query plans for hot paths.
- Reuse the audit pipeline and concurrency retry in `AppDbContext` rather than adding ad-hoc save logic.

## Frontend (React + Vite + TanStack Query)

- Use TanStack Query for server state with proper `queryKey` and caching; avoid refetching on every render.
- Memoize expensive computations (`useMemo`) and stable callbacks (`useCallback`) only when measured to help.
- Keep component state local to where it is used (Zustand for shared client state only).
- Code-split routes and defer heavy third-party bundles.
- Avoid passing object/array literals as effect dependencies that change identity every render.

## API boundary

- Return only the fields the UI needs; keep AM response contracts lean.
- Use cancellation tokens end to end so abandoned requests do not keep querying.

## Verification

- Backend: run integration tests and observe query behavior: `dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore`.
- Frontend: `npm run build` and profile with the browser devtools performance panel.

## Definition of Done

- The bottleneck is identified and fixed at its source.
- Query shape, paging, and indexing decisions are documented.
- Build and tests pass after the change.

