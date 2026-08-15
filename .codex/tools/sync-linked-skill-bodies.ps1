$skillBodies = @{
  "agent-orchestration-improver" = @'
---
name: agent-orchestration-improver
description: Improve how Codex decomposes and coordinates multi-agent work for ResumeEnhancer, especially for parallel story execution, research, review, and implementation. Use when a task is large enough to benefit from structured delegation and synthesis.
---

# Agent Orchestration Improver

Systematic improvement of existing ResumeEnhancer-oriented agent workflows through performance analysis, prompt refinement, delegation boundaries, and validation loops.

## Use this skill when

- Improving an existing Codex workflow, subagent pattern, or orchestration plan
- Diagnosing why delegated work is inefficient, overlapping, or low quality
- Designing safer multi-agent delivery for ResumeEnhancer

## Do not use this skill when

- You are creating a brand-new agent from scratch without any workflow to improve
- There are no examples, failures, or evaluation targets to learn from
- A single agent can complete the work safely without delegation

## Instructions

1. Establish baseline examples, failure modes, or coordination pain points.
2. Identify where delegation boundaries, context size, sequencing, or approval timing are hurting results.
3. Improve prompts, synthesis responsibilities, and approval checkpoints.
4. Validate changes on realistic repository tasks before trusting them broadly.
5. Favor smaller, clearer agent responsibilities over overly clever orchestration graphs.

## ResumeEnhancer focus

- story kickoff and branch isolation
- research versus implementation splits
- backend versus frontend ownership
- conflict-aware parallelization around contracts, validators, migrations, and shared UI state

## Output requirements

- baseline issues
- proposed orchestration improvements
- validation strategy
- rollback or simplification guidance if the orchestration gets too complex

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/agent-orchestration-improve-agent
'@
  "architecture-decision-records" = @'
---
name: architecture-decision-records
description: Write clear architecture decision records for ResumeEnhancer that capture context, options, tradeoffs, and consequences in project-specific language. Use when Codex needs to document a meaningful technical decision or proposed architecture change.
---

# Architecture Decision Records

Document significant ResumeEnhancer decisions using durable ADRs that preserve context, options, rationale, and operational consequences.

## Use this skill when

- A technical decision affects module boundaries, persistence, security, testing, or rollout strategy
- You need durable architectural documentation for ResumeEnhancer
- A tradeoff should remain visible for future contributors and agents

## Do not use this skill when

- The change is a tiny implementation detail
- There is no actual architectural decision to record
- The note would be better captured in a code comment or PR description

## Instructions

1. Capture the decision context, constraints, and drivers.
2. Document considered options with honest tradeoffs.
3. Record the decision, rationale, and consequences.
4. Link related ADRs, user stories, or business requirements when available.
5. State whether the decision is proposed, accepted, deprecated, rejected, or superseded.

## Suggested ADR shape

- title
- status
- context
- decision drivers
- considered options
- decision
- consequences
- follow-up actions

## ResumeEnhancer focus

- host and module composition boundaries
- Web, AM, SL, PL, DM responsibilities
- frontend and backend contract synchronization
- migration, seeding, and testing consequences

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/architecture-decision-records
'@
  "architect-review" = @'
---
name: architect-review
description: Review ResumeEnhancer changes from an architecture perspective, focusing on layering, module boundaries, dependency direction, scalability, and long-term maintainability. Use when Codex needs a design-level review beyond line-by-line correctness.
---

# Architect Review

Perform a design-level review of ResumeEnhancer changes with emphasis on architecture fit, dependency direction, and future maintainability.

## Use this skill when

- The main question is whether a change fits the architecture
- You need a design-level review beyond correctness
- You want to assess layering, module boundaries, or long-term maintainability

## Do not use this skill when

- There is no architecture-sensitive change
- A pure defect review is enough without design analysis
- The task is only to generate implementation code

## Instructions

1. Review dependency direction, boundary ownership, and integration impact.
2. Check whether the host, Web, SL, PL, and DM roles remain clean.
3. Call out architectural shortcuts that make future work harder.
4. Prefer repository-specific reasoning over generic pattern advice.
5. Highlight what should be changed now versus what is a future architectural watch item.

## Review focus

- compile-time dependency direction
- module composition entry points
- transport versus domain versus persistence leakage
- API contract boundaries
- scalability, testability, and refactor cost

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/architect-review
'@
  "backend-feature-development" = @'
---
name: backend-feature-development
description: Implement backend features in ResumeEnhancer using the existing .NET modular architecture, Minimal APIs, FluentValidation, Mediator handlers, Mapster, repositories, and tests. Use when Codex needs to add or modify backend behavior in a project-consistent way.
---

# Backend Feature Development

Orchestrate backend feature work in ResumeEnhancer from requirement to handler, repository, and verification while preserving the current modular monolith architecture.

## Use this skill when

- Adding or changing backend behavior in ResumeEnhancer
- Coordinating contracts, validators, handlers, repositories, and tests
- Delivering a backend story end to end within the current architecture

## Do not use this skill when

- The work is frontend-only
- The task is a tiny isolated tweak that does not need workflow guidance
- You only need abstract architecture review without implementation

## Instructions

1. Start from the user story and requirement evidence.
2. Define or update AM contracts only if the API shape must change.
3. Add or update validators in `ResumeModuleWeb`.
4. Add or update Mediator contracts and handlers in `ResumeModelSL`.
5. Extend persistence abstractions in SL only when needed.
6. Implement repository or EF behavior in PL.
7. Add unit or integration tests at the right boundary.
8. Call out migration, seeding, or cache implications when relevant.

## Delivery checklist

- acceptance criteria mapped to code changes
- request validation covered
- handler logic isolated correctly
- persistence changes verified
- tests added at the right layer

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/backend-development-feature-development
'@
  "backend-security-coder" = @'
---
name: backend-security-coder
description: Implement and review secure backend code for ResumeEnhancer with OWASP-oriented practices around validation, authorization, data handling, and abuse resistance. Use when Codex is changing APIs, persistence flows, auth-sensitive logic, or other backend attack surfaces.
---

# Backend Security Coder

Apply security-focused implementation and review practices to ResumeEnhancer backend changes that touch trust boundaries, user-owned resources, or sensitive data.

## Use this skill when

- Backend work touches auth, sharing, admin, export, billing, privacy, or AI usage limits
- API and persistence changes need a security-focused implementation pass

## Do not use this skill when

- The task is unrelated to backend trust boundaries
- A generic code review is enough without security depth

## Instructions

1. Validate request shape and business preconditions.
2. Enforce ownership and authorization.
3. Minimize sensitive data exposure.
4. Use safe token and identifier generation.
5. Avoid logging secrets or private document content.
6. Prefer explicit failure handling over ambiguous partial success.

## Review focus

- auth and authorization
- input validation
- data exposure
- secret handling
- abuse resistance and rate controls

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/backend-security-coder
'@
  "clean-code" = @'
---
name: clean-code
description: Keep ResumeEnhancer changes readable, cohesive, and easy to extend without introducing unnecessary abstractions or duplication. Use when Codex is implementing, refactoring, or reviewing code quality across backend or frontend areas.
---

# Clean Code

Apply clean-code principles to ResumeEnhancer changes so future contributors and agents can read, change, and verify the code confidently.

## Use this skill when

- Writing new code that should be easy for the next engineer or agent to understand
- Refactoring logic that works but is difficult to maintain
- Reviewing code quality through readability and cohesion

## Do not use this skill when

- The main need is architecture, security, or deployment guidance instead
- The code is simple enough that extra abstraction would make it worse

## Instructions

1. Use intention-revealing names and clear boundaries.
2. Keep functions and components focused.
3. Prefer cleaner structure over explanatory comments when possible.
4. Respect the repository’s existing layering and folder conventions.
5. Remove duplication only when the resulting abstraction is clearer than the repeated code.

## Clean-code lenses

- naming
- function and method size
- responsibility boundaries
- comments versus expressive code
- duplication and complexity

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/clean-code
'@
  "code-refactoring-tech-debt" = @'
---
name: code-refactoring-tech-debt
description: Reduce technical debt in ResumeEnhancer through scoped refactoring that preserves behavior while improving structure, duplication, and maintainability. Use when Codex needs to clean up existing code, prepare for new features, or address architectural friction.
---

# Code Refactoring Tech Debt

Reduce technical debt in ResumeEnhancer through targeted, reviewable refactoring that preserves behavior and lowers future delivery cost.

## Use this skill when

- Refactoring for maintainability without changing intended behavior
- Reducing duplication or structural friction before new feature work
- Preparing a code area for safer future changes

## Do not use this skill when

- A full redesign is being mistaken for a refactor
- The change lacks tests or other verification anchors

## Instructions

1. Identify the concrete pain: duplication, unclear ownership, bloated handlers, brittle UI state, or persistence leakage.
2. Preserve behavior with focused tests before broad edits.
3. Refactor along existing boundaries instead of inventing new ones.
4. Keep the diff small enough to review safely.
5. Call out any remaining debt that should not be solved in the same change set.

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/code-refactoring-tech-debt
'@
  "production-code-reviewer" = @'
---
name: production-code-reviewer
description: Perform production-grade code review for ResumeEnhancer with a defect-first mindset across architecture, correctness, security, tests, and maintainability. Use when Codex needs to review diffs, pull requests, or uncommitted changes before merge.
---

# Production Code Reviewer

Transform code review from gatekeeping into high-signal defect detection, architectural alignment, and knowledge sharing for ResumeEnhancer.

## Use this skill when

- Reviewing pull requests or diffs in ResumeEnhancer
- Auditing for correctness, security, performance, or architecture drift
- Establishing review standards for this repository

## Do not use this skill when

- There are no code changes to inspect
- The task is design-only and contains no implementation
- The goal is to implement fixes rather than review

## Instructions

1. Read `AGENTS.md` and the relevant story or requirement.
2. Inspect the actual diff and enough surrounding code to understand the changed path.
3. Check whether the change respects module boundaries and existing patterns.
4. Validate behavior through tests, call sites, validators, mappers, and API contracts.
5. Report findings by severity, with concrete impact and narrow file references.
6. If you need more detailed patterns and examples, open:
   - `references/implementation-playbook.md`
   - `references/review-playbook.md`

## Review mindset

- catch bugs and regressions
- protect maintainability
- surface security and performance risks
- distinguish blocking issues from suggestions

## Output requirements

- findings first
- severity-ordered issues
- brief overall risk summary
- test and verification gaps

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/code-review-excellence
'@
  "deep-research" = @'
---
name: deep-research
description: Perform deep, evidence-based research for ResumeEnhancer across product requirements, user stories, code, tests, and external sources when needed. Use when Codex needs a thorough answer before implementation, architecture, or planning decisions.
---

# Deep Research

Perform broad, evidence-driven investigation before important ResumeEnhancer decisions, especially when repository context, business requirements, and external references all matter.

## Use this skill when

- The answer depends on multiple repository layers or current external evidence
- You need research before coding, planning, or architecture work
- The topic is cross-cutting and assumptions would be risky

## Do not use this skill when

- The task can be answered with a small local code read
- Current external data is not relevant

## Instructions

1. Start with local requirements, user stories, implementation, and tests.
2. Use external sources only when the question is current, specific, or user-requested.
3. Separate observed, inferred, and recommended conclusions.
4. Prefer concise synthesis over dumping raw evidence.

## Output requirements

- findings grouped by theme
- evidence summary
- open questions
- implications for implementation or planning

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/deep-research
'@
  "domain-driven-design" = @'
---
name: domain-driven-design
description: Apply domain-driven design thinking to ResumeEnhancer by clarifying business concepts, boundaries, invariants, and model responsibilities. Use when Codex needs to shape new domain behavior or review whether a design fits the business model cleanly.
---

# Domain Driven Design

Use DDD thinking pragmatically in ResumeEnhancer to clarify business language, domain boundaries, invariants, and tactical implementation choices.

## Use this skill when

- You need to model a complex business concept or boundary in ResumeEnhancer
- You want to decide whether deeper DDD practices are justified
- You need to connect strategic product language to tactical implementation

## Do not use this skill when

- The task is simple CRUD with low business complexity
- The work is a localized bug fix with no domain ambiguity

## Instructions

1. Run a viability check before committing to full DDD depth.
2. Produce strategic artifacts first: subdomains, bounded contexts, glossary.
3. Map strategic ideas to the current modular monolith structure.
4. If a deliverable checklist is needed, open `references/ddd-deliverables.md`.

## Output requirements

- scope and assumptions
- current DDD stage
- artifacts produced
- next-step recommendation

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/domain-driven-design
'@
  "dotnet-architect" = @'
---
name: dotnet-architect
description: Design and review ResumeEnhancer solutions as a .NET modular monolith with clean layering, Minimal APIs, Mediator, EF Core, and explicit composition. Use when Codex needs framework-aware architecture guidance for backend or cross-layer changes.
---

# Dotnet Architect

Provide .NET-specific architecture guidance for ResumeEnhancer using the actual stack in this repository: ASP.NET Core, Minimal APIs, Mediator, EF Core, explicit composition, and modular layering.

## Use this skill when

- A backend or cross-layer change needs .NET-specific architecture judgment
- You need guidance on ASP.NET Core, composition, EF Core, or clean layering
- You are reviewing whether a .NET solution fits the current design direction

## Do not use this skill when

- The task is not .NET or backend related
- A smaller implementation-focused skill is sufficient

## Instructions

1. Clarify goals, constraints, and impacted layers.
2. Apply repository-specific architecture rules before generic .NET advice.
3. Check dependency direction, composition boundaries, and persistence integration.
4. For deeper implementation patterns, open the local `dotnet-backend-patterns` references.

## Capabilities

- modular monolith design
- Minimal API and service-layer boundaries
- EF Core and repository integration
- testing and verification strategy
- performance and maintainability tradeoffs

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/dotnet-architect
'@
  "frontend-dev-guidelines" = @'
---
name: frontend-dev-guidelines
description: Apply ResumeEnhancer frontend development guidelines for React, TypeScript, Vite, feature boundaries, forms, data fetching, and UI states. Use when Codex needs project-consistent frontend implementation guidance.
---

# Frontend Development Guidelines

Apply project-consistent frontend implementation standards for ResumeEnhancer while learning from the upstream production-grade frontend guidance.

## Use this skill when

- Implementing frontend code in ResumeEnhancer
- You need house rules for feature structure, forms, routing, fetching, and UI states
- You are reviewing whether a frontend change fits the current client architecture

## Do not use this skill when

- The task is backend-only
- You only need visual design guidance without implementation standards

## Instructions

1. Prefer the ResumeEnhancer client architecture over the upstream stack assumptions.
2. Reuse existing `features`, `shared/ui`, `shared/api`, route, and model patterns.
3. Keep forms aligned with current schema, API models, and feature hooks.
4. For detailed adapted guidance, open:
   - `references/project-adaptation.md`
   - `references/file-organization.md`
   - `references/data-fetching.md`
   - `references/loading-and-error-states.md`
   - `references/component-patterns.md`

## Core rules

- preserve feature boundaries
- define loading, empty, error, and success states intentionally
- keep API interaction typed and centralized
- avoid introducing stack patterns that do not fit the current React Router + Vite setup

## Output requirements

- recommended file placement
- state and data flow pattern
- loading and error state plan
- type and form alignment notes

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-dev-guidelines
'@
  "frontend-design" = @'
---
name: frontend-design
description: Design polished, intentional interfaces for ResumeEnhancer that fit the product domain, existing implementation patterns, and accessibility expectations. Use when Codex needs to shape layout, hierarchy, copy, and interaction design before or during frontend implementation.
---

# Frontend Design

Design deliberate, product-quality user experiences for ResumeEnhancer rather than generic component assemblies.

## Use this skill when

- The task is primarily about product design quality and user experience
- Layout, hierarchy, copy, and interaction need deliberate design treatment

## Do not use this skill when

- The work is purely backend or data-layer oriented
- The request is only about implementation mechanics

## Instructions

1. Design for the ResumeEnhancer product context, not generic templates.
2. Keep accessibility and mobile behavior intentional.
3. Align design decisions with the existing client architecture so implementation stays practical.
4. Distinguish visual direction, interaction design, and implementation notes.

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-design
'@
  "frontend-developer" = @'
---
name: frontend-developer
description: Implement frontend features in ResumeEnhancer using the existing React, TypeScript, Vite, feature, route, hook, and API-client patterns. Use when Codex needs to build or modify product UI in a way that fits the current client architecture.
---

# Frontend Developer

Implement production-facing client work in ResumeEnhancer with strong attention to feature boundaries, typed models, interaction states, accessibility, and API integration.

## Use this skill when

- Building or modifying product-facing client code in ResumeEnhancer
- Implementing UI, hooks, routes, forms, or API integration in the current stack

## Do not use this skill when

- The work is backend architecture or persistence design
- The task is visual design only without implementation

## Instructions

1. Clarify the target page, feature, route, and interaction flow.
2. Reuse existing feature boundaries, shared UI, typed models, and API client patterns.
3. Implement accessibility, responsive behavior, and meaningful empty or error states.
4. Keep frontend state aligned with current schema and service models.
5. Prefer implementation choices that match the existing app shell and route structure.

## Output requirements

- feature placement
- route and state approach
- API integration notes
- verification checklist

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-developer
'@
  "frontend-security-coder" = @'
---
name: frontend-security-coder
description: Implement and review secure frontend behavior in ResumeEnhancer with attention to auth flow, data exposure, user input handling, and safe API interaction. Use when Codex is changing client-side behavior with security or privacy implications.
---

# Frontend Security Coder

Secure ResumeEnhancer client behavior around authentication, user-owned content, sensitive UI state, and safe interactions with backend APIs.

## Use this skill when

- Client work touches authentication, sensitive content, sharing, account state, or potentially unsafe rendering

## Do not use this skill when

- Security implications are negligible and ordinary frontend guidance is enough

## Instructions

1. Protect user data in the UI and client-side interaction flow.
2. Avoid unsafe rendering patterns and accidental data exposure.
3. Keep auth and permission assumptions aligned with server behavior.
4. Review copy, empty states, and errors for accidental information disclosure.

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-security-coder
'@
  "frontend-slides" = @'
---
name: frontend-slides
description: Create presentation-ready frontend walkthroughs, implementation summaries, and UI narratives for ResumeEnhancer. Use when Codex needs to turn frontend work into stakeholder-friendly slides, demos, or visual explanations.
---

# Frontend Slides

Turn ResumeEnhancer frontend work into stakeholder-friendly slide narratives, demos, and visual walkthroughs without losing technical accuracy.

## Use this skill when

- The output is a presentation, demo, or stakeholder-facing narrative about frontend work

## Do not use this skill when

- The main deliverable is production code rather than presentation material

## Instructions

1. Convert frontend work into clear flows, screenshots, or slide-ready talking points.
2. Keep the narrative grounded in actual product behavior and implementation status.
3. Separate shipped behavior, planned work, and open risks.

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/frontend-slides
'@
  "full-stack-feature-orchestrator" = @'
---
name: full-stack-feature-orchestrator
description: Coordinate full-stack feature delivery in ResumeEnhancer across requirements, frontend, API, service layer, persistence, tests, and PR packaging. Use when Codex needs to drive an end-to-end feature from story to review-ready implementation.
---

# Full Stack Feature Orchestrator

Coordinate end-to-end ResumeEnhancer feature delivery from requirement through frontend, backend, persistence, tests, rollout, and PR packaging.

## Use this skill when

- A ResumeEnhancer feature spans frontend, backend, and tests
- Coordinated end-to-end delivery matters more than isolated edits
- You need a plan that connects story intent to review-ready implementation

## Do not use this skill when

- The task is small and isolated to one layer
- The work does not need orchestration across multiple areas

## Instructions

1. Start from requirements, acceptance criteria, and constraints.
2. Plan the cross-layer path: frontend, API contracts, validators, handlers, persistence, tests, and PR packaging.
3. Identify dependencies, rollout considerations, and verification checkpoints early.
4. Keep the workflow proportional to the real scope of the feature.
5. Explicitly call out any shared-file, migration, or contract conflicts before parallel work starts.

## Output requirements

- phase plan
- impacted layers
- dependency and risk summary
- verification and rollout plan

## Upstream URL
- https://github.com/benjaminasterA/antigravity-awesome-skills/tree/main/skills/full-stack-orchestration-full-stack-feature
'@
}

foreach ($name in $skillBodies.Keys) {
  $path = Join-Path $PSScriptRoot "..\\skills\\$name\\SKILL.md"
  Set-Content -LiteralPath ([IO.Path]::GetFullPath($path)) -Value $skillBodies[$name] -NoNewline
}
