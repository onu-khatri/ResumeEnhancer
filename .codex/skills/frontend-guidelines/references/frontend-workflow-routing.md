# Frontend Workflow Routing

Use this reference to choose the smallest effective frontend workflow. It defines coordination only; implementation standards remain in the routed frontend-guidelines references.

## Default Order

1. Start from the requirement, route, current behavior, applicable contract, and local repository evidence.
2. Use `$research-deep` only when significant ambiguity or cross-cutting impact remains after local discovery. Its decision brief becomes input to the next owner.
3. Use `$workflow-user-interview` when a material product, audience, scope, priority, constraint, or approval decision remains unresolved after available evidence. It returns confirmed decisions to the calling workflow.
4. Use `$frontend-design` only when visual direction, hierarchy, copy, or interaction intent is missing or deliberately changing. Its output is a handoff, not code delivery.
5. Use `$frontend-development` as the default primary implementation owner. It reads `$frontend-guidelines` for standards and applies them directly.
6. Use exactly one specialist only when its trigger applies, then return its constraints or findings to the primary owner.
7. Use `$frontend-design-review` for an explicit critique or pre-ship visual review after an inspectable surface exists. Use `$frontend-presentations` only after implementation when the deliverable is a presentation.

## Specialist Triggers

| Need | Select | Do not select when |
| --- | --- | --- |
| Significant ambiguity across requirements, source, tests, users, or current external standards | `$research-deep` before design or implementation | A focused local inspection can answer the decision confidently. |
| Material goal, scope, audience, constraint, tradeoff, or approval decision lacks an evidence-backed answer | `$workflow-user-interview` before the blocked decision | The answer is already available from the request, repository, or authoritative source. |
| Shared client architecture, data, forms, routes, state, accessibility, or tests | `$frontend-guidelines` with the primary owner | The task is visual direction only. |
| A React or TypeScript pattern decision that remains unclear after local inspection | `$frontend-react-patterns` | It would duplicate the guidelines or existing source pattern. |
| UI-dominant page or flow generation with an explicit implementation request | `$frontend-production-ui` instead of `$frontend-development` | Another primary implementation lane is already assigned. |
| Auth, redirects, untrusted content, sensitive data, or browser trust boundary | `$frontend-security` | No client trust boundary changes. |
| Measured or visible rendering, bundle, network, or responsiveness regression | `$quality-performance` | No concrete performance problem exists. |
| Design-quality findings on an existing surface | `$frontend-design-review` | A new design direction is needed before implementation. |
| Stakeholder walkthrough or presentation | `$frontend-presentations` | Production code is the deliverable. |

## Loop Guards

- A workstream has one primary owner: `$frontend-development`, `$frontend-production-ui`, or `frontend-implementer`; never more than one concurrently.
- `$frontend-guidelines`, `$frontend-react-patterns`, and the specialists are consulted references or bounded review passes, not separate delivery coordinators.
- A specialist does not invoke another frontend skill or agent. It returns a concrete handoff, finding, or constraint to the primary owner.
- `$research-deep` may use `$workflow-user-interview` only to frame an otherwise unanswerable decision; `$workflow-user-interview` never starts research or implementation. After either completes, control returns to the calling workflow.
- The primary owner may request at most the specialists whose triggers are present. It does not re-enter design after an approved handoff unless the requirement materially changes.
- `frontend-implementer` does not delegate or create subagents. A parent coordinator owns cross-layer contracts, synthesis, and any parallelization.
- `frontend-implementer` does not interview the user or open a broad research workstream. It reports the exact unresolved decision and evidence gap to its parent, which selects the gate.
- For a frontend/backend shared contract, use `$delivery-full-stack-feature` or `story-orchestrator`; do not split the contract across frontend specialists.
