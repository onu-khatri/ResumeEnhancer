$skillBodies = @{
  "agent-orchestration-improver" = @'
---
name: agent-orchestration-improver
description: Improve how Codex decomposes and coordinates multi-agent work for ResumeEnhancer, especially for parallel story execution, research, review, and implementation. Use when a task is large enough to benefit from structured delegation, validation, and synthesis.
---

# Agent Orchestration Improver

Use this skill to strengthen existing ResumeEnhancer orchestration flows instead of inventing clever delegation for its own sake.

## Use this skill when

- a story or initiative is large enough to split across specialized agents
- an existing kickoff or background-agent flow is producing overlap, rework, or missed context
- you need a safer pattern for branch isolation, sequencing, or synthesis

## Do not use this skill when

- one focused agent can complete the work safely end to end
- there is no baseline workflow, failure mode, or evaluation target to improve
- delegation would add ceremony without reducing risk

## Improvement workflow

1. Establish the baseline workflow, examples, and failure symptoms.
2. Separate the work into roles such as research, architecture, backend, frontend, review, and packaging.
3. Define what context each agent truly needs and what should stay with the parent coordinator.
4. Add short approval checkpoints before destructive or branch-shaping actions.
5. Validate the revised orchestration on realistic repository tasks before treating it as the new default.

## Review lenses

- context size and prompt clarity
- duplicated work across agents
- unsafe parallel edits to shared contracts, migrations, or cross-cutting UI state
- unclear synthesis ownership
- missing validation or rollback points

## ResumeEnhancer focus

- user-story kickoff with readiness checks
- isolated branches or worktrees per story
- frontend and backend split only after shared contract risks are known
- final synthesis that reports touched layers, verification, and blockers

## Output requirements

- baseline issues
- recommended delegation boundaries
- required approval checkpoints
- validation plan
- simplification guidance if orchestration becomes heavier than the task

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/agent-orchestration-improve-agent
'@
  "architecture-decision-records" = @'
---
name: architecture-decision-records
description: Write clear architecture decision records for ResumeEnhancer that capture context, options, tradeoffs, status, and consequences in project-specific language. Use when Codex needs to document a meaningful technical decision or proposed architecture change.
---

# Architecture Decision Records

Use this skill to create durable ADRs that another engineer or agent can understand months later without this conversation.

## Use this skill when

- a technical decision changes module boundaries, integration style, persistence, deployment, or testing expectations
- multiple options were considered and the tradeoff should remain visible
- a design should be documented before or alongside implementation

## Do not use this skill when

- the change is a minor implementation detail
- the note belongs in a code comment, commit message, or PR description instead
- there is no actual decision to record

## ADR workflow

1. Capture the business and technical context that forced the decision.
2. Name the decision drivers explicitly: maintainability, delivery speed, security, performance, cost, or team familiarity.
3. Document the realistic options, not strawmen.
4. Record the chosen path, why it won, and what it costs.
5. State consequences, follow-up actions, and whether the ADR is proposed, accepted, rejected, deprecated, or superseded.

## Recommended structure

- title
- status
- date or version context
- problem and constraints
- decision drivers
- considered options
- decision
- consequences
- follow-up actions
- related stories, requirements, or ADRs

## ResumeEnhancer focus

- host versus module composition rules
- Web, AM, SL, PL, and DM ownership boundaries
- frontend and backend contract coordination
- migration, seeding, and testing implications

## Quality bar

- be specific enough that future contributors can repeat the reasoning
- document both upside and cost
- avoid abstract pattern language when the repository has concrete terms

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/architecture-decision-records
'@
  "architect-review" = @'
---
name: architect-review
description: Review modular and distributed architecture-sensitive changes for structural risk, quality attributes, and evidence-backed corrective direction. Use when design-level review is needed beyond local correctness.
---

# Architect Review

Use this skill for evidence-led design review when code correctness alone is not enough.

## Use this skill when

- a diff or design changes ownership, dependency direction, contracts, composition, or long-lived abstractions
- the main question is architecture fit, structural risk, or future maintenance cost

## Do not use this skill when

- the task is only a local correctness review
- there is no architecture-sensitive behavior

## Knowledge Routing

1. Read `KnowledgeBase/INDEX.md`.
2. Read `architecture-review.knowledge.md` before producing findings.
3. Select modular-monolith mode by default; use distributed mode only for remote boundaries, asynchronous messaging, independent deployment, eventual consistency, resilience, or distributed observability.
4. In distributed mode, read `distributed-architecture-review.knowledge.md`.
5. Read `dotnet-modular-architecture.knowledge.md` only for applicable architecture lenses.
6. For ResumeEnhancer facts, read `resumeenhancer-architecture-routing.knowledge.md`, then the authority it identifies.

## Specialist Gates

- Load `$domain-driven-design` for business vocabulary, bounded-context, aggregate, lifecycle, or invariant risk.
- Load `$backend-security-coder` for trust, authorization, sensitive-data, secret, abuse, privacy, or export risk.
- Load `$performance-optimization` for observed or visible latency, throughput, query, cache, resource, or capacity risk.
- Load `$deep-research` before concrete distributed platform or pattern recommendations unsupported by repository evidence.
- Recommend `$architecture-decision-records` for durable boundary, integration, persistence, deployment, or test-expectation decisions.

## Review Gate

Do not label a concern architectural without evidence of a boundary, dependency, operational, or evolution consequence. Mark uninspected behavior as unverified.

Do not enter distributed mode merely because a change is large or future-facing. Do not present a technology preference as a requirement, violation, or solution without quality-attribute evidence and constraints.

## Output Requirements

- findings first
- Architecture Impact: High, Medium, or Low, with affected quality attributes and rationale
- severity, evidence, impact, and smallest corrective direction
- blocking defects separate from significant risks and watch items
- required specialist skill, research, or ADR action and why
- residual risks and verification gaps stated explicitly

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/architect-review
'@
  "backend-feature-development" = @'
---
name: backend-feature-development
description: Implement backend features in ResumeEnhancer using the existing .NET modular architecture, Minimal APIs, FluentValidation, Mediator handlers, Mapster, repositories, EF Core, and tests. Use when Codex needs to add or modify backend behavior in a project-consistent way.
---

# Backend Feature Development

Use this skill to move from a ResumeEnhancer story to review-ready backend code without losing architectural discipline.

## Use this skill when

- a story changes API behavior, validation, service logic, persistence, or backend tests
- the task spans multiple backend layers and needs coordinated delivery
- contract, migration, or rollout risks should be called out early

## Do not use this skill when

- the task is frontend-only
- the change is a tiny local bug fix with no workflow value
- you only need architecture review without implementation

## Delivery workflow

1. Start from the user story, acceptance criteria, and existing behavior.
2. Identify the entry point: endpoint, background flow, or service contract.
3. Update request or response contracts only if the external behavior truly changes.
4. Add or refine validation in the Web layer.
5. Implement business behavior in the SL layer with focused handlers and mapping.
6. Extend persistence abstractions only when the behavior requires it.
7. Implement repository or EF Core details in PL.
8. Add or update tests at the narrowest useful boundary.

## Implementation lenses

- backward compatibility of contracts
- explicit validation and business preconditions
- handler cohesion and mapper correctness
- repository query shape and transaction safety
- migration, seed data, and rollout implications

## ResumeEnhancer focus

- `ResumeModuleWeb` request validation and endpoint wiring
- `ResumeModelSL` contracts, handlers, and mapping
- `ResumeModulePL` repository and EF Core behavior
- traceability from story language to code and tests

## Output requirements

- impacted layers
- contract changes
- persistence and migration notes
- verification and test coverage summary

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/backend-development-feature-development
'@
  "backend-security-coder" = @'
---
name: backend-security-coder
description: Implement and review secure backend code for ResumeEnhancer with OWASP-oriented practices around validation, authorization, data handling, logging, and abuse resistance. Use when Codex is changing APIs, persistence flows, auth-sensitive logic, or other backend attack surfaces.
---

# Backend Security Coder

Use this skill to apply practical backend security during implementation and review, not as an afterthought.

## Use this skill when

- backend work touches authentication, authorization, exports, admin actions, privacy, file handling, or AI usage limits
- an endpoint or repository flow crosses a trust boundary
- security-sensitive code needs a dedicated implementation pass

## Do not use this skill when

- the task has no meaningful backend trust or data-handling risk
- a general code-quality review is enough

## Security workflow

1. Identify the trust boundary and who is allowed to perform the action.
2. Validate request shape and business preconditions explicitly.
3. Enforce ownership and authorization close to the action.
4. Minimize data exposure in responses, logs, and exceptions.
5. Review abuse paths such as enumeration, replay, and unbounded resource use.
6. Verify the security behavior with targeted tests when practical.

## Security lenses

- broken access control and IDOR risk
- input validation and injection risk
- sensitive data exposure
- secret and token handling
- rate limiting and abuse resistance
- auditability and safe failure behavior

## ResumeEnhancer focus

- user-owned resume data
- admin or support-only operations
- file import or export handling
- AI-adjacent usage quotas, billing, or entitlement checks

## Output requirements

- identified trust boundaries
- blocking versus hardening issues
- concrete code-level mitigation steps
- verification notes

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/backend-security-coder
'@
  "clean-code" = @'
---
name: clean-code
description: Keep ResumeEnhancer changes readable, cohesive, and easy to extend without introducing unnecessary abstractions or duplication. Use when Codex is implementing, refactoring, or reviewing code quality across backend or frontend areas.
---

# Clean Code

Use this skill to improve clarity and maintainability while preserving the repository's real architecture and delivery pace.

## Use this skill when

- writing new code that should stay easy to understand and modify
- refactoring code that works but is painful to change
- reviewing whether structure, naming, or abstraction quality is slipping

## Do not use this skill when

- the main concern is security, architecture, or deployment strategy instead
- an abstraction would make simple code harder to follow

## Principles

- name things by responsibility, not implementation accident
- keep units of code focused
- prefer explicit control flow over cleverness
- reduce duplication only when the resulting shape is clearer
- let the repository structure explain intent where possible

## Review lenses

- intention-revealing names
- function and component size
- side effects and hidden coupling
- comments versus expressive code
- complexity added for speculative reuse

## ResumeEnhancer focus

- handlers that do one business job well
- validators that are declarative and narrow
- API clients and hooks that do not mix UI rendering concerns
- tests whose names explain behavior, not implementation trivia

## Output requirements

- specific readability or cohesion issues
- refactor suggestion with rationale
- risks of over-abstraction when relevant

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/clean-code
'@
  "code-refactoring-tech-debt" = @'
---
name: code-refactoring-tech-debt
description: Reduce technical debt in ResumeEnhancer through scoped refactoring that preserves behavior while improving structure, duplication, and maintainability. Use when Codex needs to clean up existing code, prepare for new features, or address architectural friction.
---

# Code Refactoring Tech Debt

Use this skill to make the codebase cheaper to change without disguising a redesign as a refactor.

## Use this skill when

- a code area is slowing feature work through duplication, unclear ownership, or brittle structure
- a cleanup should happen before new behavior is added
- you want a safer, reviewable refactor plan

## Do not use this skill when

- the proposed change is really a redesign
- there is no reliable verification path for preserved behavior

## Refactoring workflow

1. Name the debt precisely: duplication, mixed responsibilities, unstable tests, poor boundaries, or query sprawl.
2. Establish a behavior baseline using tests, fixtures, or observable current behavior.
3. Refactor in small passes that keep the diff understandable.
4. Stop when the code becomes easier to change; do not continue polishing for its own sake.
5. Record remaining debt separately if it does not belong in the same change.

## Review lenses

- preserved behavior
- reduced cognitive load
- smaller blast radius for future changes
- fewer hidden dependencies
- improved testability

## ResumeEnhancer focus

- bloated handlers or repositories
- duplicated frontend form and mapping logic
- persistence concerns leaking upward
- cross-layer naming drift that weakens story traceability

## Output requirements

- current debt statement
- proposed refactor boundary
- verification approach
- residual debt left intentionally untouched

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/code-refactoring-tech-debt
'@
  "production-code-reviewer" = @'
---
name: production-code-reviewer
description: Perform production-grade code review for ResumeEnhancer with a defect-first mindset across architecture, correctness, security, tests, and maintainability. Use when Codex needs to review diffs, pull requests, or uncommitted changes before merge.
---

# Production Code Reviewer

Use this skill to produce high-signal review findings that protect correctness and delivery quality without devolving into style nitpicks.

## Use this skill when

- reviewing pull requests, staged changes, or local diffs
- auditing risk before merge
- preparing or teaching repository-specific review standards

## Do not use this skill when

- there are no implementation changes to inspect
- the task is only to write code, not review it
- the user needs architecture advice without a concrete diff

## Review workflow

1. Read `AGENTS.md`, the user story, and any touched requirements.
2. Inspect the real diff and enough surrounding code to understand the full execution path.
3. Check architecture fit, correctness, validation, mapping, persistence behavior, and tests.
4. Look for security, performance, and rollout risks in proportion to the change.
5. Report findings by severity with concrete impact and narrow file references.

## Review lenses

- correctness and regression risk
- module boundary discipline
- validator, mapper, and contract consistency
- query shape, transaction safety, and error handling
- user-facing loading, error, and permission states
- tests that prove the changed behavior

## Read these references when needed

- `references/implementation-playbook.md` for detailed code-review patterns
- `references/review-playbook.md` for the ResumeEnhancer review sequence
- `references/review-checklist.md` for systematic checklist coverage
- `references/ai-review-playbook.md` for AI-assisted review and triage patterns

## Output requirements

- findings first
- severity-ordered issues
- concise risk summary
- verification and testing gaps

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/code-review-excellence
'@
  "deep-research" = @'
---
name: deep-research
description: Perform deep, evidence-based research for ResumeEnhancer across product requirements, user stories, code, tests, and external sources when needed. Use when Codex needs a thorough answer before implementation, architecture, or planning decisions.
---

# Deep Research

Use this skill when a shallow code read would create avoidable risk and the answer needs evidence from multiple sources.

## Use this skill when

- the question spans requirements, code, tests, and current external information
- implementation or planning depends on a reliable understanding first
- there is meaningful ambiguity or cross-cutting impact

## Do not use this skill when

- a short local inspection can answer the question confidently
- current external data is unnecessary

## Research workflow

1. Start with local sources: requirements, stories, code, tests, and architecture notes.
2. Separate observed facts from inferred conclusions.
3. Use external sources only when the topic is current, user-requested, or not fully represented locally.
4. Synthesize around the actual decision the user must make.

## Evidence order

1. repository facts
2. tests and executable behavior
3. current external documentation or standards
4. recommendations grounded in the first three

## Output requirements

- findings grouped by theme
- evidence summary
- open questions
- implications for design, implementation, or rollout

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/deep-research
'@
  "domain-driven-design" = @'
---
name: domain-driven-design
description: Apply pragmatic domain modeling to clarify business language, context boundaries, invariants, and model responsibilities. Use when business complexity materially affects architecture or behavior.
---

# Domain Driven Design

Use this skill pragmatically. The goal is clearer business modeling, not performative DDD vocabulary.

## Use this skill when

- business concepts, rules, or ownership boundaries are ambiguous
- lifecycle, policy, or state-transition invariants affect design
- a design needs a bounded-context, aggregate, value-object, or domain-service decision

## Do not use this skill when

- the task is straightforward data maintenance with no meaningful invariants
- the issue is purely technical and has no business-model consequence

## Knowledge Routing

1. Read `KnowledgeBase/INDEX.md`.
2. Read `domain-modeling.knowledge.md` before proposing a domain-modeling pattern.
3. Read `dotnet-modular-architecture.knowledge.md` when the model decision changes dependencies, composition, or integration seams.
4. For ResumeEnhancer adaptation, read `resumeenhancer-architecture-routing.knowledge.md`, then the authority it identifies.

## Workflow Gate

Stop after the viability assessment when the task has no meaningful invariant, divergent context, or lifecycle rule. Do not introduce tactical DDD patterns for simple CRUD.

## Output Requirements

- DDD viability assessment
- vocabulary, ownership, and invariant decisions when modeling is justified
- explicit translation and integration boundaries when applicable
- evidence and ADR recommendation for durable context or consistency choices

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/domain-driven-design
'@
  "dotnet-architect" = @'
---
name: dotnet-architect
description: Design .NET backend and modular-application architecture with explicit ownership, dependency, composition, integration, and verification decisions. Use when a change needs architecture judgment before implementation.
---

# Dotnet Architect

Use this skill when an architecture decision needs .NET-aware reasoning rather than a local implementation pattern.

## Use this skill when

- a backend or cross-layer change needs ownership, dependency, composition, or integration decisions
- a new abstraction, module seam, registration boundary, or durable architecture choice is proposed
- architecture guidance must account for application, domain, transport, persistence, and host responsibilities together

## Do not use this skill when

- the task is not meaningfully architecture-sensitive
- an established local pattern answers the question without a boundary decision
- you only need a design review of an existing change; use `$architect-review`

## Knowledge Routing

1. Read `KnowledgeBase/INDEX.md`.
2. Read `dotnet-modular-architecture.knowledge.md` before selecting a boundary, dependency, composition, or integration pattern.
3. Read `domain-modeling.knowledge.md` only when business boundaries, language, or invariants are material.
4. For ResumeEnhancer adaptation, read `resumeenhancer-architecture-routing.knowledge.md`, then the authority it identifies.
5. Retrieve Group 1 API/application or EF/persistence knowledge only when that decision area is affected.

## Workflow

1. Clarify the behavior, constraints, and affected owners.
2. Make the smallest boundary decision that preserves dependency direction and explicit composition.
3. Evaluate lifecycle, configuration, failure, and test consequences.
4. State alternatives and tradeoffs when the choice is durable.
5. Use `$architecture-decision-records` when the project needs the decision recorded.

## Output Requirements

- proposed ownership and dependency direction
- applicable project authority and any unresolved conflict
- composition and verification consequences
- ADR recommendation when the decision is durable

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/dotnet-architect
'@
  "dotnet-backend-patterns" = @'
---
name: dotnet-backend-patterns
description: Apply .NET backend implementation patterns for API boundaries, application behavior, persistence, and testing. Use when a change needs pattern selection or a framework-aware implementation review.
---

# Dotnet Backend Patterns

Use this skill to select and apply the smallest proven .NET backend pattern that fits the change and the target project.

## Use this skill when

- implementing or reviewing .NET backend code
- you need a pattern for API boundaries, application behavior, persistence, or verification
- a change should align with proven patterns instead of introducing a new stack shape

## Knowledge routing

1. Check `KnowledgeBase/INDEX.md`.
2. Read the API/application delivery topic for request flow, validation, mapping, error behavior, and test-boundary choices.
3. Read the EF Core persistence topic for model, query, transaction, migration, or initialization choices.
4. Read the target project's adaptation topic before applying project-specific frameworks, composition conventions, or abstractions.

## Pattern selection

- Preserve explicit transport, application, domain, and persistence boundaries.
- Prefer established project patterns over introducing a framework or abstraction without a demonstrated need.
- Treat contract compatibility, validation, cancellation, error behavior, query shape, and verification as deliberate design decisions.
- Do not assume a particular mediator, mapper, repository, unit-of-work, or validation library unless project guidance requires it.

## Output requirements

- impacted backend layers
- selected pattern and rationale
- persistence and testing notes

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/dotnet-backend-patterns
'@
  "frontend-dev-guidelines" = @'
---
name: frontend-dev-guidelines
description: Build and review ResumeEnhancer React frontend code with current architecture, typed data flows, accessible UI states, and proportionate performance practices. Use for feature, component, form, route, or client-data changes.
---

# Frontend Development Guidelines

Use this skill to make frontend changes that are reliable, maintainable, and native to the existing React/Vite client. Preserve verified local patterns; do not import a framework doctrine from another application.

Read `KnowledgeBase/INDEX.md`, inspect the target route, feature, shared primitive, and nearest tests, then read [project adaptation](references/project-adaptation.md).

For non-trivial work, assess the approach with [decision triage](references/decision-triage.md). Use the smallest relevant guide for [client architecture](references/client-architecture.md), [data and state](references/data-and-state.md), [routing and styling](references/routing-and-styling.md), [accessibility](references/accessibility-and-responsive.md), [TypeScript and performance](references/typescript-and-performance.md), or [testing](references/testing-guide.md).

Keep network access typed and centralized, model all user-visible async states, keep forms schema-led, preserve responsive keyboard-accessible interaction, and report exactly which validation ran.
'@
  "frontend-design" = @'
---
name: frontend-design
description: Design polished, intentional interfaces for ResumeEnhancer that fit the product domain, existing implementation patterns, and accessibility expectations. Use when Codex needs to shape layout, hierarchy, copy, and interaction design before or during frontend implementation.
---

# Frontend Design

Use this skill to shape product-quality experiences instead of interchangeable component grids.

## Use this skill when

- the task is primarily about layout, hierarchy, motion, copy, or interaction quality
- a page or flow needs a deliberate visual direction before implementation
- you need design guidance that still respects the current React client

## Do not use this skill when

- the work is purely backend or data-layer oriented
- implementation mechanics matter more than design direction

## Design workflow

1. Clarify the user goal, emotional tone, and business outcome.
2. Work from information hierarchy before styling details.
3. Define interaction states, not just the default view.
4. Keep accessibility, responsiveness, and implementation realism in scope.
5. Hand off design decisions in a way the frontend skill can implement cleanly.

## Design lenses

- hierarchy and readability
- strong visual identity
- purposeful motion
- empty, loading, and error states
- mobile behavior and accessibility

## ResumeEnhancer focus

- professional but distinctive product surfaces
- resume-centric workflows and credibility cues
- alignment with existing shared UI and route structure

## Output requirements

- visual direction
- state and interaction notes
- content hierarchy
- implementation constraints or handoff notes

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-design
'@
  "frontend-developer" = @'
---
name: frontend-developer
description: Deliver production-ready ResumeEnhancer React features with correct routes, typed client data, accessible responsive behavior, and focused verification. Use when building or changing product UI, hooks, forms, or client integration.
---

# Frontend Developer

Use this skill to turn approved frontend behavior into implementation-ready code that fits the current React/Vite client. `$frontend-dev-guidelines` owns detailed shared standards; this skill owns delivery sequencing and evidence.

## Use this skill when

- building or modifying ResumeEnhancer UI, routes, hooks, forms, or API integration
- turning user stories into frontend code within the established client architecture

## Do not use this skill when

- the work is purely backend or persistence design
- the task is design-only without implementation

## Workflow

1. Identify the page, route, feature boundary, and API dependencies.
2. Reuse existing shared UI, hooks, and typed models before creating new primitives.
3. Implement typed service, query/mutation, form/state, and presentation flow.
4. Implement complete pending, empty, error, success, and permission states.
5. Keep form state, schema, and API models aligned; verify responsive accessible behavior and focused tests.

## ResumeEnhancer focus

- feature-scoped code under the existing frontend structure
- typed interaction with `shared/api`
- route-aware page composition
- user-story traceability from UI behavior to service calls
- no Next.js, server-component, or uninstalled-tooling assumptions without repository evidence

## Output requirements

- file placement
- state and route approach
- API integration notes
- verification checklist

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-developer
'@
  "frontend-security-coder" = @'
---
name: frontend-security-coder
description: Implement and review secure frontend behavior in ResumeEnhancer with attention to auth flow, data exposure, user input handling, DOM safety, redirects, and safe API interaction. Use when Codex is changing client-side behavior with security or privacy implications.
---

# Frontend Security Coder

Use this skill for client-side security work that should prevent real browser-side vulnerabilities instead of relying on backend protection alone.

## Use this skill when

- the UI handles authentication, tokens, redirects, user-generated content, or sensitive data
- a component or page renders untrusted input
- a frontend change needs a dedicated security-focused implementation or review pass

## Do not use this skill when

- the task has no meaningful client-side trust boundary
- ordinary frontend guidance is sufficient

## Security workflow

1. Identify trusted versus untrusted content and where it enters the UI.
2. Prefer safe DOM patterns and avoid unsafe rendering shortcuts.
3. Validate redirect targets, route parameters, and user-controlled links.
4. Keep authentication and session behavior aligned with server expectations.
5. Review error states, analytics, and logs for accidental information leakage.

## Security lenses

- XSS and unsafe HTML rendering
- open redirect or navigation abuse
- token and session handling
- sensitive state exposure in UI or storage
- third-party script or widget risk
- privacy and consent-aware telemetry

## ResumeEnhancer focus

- resume content rendered back to the user
- auth, account, or entitlement screens
- export, upload, or rich-text flows
- frontend handling of backend security assumptions

## Output requirements

- identified client-side threat areas
- blocking versus hardening issues
- concrete implementation guidance
- verification notes

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-security-coder
'@
  "frontend-slides" = @'
---
name: frontend-slides
description: Create presentation-ready frontend walkthroughs, implementation summaries, and UI narratives for ResumeEnhancer. Use when Codex needs to turn frontend work into stakeholder-friendly slides, demos, or visual explanations.
---

# Frontend Slides

Use this skill to turn product or implementation work into clear, polished presentation material without losing technical truth.

## Use this skill when

- the deliverable is a presentation, demo narrative, or HTML slide deck
- frontend work needs to be explained to stakeholders, leadership, or teammates
- you need a visual story, not just bullet points

## Do not use this skill when

- the primary deliverable is production application code
- a short written summary would serve better than slides

## Workflow

1. Clarify the audience, purpose, and desired level of technical depth.
2. Build the slide story around outcomes, flows, and proof points.
3. Prefer screenshots, before-and-after framing, architecture callouts, and concise captions.
4. Separate shipped behavior, planned work, and open risks.
5. If building browser-based slides, keep them lightweight and demo-safe.

## Slide design lenses

- strong narrative arc
- one idea per slide
- visual hierarchy over dense text
- honest status labeling
- speaker-note-ready details when needed

## ResumeEnhancer focus

- user journey changes
- frontend architecture walkthroughs
- implementation status for stories or milestones
- risks, dependencies, and next steps

## Output requirements

- audience and goal
- suggested slide sequence
- evidence to show on each slide
- status and risk callouts

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-slides
'@
  "production-ui-generator" = @'
---
name: production-ui-generator
description: Generate production-grade UI for ResumeEnhancer with intentional design, accessibility, component reuse, theming, and implementation-ready React patterns. Use when Codex needs to create or improve pages, flows, feature UIs, or polished product surfaces in the React/Vite client.
---

# Production UI Generator

Use this skill when the UI should feel launch-ready, deliberate, and implementable in the current client stack.

## Use this skill when

- building or upgrading a major product surface
- a user flow needs stronger hierarchy, polish, and state handling
- the UI must balance aesthetics with reusable implementation patterns

## Do not use this skill when

- the task is only backend behavior
- a tiny styling tweak does not need a broader UI system pass

## Workflow

1. Read the story and inspect the current page or component context.
2. Define user goals and critical states before drawing the default layout.
3. Reuse shared UI, hooks, and typed models wherever possible.
4. Choose a clear theme direction and interaction language.
5. Hand off implementation notes that fit React, TypeScript, and the current shared primitives.

## UI lenses

- hierarchy, spacing, and rhythm
- themed surfaces and strong contrast
- meaningful motion rather than decoration
- mobile and desktop behavior
- empty, loading, error, success, and entitlement states

## ResumeEnhancer focus

- professional and trustworthy product tone
- resume-centric workflows with clear progress feedback
- compatibility with the existing client architecture, not a separate design system fantasy

## Output requirements

- visual direction
- state plan
- reusable component notes
- implementation constraints and follow-up steps

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-ui-dark-ts
'@
  "full-stack-feature-orchestrator" = @'
---
name: full-stack-feature-orchestrator
description: Coordinate full-stack feature delivery in ResumeEnhancer across requirements, frontend, API, service layer, persistence, tests, and PR packaging. Use when Codex needs to drive an end-to-end feature from story to review-ready implementation.
---

# Full Stack Feature Orchestrator

Use this skill when a feature spans multiple layers and success depends on coordinated delivery rather than isolated edits.

## Use this skill when

- a story touches frontend, backend, persistence, and tests
- there are dependencies or rollout concerns across layers
- the user wants a coherent path from story to review-ready implementation

## Do not use this skill when

- the task is small and isolated to one layer
- orchestration would add more process than value

## Workflow

1. Start from requirements, acceptance criteria, and current behavior.
2. Identify impacted layers: frontend, API, validation, handlers, persistence, tests, and PR packaging.
3. Surface shared-contract, migration, or concurrency risks before parallel work starts.
4. Decide which steps can be delegated and which must stay coordinated.
5. End with a verification, rollout, and review plan.

## Review lenses

- story traceability
- contract consistency across layers
- migration and deployment risk
- test coverage at the right boundaries
- reviewer clarity and PR packaging

## ResumeEnhancer focus

- frontend feature changes coordinated with Web and SL contracts
- persistence changes flagged early
- branch and worktree safety during multi-story delivery

## Output requirements

- phase plan
- impacted layers
- risks and dependencies
- verification and rollout notes

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/full-stack-orchestration-full-stack-feature
'@
}

function Set-FileText {
  param(
    [string]$Path,
    [string]$Content
  )

  $dir = Split-Path -Parent $Path
  if (-not (Test-Path $dir)) {
    New-Item -ItemType Directory -Path $dir | Out-Null
  }

  Set-Content -LiteralPath $Path -Value $Content -NoNewline
}

function Copy-MarkdownWithFooter {
  param(
    [string]$SourcePath,
    [string]$DestinationPath,
    [string]$UpstreamUrl
  )

  $content = Get-Content -LiteralPath $SourcePath -Raw
  $content = $content.TrimEnd("`r", "`n") + "`r`n`r`n## Upstream URL`r`n- $UpstreamUrl"
  Set-FileText -Path $DestinationPath -Content $content
}

function Copy-CSharpWithFooter {
  param(
    [string]$SourcePath,
    [string]$DestinationPath,
    [string]$UpstreamUrl
  )

  $content = Get-Content -LiteralPath $SourcePath -Raw
  $content = $content.TrimEnd("`r", "`n") + "`r`n`r`n// Upstream URL: $UpstreamUrl"
  Set-FileText -Path $DestinationPath -Content $content
}

$skillRoot = Join-Path $PSScriptRoot "..\skills"
foreach ($name in $skillBodies.Keys) {
  $path = [IO.Path]::GetFullPath((Join-Path $skillRoot "$name\SKILL.md"))
  Set-FileText -Path $path -Content $skillBodies[$name]
}

$upstreamRoot = Join-Path $env:TEMP "antigravity-awesome-skills-audit\skills"

Copy-MarkdownWithFooter `
  -SourcePath (Join-Path $upstreamRoot "dotnet-backend-patterns\references\dapper-patterns.md") `
  -DestinationPath ([IO.Path]::GetFullPath((Join-Path $skillRoot "dotnet-backend-patterns\references\dapper-patterns.md"))) `
  -UpstreamUrl "https://github.com/benjaminasterA/antigravity-awesome-skills/blob/main/skills/dotnet-backend-patterns/references/dapper-patterns.md"

Copy-CSharpWithFooter `
  -SourcePath (Join-Path $upstreamRoot "dotnet-backend-patterns\assets\repository-template.cs") `
  -DestinationPath ([IO.Path]::GetFullPath((Join-Path $skillRoot "dotnet-backend-patterns\assets\repository-template.cs"))) `
  -UpstreamUrl "https://github.com/benjaminasterA/antigravity-awesome-skills/blob/main/skills/dotnet-backend-patterns/assets/repository-template.cs"

Copy-CSharpWithFooter `
  -SourcePath (Join-Path $upstreamRoot "dotnet-backend-patterns\assets\service-template.cs") `
  -DestinationPath ([IO.Path]::GetFullPath((Join-Path $skillRoot "dotnet-backend-patterns\assets\service-template.cs"))) `
  -UpstreamUrl "https://github.com/benjaminasterA/antigravity-awesome-skills/blob/main/skills/dotnet-backend-patterns/assets/service-template.cs"

$reviewChecklist = @'
# ResumeEnhancer Review Checklist

Use this checklist when a review should be systematic rather than intuitive.

## Context

- confirm the user story or requirement being implemented
- confirm the change scope and whether unrelated files are mixed in
- confirm the changed layers: frontend, Web, AM, SL, PL, DM, tests

## Correctness

- does the change actually satisfy the acceptance criteria
- are edge cases and null or empty states handled
- are validators, handlers, and mappers consistent with each other
- are response shapes and UI expectations still aligned

## Security

- are ownership and authorization checks present where needed
- does the change expose secrets or sensitive user data
- are user inputs validated and safely rendered or persisted
- does logging avoid leaking protected information

## Performance

- are repository queries shaped appropriately
- is there unnecessary duplicate fetching or heavy client re-rendering
- are paging, filtering, or caching implications understood

## Maintainability

- are responsibilities in the right layer
- are names and abstractions clear enough for future changes
- does the diff add avoidable coupling or duplication

## Tests

- are tests present at the right boundary
- do they prove the changed behavior rather than implementation trivia
- are there obvious gaps in error, permission, or integration coverage

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/code-review-checklist
'@

$aiReviewPlaybook = @'
# AI Review Playbook

Use this note when combining human review judgment with AI-assisted triage.

## Goals

- use automation to widen coverage, not replace engineering judgment
- triage large diffs quickly before deep review
- focus human attention on security, contracts, architecture, and business behavior

## Practical flow

1. classify the change by size, language, and risk
2. use automated scanners or repository tests for fast signals
3. ask AI to reason about edge cases, architectural fit, and missing tests
4. verify AI findings against the real code before reporting them

## Good AI review prompts include

- the actual diff or changed files
- surrounding context needed to understand the call path
- the acceptance criteria or story intent
- explicit focus areas such as security, performance, or compatibility

## Do not trust AI review blindly

- verify every finding before surfacing it as a defect
- do not present speculative warnings as confirmed bugs
- prefer precise, reproducible findings over long generic commentary

## ResumeEnhancer focus

- contract drift between frontend and backend
- validator or mapper mismatches
- boundary leakage across Web, SL, and PL
- missing tests around permissions, error handling, and migrations

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/code-review-ai-ai-review
'@

Set-FileText `
  -Path ([IO.Path]::GetFullPath((Join-Path $skillRoot "production-code-reviewer\references\review-checklist.md"))) `
  -Content $reviewChecklist

Set-FileText `
  -Path ([IO.Path]::GetFullPath((Join-Path $skillRoot "production-code-reviewer\references\ai-review-playbook.md"))) `
  -Content $aiReviewPlaybook

$extendedReferences = @{
  "agent-orchestration-improver\references\optimization-playbook.md" = @'
# Agent Optimization Playbook

Use this playbook when the orchestration itself needs redesign rather than another prompt tweak.

## Baseline signals

- repeated user corrections
- duplicated work across delegated agents
- long prompts with weak task ownership
- synthesis steps that re-open work instead of closing it

## Improvement sequence

1. capture representative successful and failed runs
2. classify failures by prompt clarity, context loss, tool misuse, sequencing, or approval timing
3. simplify responsibilities before adding more delegation
4. test the revised workflow on realistic repository tasks

## Practical tactics

- keep one owner for final synthesis
- keep shared-contract work out of unsafe parallel lanes
- insert approval gates before branching, PR creation, or destructive actions
- prefer small reusable roles over giant “do everything” subagents

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/agent-orchestration-improve-agent
'@
  "architecture-decision-records\references\adr-patterns.md" = @'
# ADR Patterns

Use this note for higher-quality ADRs when the decision is important enough to revisit later.

## Patterns worth documenting

- platform and framework choices
- contract and integration styles
- persistence and migration strategy
- security or compliance architecture
- tradeoffs between delivery speed and long-term maintainability

## Good ADR habits

- write the real forcing context
- document realistic alternatives
- include negative consequences, not just the winning pitch
- link the decision to stories, code, and follow-up work

## Status model

- proposed
- accepted
- rejected
- deprecated
- superseded

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/architecture-decision-records
'@
  "backend-security-coder\references\security-review-guide.md" = @'
# Backend Security Review Guide

Use this note when the code touches a trust boundary or protected data path.

## Check for

- broken access control
- insecure direct object reference patterns
- injection and unsafe query composition
- sensitive data exposure in logs, errors, or responses
- replay, abuse, or quota bypass paths

## Mitigation mindset

- deny by default
- validate early
- expose the minimum necessary data
- make failures safe and observable

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/backend-security-coder
'@
  "code-refactoring-tech-debt\references\refactoring-playbook.md" = @'
# Refactoring Playbook

Use this playbook when behavior must stay stable while structure improves.

## Sequence

1. define the debt clearly
2. establish a verification baseline
3. isolate a narrow refactor boundary
4. refactor in reviewable steps
5. stop once the next change becomes easier

## Common debt themes

- mixed responsibilities
- duplicated branching logic
- query sprawl
- hidden coupling across layers
- tests that only prove implementation details

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/code-refactoring-tech-debt
'@
  "frontend-design\references\design-playbook.md" = @'
# Frontend Design Playbook

Use this playbook when the interface needs a stronger design point of view.

## Design sequence

1. clarify the audience and task outcome
2. define hierarchy before decoration
3. design non-default states intentionally
4. make mobile behavior a first-class constraint

## Avoid

- interchangeable card grids with no visual idea
- motion with no communication purpose
- inaccessible contrast or fragile spacing systems

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-design
'@
  "frontend-developer\references\implementation-playbook.md" = @'
# Frontend Implementation Playbook

Use this note for a route, form, remote-data, or multi-state feature. It complements `$frontend-dev-guidelines`; current source and requirements remain authoritative.

## Delivery sequence

1. confirm the user outcome, route entry, entitlement, API contract, and acceptance behavior
2. extend the nearest feature, shared UI, service, hook/store, and test before creating a new abstraction
3. align request/response, form, and view models at the feature boundary
4. implement service, query/mutation, local state, and page composition with complete visible states
5. validate keyboard, focus, responsive, theme, and reduced-motion behavior
6. add focused tests and run proportional client checks

## Common checks

- shared UI reused where appropriate
- API calls use `shared/api/api-client.ts` and models remain typed
- form values match validation, mapping, and backend expectations
- query invalidation and loading, error, success, and permission behavior are explicit

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-developer
'@
  "frontend-security-coder\references\client-security-playbook.md" = @'
# Client Security Playbook

Use this note when the browser-side code handles untrusted content or sensitive state.

## Check for

- unsafe HTML rendering
- token or session leakage
- insecure redirects or user-controlled navigation
- excessive data exposure in client storage, UI, or telemetry

## Safe defaults

- prefer safe DOM APIs
- validate redirect targets
- keep sensitive state short-lived and intentional
- treat third-party scripts and widgets as risk surfaces

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-security-coder
'@
  "frontend-slides\references\slides-playbook.md" = @'
# Slides Playbook

Use this note when a UI story needs to become a clear presentation.

## Core rules

- build the narrative around audience outcomes
- keep one main idea per slide
- prefer visuals, evidence, and concise captions over dense prose
- separate shipped work from proposed work

## Useful slide types

- before and after comparison
- user flow walkthrough
- architecture snapshot
- risks and next steps

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-slides
'@
  "production-ui-generator\references\theme-and-layout-playbook.md" = @'
# Theme And Layout Playbook

Use this note when the UI needs stronger visual structure and theming decisions.

## Focus areas

- clear visual hierarchy
- reusable surface and spacing rules
- intentional interaction and state styling
- launch-ready mobile and desktop behavior

## Practical approach

1. define the page purpose and key action
2. choose a clear theme direction
3. design states before polish
4. reuse shared primitives wherever possible

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-ui-dark-ts
'@
  "full-stack-feature-orchestrator\references\feature-delivery-playbook.md" = @'
# Feature Delivery Playbook

Use this note when a feature spans frontend, backend, persistence, and release preparation.

## Delivery sequence

1. confirm the story and acceptance criteria
2. identify cross-layer contract risks
3. plan frontend, backend, and test changes together
4. call out rollout, migration, and verification needs early

## Common risks

- API drift between layers
- shared-file conflicts during parallel work
- persistence changes without rollback notes
- incomplete test coverage at integration boundaries

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/full-stack-orchestration-full-stack-feature
'@
}

foreach ($relativePath in $extendedReferences.Keys) {
  Set-FileText `
    -Path ([IO.Path]::GetFullPath((Join-Path $skillRoot $relativePath))) `
    -Content $extendedReferences[$relativePath]
}
