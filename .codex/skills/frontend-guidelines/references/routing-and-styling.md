# Routing And Styling

## Routing

- Define browser routes in `src/app/router.tsx`; keep the route table declarative and let page/feature components own the screen behavior.
- Use `ProtectedRoute` and `Outlet` for authenticated children instead of repeating auth redirects in pages.
- Add `routeContexts` metadata when a route needs shell title, shell classification, or unsaved-work handling.
- Use `Navigate` for deterministic redirects and `useNavigate` for user-triggered transitions. Preserve the intended return path only where the existing auth flow supports it.
- Lazy-load an entire route or heavy optional editor only when it reduces meaningful initial work. Keep the loading and error boundary explicit.

## Styling

- Compose Tailwind utility classes with `cn` and reuse `shared/ui` primitives before building a feature-local substitute.
- Use semantic CSS variables such as `--color-accent` and established utility variants. Do not hard-code a color that bypasses the supported light, midnight, and forest themes.
- Prefer a clear component hierarchy over a long opaque class string. Extract a local component when it has a distinct semantic or interaction responsibility, not solely because a line count was crossed.
- Preserve focus-visible styles, disabled state, responsive breakpoints, and dark-theme behavior when customizing a shared control.
- Use Heroicons consistently for interface icons. Pair icon-only controls with an accessible name.
