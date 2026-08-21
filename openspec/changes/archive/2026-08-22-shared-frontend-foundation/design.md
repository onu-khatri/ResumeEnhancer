## Context

See [proposal.md](../../../openspec/changes/shared-frontend-foundation/proposal.md) for motivation. The attached user story defines a cross-cutting frontend foundation that future React stories must reuse instead of redefining. The repository already separates frontend code under `application/WebSolution/websolution.client/src/features`, and the foundation must fit that feature-sliced direction while also establishing shared shells, route metadata expectations, token usage, reusable UI-state behavior, themes, templates, common components, utilities, and test quality gates.

The current implementation is a React 19/Vite client using Tailwind CSS 4, React Router, Headless UI, React Hook Form, TanStack Query, and Zustand. Its `index.css` defines a single semantic light palette with a `body.dark` override; there is no theme-selection service. `app/router.tsx` directly maps the public login route and protected resume routes, while the resume feature provides its own `ResumeShell`. The shared UI contains `Button`, `InputField`, `TextareaField`, `SelectField`, `Card`, and status components, and `shared/lib` contains class-name and formatting helpers. Three resume-template options exist in the resume model, but no template-specific renderer currently consumes one common resume-information model. The client package has no test or coverage script and no test files were found, although the project metadata identifies Vitest as its JavaScript test framework.

There is no existing OpenSpec capability set in `openspec/specs`, so this change needs to introduce the contract as a new capability rather than modify an existing one. Because the story spans public pages, authenticated routes, reusable primitives, accessibility, and responsive behavior, the design needs to create one coherent reference model that later implementation stories can apply incrementally.

## Goals / Non-Goals

**Goals:**
- Define one implementation-ready reference for frontend foundation behavior before feature stories diverge.
- Separate what must be consistent globally from what individual features may specialize locally.
- Establish design decisions that support gradual implementation in the existing React/Vite codebase rather than a one-time rewrite.
- Make future review straightforward by tying shell behavior, token use, state patterns, and ownership boundaries to explicit artifacts.

**Non-Goals:**
- Selecting a final marketing brand identity or campaign-specific visual language.
- Implementing every shared component or every future page in this change.
- Defining backend contracts beyond the UI state and entitlement information the frontend needs to render shared patterns.
- Achieving 92% coverage for code outside the defined frontend measured-source boundary, such as generated output, build configuration, or type-only declarations.
- Creating, renaming, or composing any future backend business module; those modules remain separate backlog changes governed by ADR-001.

## Decisions

### 1. Treat the foundation as a single cross-cutting capability

The change introduces one `frontend-foundation` capability instead of splitting shells, tokens, navigation, and accessibility into separate capabilities. The story describes these concerns as one reusable pack that future frontend stories must consult together, and splitting them now would create artificial seams before any existing spec structure exists.

Alternatives considered:
- Multiple capabilities such as `design-tokens`, `app-shell`, and `ui-states`: rejected because there is no current spec hierarchy, and the story treats them as one standard that should be reviewed together.

### 2. Use route-declared shell context as the organizing model

Public pages, authenticated pages, and transient task surfaces will be treated as distinct shell contexts selected by route or flow metadata. This keeps shell switching explicit, supports coherent public-to-authenticated transitions, and gives future stories a predictable place to declare unsaved-work handling and contextual actions.

Alternatives considered:
- Infer shell behavior only from URL structure: rejected because complex flows and transient surfaces may not map cleanly to path patterns.
- Use a single universal shell with local overrides: rejected because the story explicitly requires distinct public and authenticated patterns.

### 3. Centralize tokens and breakpoint rules before component proliferation

Typography, spacing, color, surfaces, motion, and breakpoint values should come from shared tokens that future screens and reusable components consume. This reduces visual drift and gives reviewers a concrete validation rule: new screens should not introduce undocumented values.

Alternatives considered:
- Allow teams to establish local values first and consolidate later: rejected because the story identifies consistency drift as the problem to prevent.

### 4. Standardize state patterns around a shared status model

Loading, empty, success, error, premium-lock, offline, and unsaved-work-risk treatments should be implemented as a consistent pattern family, backed by explicit status models for non-trivial workflows. This lets future features share UI behavior even when they use different APIs.

Alternatives considered:
- Let each feature choose its own async-state representation: rejected because it undermines the story's goal of recognizable, reusable interaction patterns.

### 5. Keep shared primitives presentation-first and feature logic feature-owned

Shared components should remain presentation-first where practical, while orchestration, entitlement decisions, and route-aware behavior stay in feature-level containers, hooks, or state modules. This aligns with the repository's frontend feature boundary rule and keeps shared primitives reusable across modules.

Alternatives considered:
- Embed feature logic in shared components for convenience: rejected because it couples unrelated flows and makes later extension harder.

### 6. Model themes as semantic token sets with a persisted selection

Theme variants will supply the same semantic token names for canvas, surface, text, border, action, and status roles. A small theme preference layer will resolve the default and restore a user choice on the same device without putting color decisions in feature components. Existing light and dark styles are the migration source, but the supported theme set will be explicit and extensible.

Alternatives considered:
- Let each feature own its own visual theme: rejected because it breaks cross-route consistency and makes user preference impossible to apply predictably.
- Couple themes to resume templates: rejected because application preference and document presentation are distinct user decisions.

### 7. Separate resume content from template renderers

The resume model will remain the single source of user-entered information. The template dropdown will select a template identifier, and a template registry will resolve that identifier to a renderer that consumes the same normalized resume data. The renderer may change layout and visual treatment but cannot own, transform destructively, or duplicate the resume data state.

Alternatives considered:
- Maintain a separate data model per template: rejected because users would lose consistency and template switching would require migration between models.
- Keep template choices as labels only: rejected because the visible presentation would not reflect the user's selection.

### 8. Build a small composable component library before extending feature-local controls

The initial component set will cover buttons, form inputs, text areas, radio buttons, checkboxes, autocomplete, dropdowns, removable pills, pagination, and sortable action lists with fixed headers. Components will use shared tokens, native or established accessible primitives where appropriate, and public props that keep feature orchestration outside the component. Existing `Button`, field components, and `SelectField` will be consolidated or evolved rather than replaced without reason.

Alternatives considered:
- Build components only when each feature requests one: rejected because current resume controls already duplicate interaction and styling decisions that the foundation is intended to govern.
- Adopt a complete external component suite: rejected for now because the existing stack already has compatible low-level primitives and the story needs a focused product contract, not a wholesale visual-system replacement.

### 9. Establish a constrained shared-utility layer

`shared/lib` will contain only reusable, framework-neutral helpers. The initial utility inventory is class-name composition, display formatting, form-value normalization, safe browser storage access, and error normalization. Utilities will be pure where practical and must have explicit fallback behavior for browser APIs so they can be exhaustively tested.

Alternatives considered:
- Create a broad generic utility package preemptively: rejected because it would create ownership ambiguity and unused abstractions.

### 10. Treat 92% coverage as an enforced foundation quality gate

The test setup will use Vitest with a browser-like component-test environment and coverage instrumentation. The coverage command will measure application source under `src` while excluding generated artifacts, type-only declarations, and test/support files; all exclusions will be listed in versioned configuration. The gate will require at least 92% lines, branches, functions, and statements, and the test design will favor small pure utilities and composable components to keep that bar sustainable.

Alternatives considered:
- Retain a 100% initial threshold: rejected because the current greenfield implementation includes substantial pre-foundation client behavior whose fully exhaustive coverage would delay the foundation; 92% remains a strict quality gate while preserving delivery momentum.
- Count configuration and non-instrumentable code: rejected because it makes the metric misleading rather than measuring executable application behavior.

### 11. Make the foundation module-ready without becoming a business module

ADR-001 classifies `Frontend Foundation` as shared frontend and application-shell work, not a standalone backend business module. The shared layer will therefore own only cross-cutting presentation contracts: shell contexts, semantic themes, common components, generic utilities, standard UI states, and frontend integration conventions. Each business feature will own its client-side adapter, module API client, response-to-view-model mapping, authorization and entitlement interpretation, route composition, and workflow state.

This produces a consistent future integration model:
- `ProfilingModule` can provide authenticated user preferences, including a server-backed theme preference, while the frontend foundation retains its same-device fallback.
- `TemplatesModule` can provide template catalog metadata, availability, imports, and template administration rules, while the resume feature owns selection and the frontend template registry only resolves presentation renderers.
- `BillingModule` and other entitlement-owning modules can provide eligibility decisions, while feature adapters translate those decisions into the foundation's premium-lock and error patterns.
- Other backlog modules, including discovery, AI assistance, ATS analysis, export, review sharing, job tracking, notifications, privacy, support, administration, moderation, observability, and configuration, can add feature routes without changing the shared component API to carry module-specific rules.

Alternatives considered:
- Put module-specific API clients and business-state stores in `shared`: rejected because it violates ADR-001's module boundaries and would make unrelated features transitively depend on each other.
- Create one frontend module per future backend module now: rejected because no such feature contracts exist yet and premature client packages would duplicate or guess at module behavior.
- Make the foundation depend directly on future module implementations: rejected because it would prevent independent delivery and testing of the shared layer.

## Risks / Trade-offs

- Broad foundation scope may invite vague implementation work -> Mitigation: tasks break the change into shell, token, state, and governance deliverables with explicit verification points.
- Early token decisions may need refinement once more screens exist -> Mitigation: define the contract and adoption rules now, while allowing future stories to extend tokens through documented changes.
- Unsaved-work and offline behavior can vary by feature complexity -> Mitigation: standardize the required interaction pattern and route metadata contract now, while allowing feature stories to tailor the trigger logic.
- A single capability may become dense -> Mitigation: keep the spec focused on observable behavior and move implementation detail into design and future delivery stories.
- A 92% coverage requirement can still slow changes or encourage low-value tests -> Mitigation: constrain the measured source, require meaningful state and fallback assertions, and keep configuration exclusions explicit and reviewable.
- Template renderers may drift while sharing the same data -> Mitigation: test each renderer against common fixture data and template-switch preservation scenarios.
- Common components may become an oversized design system -> Mitigation: deliver the explicitly required initial set and add further components only when repeated feature needs justify them.
- Future backend modules may expose inconsistent loading, authorization, entitlement, or error contracts -> Mitigation: require each feature adapter to normalize its own module contract to the shared UI-state model and cover the mapping with tests.
- A server-backed preference may be unavailable or stale -> Mitigation: preserve a safe same-device theme fallback and reconcile only through the profile-preference feature boundary.

## Migration Plan

1. Approve this capability and use it as the reference for subsequent frontend stories.
2. Implement the test and coverage configuration first, define the measured-source scope, and add tests before or alongside every foundation behavior.
3. Implement theme preference and semantic token sets, then migrate current shared shell and components to consume them.
4. Implement the common component and utility inventory, evolve existing shared controls, and add the template registry and renderers over the shared resume-information model.
5. Document and implement the feature-adapter convention so future backend modules can map their own contracts to shared presentation patterns without entering `shared`.
6. Update in-flight and new frontend stories to reference this capability instead of redefining baseline behavior.
7. During rollout, treat deviations as explicit follow-up changes so the foundation remains authoritative. Roll back by restoring the prior shared component or theme implementation while retaining persisted user content and template selection.
