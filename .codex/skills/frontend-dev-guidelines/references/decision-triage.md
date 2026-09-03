# Frontend Decision Triage

Use this before a new page, cross-feature flow, new dependency, store, cache strategy, or shared primitive. Small local edits do not need ceremony.

## Feasibility Check

Score each dimension from 1 (poor) to 5 (strong):

| Dimension | Question |
| --- | --- |
| Architectural fit | Does it extend an existing feature, shared primitive, or route pattern? |
| User-value clarity | Is the required behavior and outcome concrete? |
| Complexity | Are state, data, validation, and interaction responsibilities contained? |
| Performance impact | Does it avoid avoidable rendering, network, bundle, or layout-shift cost? |
| Maintenance cost | Can a future maintainer locate and safely change it? |

`FFCI = architectural fit + user-value clarity + performance impact - complexity - maintenance cost`

| Score | Action |
| --- | --- |
| 6 to 10 | Proceed with the chosen structure. |
| 2 to 5 | Proceed only after splitting responsibilities or documenting the tradeoff. |
| 1 or below | Redesign or ask for a material missing decision. |

The score is a prompt for design judgment, not a gate or an excuse to manufacture reusable abstractions.

## Decision Rules

- Prefer an existing feature extension before creating a new feature folder.
- Prefer local state for local interaction. Introduce Zustand only for client state shared across independent components or routes that is not server cache.
- Let React Query own remote data. Do not mirror query data into a store without a synchronization reason.
- Make a component shared only when it is generic and used by multiple owners; otherwise keep it inside the feature.
- Lazy-load a route or substantial optional surface when it provides a credible bundle or interaction benefit. Do not split tiny components just to satisfy a rule.
- Treat a new package as a design decision: inspect existing dependencies, state the gap, and choose the smallest maintained addition.
