$skillBodies = @{
  "delivery-pull-request" = @'
---
name: delivery-pull-request
description: Create high-quality pull requests for ResumeEnhancer with branch safety, story traceability, reviewer context, and clear validation notes. Use when Codex needs to prepare a branch, stage work, summarize code changes, link user stories or business requirements, and produce a reviewer-ready PR flow.
---

# PR Creator

Use this skill to finish implementation work safely and package it for review with the right branch, story references, architectural context, and verification notes.

## Workflow

1. Read `AGENTS.md`, the relevant user story, and any touched business requirements.
2. Check the current branch. If it is `main`, use the approved workflow to create the canonical `openspec/` branch for the issue or OpenSpec change.
3. Review the diff before staging so the PR summary reflects the real change.
4. Group the PR around one coherent scope. If the diff mixes unrelated work, separate it before creating the PR.
5. Summarize the change using project language: frontend, web boundary, service layer, persistence, tests, and story traceability.
6. Include user story IDs and requirement references when they exist.
7. Record what was verified and what remains unverified.

## ResumeEnhancer Rules

- Mention module boundaries explicitly when they matter.
- Call out changes to `ResumeModuleWeb`, `ResumeModelSL`, `ResumeModulePL`, and frontend `features/resume` separately when touched.
- Mention schema or migration impact whenever persistence changes.
- Mention validation, mapping, and test coverage when relevant.
'@
  "delivery-user-story-kickoff" = @'
---
name: delivery-user-story-kickoff
description: Prepare approved ResumeEnhancer user stories for GitHub issue handoff with readiness checks, dependency ordering, and human approval. Use delivery-issues-kickoff for implementation execution.
---

# US Kickoff

Use this skill to validate approved story slices and hand them to GitHub through `$delivery-github-issue` without losing readiness discipline or architectural boundaries. It does not create branches, worktrees, or implementation agents.

## Entry Criteria

- target stories are identified
- each story is `Ready_To_Implement` or equivalent approved state
- dependencies and sequence are known
- the user has confirmed the issue handoff after a short summary of the plan

## Workflow

1. Read the selected story files and any linked business requirements.
2. Resolve dependencies and determine issue pick order and any evidence-backed splits.
3. Identify shared files, migration risk, and likely merge conflicts.
4. Stop for a short human approval checkpoint before issue creation.
5. After approval, invoke `$delivery-github-issue` for the approved stories in dependency order.
6. After verified handoff, use `$delivery-issues-kickoff` for branches, worktrees, agents, implementation, and PR readiness.
'@
  "frontend-production-ui" = @'
---
name: frontend-production-ui
description: Generate production-grade UI for ResumeEnhancer with intentional design, accessibility, component reuse, and implementation-ready React patterns. Use when Codex needs to create or improve pages, flows, feature UIs, or polished product surfaces in the React/Vite client.
---

# Production UI Generator

Use this skill for UI work that must feel launch-ready, not demo-grade.

## Workflow

1. Read the related user story and current page or component context.
2. Reuse existing `shared/ui`, feature hooks, and model types before creating new primitives.
3. Define states first: loading, empty, error, success, and permission or entitlement states when relevant.
4. Build mobile and desktop behavior intentionally.
5. Keep styling purposeful and specific.
'@
  "quality-production-code-review" = @'
---
name: quality-production-code-review
description: Perform production-grade code review for ResumeEnhancer with a defect-first mindset across architecture, correctness, security, tests, and maintainability. Use when Codex needs to review diffs, pull requests, or uncommitted changes before merge.
---

# Production Code Reviewer

Use this skill for real review work, not style-only commentary.

## Review Order

1. Read `AGENTS.md` and the relevant story or requirement.
2. Inspect the actual diff and enough surrounding code to understand the changed path.
3. Check whether the change respects module boundaries and existing patterns.
4. Validate behavior through tests, call sites, validators, mappers, and API contracts.
5. Report findings by severity, with concrete impact and narrow file references.
'@
  "documentation-professional-writing" = @'
---
name: documentation-professional-writing
description: Write professional long-form content grounded in ResumeEnhancer product thinking, implementation detail, and evidence. Use when Codex needs to produce articles, technical explainers, internal writeups, or polished thought pieces from project material.
---

# Professional Article Writer

Use this skill for polished writing that should sound informed, concrete, and publication-ready.

## Focus

- anchor claims in repository evidence or explicit source material
- connect product goals to technical execution
- avoid hype and filler
- tailor tone to internal docs, blog posts, launch notes, or engineering explainers
'@
  "backend-ef-core" = @'
---
name: backend-ef-core
description: Design and review ResumeEnhancer database and persistence changes using EF Core, SQL Server, shared persistence infrastructure, and migration discipline. Use when Codex needs to shape schemas, repositories, seeding, mappings, or migration strategy for this project.
---

# EF Core Database Architect

Use this skill for persistence design in the actual stack this repository uses.

## Focus

- EF Core configuration and schema mapping
- migration safety
- repository and unit-of-work behavior
- setup data and seeding
- query shape, includes, and pagination
'@
  "security-management" = @'
---
name: security-management
description: Apply OWASP-oriented secure design and secure coding guidance to ResumeEnhancer across frontend, backend, authentication, persistence, and AI-adjacent flows. Use when Codex needs to assess, implement, or review security-sensitive changes in this project.
---

# Security Manager

Use this skill when a task touches authorization, data protection, input validation, file handling, privacy, secrets, payment-adjacent flows, or any feature that could introduce abuse or trust issues.

## Security Review Lens

- authentication and authorization
- input validation and output encoding
- data exposure and privacy
- persistence safety and injection risk
- logging and secret handling
- rate limiting and abuse controls
'@
  "documentation-excalidraw-diagrams" = @'
---
name: documentation-excalidraw-diagrams
description: Generate clear architecture and workflow diagrams for ResumeEnhancer that can be rendered in Excalidraw or translated into diagram assets later. Use when Codex needs to visualize modules, flows, integrations, or story delivery plans.
---

# Excalidraw Diagram Generator

Use this skill to turn architecture or workflow understanding into diagram-ready structure.

## Output

- nodes with concise labels
- grouped boundaries
- directional flows
- notes on what is observed versus inferred
'@
  "orchestration-agent-improvement" = @'
---
name: orchestration-agent-improvement
description: Improve how Codex decomposes and coordinates multi-agent work for ResumeEnhancer, especially for parallel story execution, research, review, and implementation. Use when a task is large enough to benefit from structured delegation and synthesis.
---

# Agent Orchestration Improver

Use this skill when one agent should plan, delegate, gather evidence, and synthesize results instead of doing everything serially.

## Focus

- clear subtask boundaries
- minimal context leakage
- parallelism only where file overlap is low
- explicit synthesis responsibilities for the parent agent
'@
  "architecture-adr" = @'
---
name: architecture-adr
description: Write clear architecture decision records for ResumeEnhancer that capture context, options, tradeoffs, and consequences in project-specific language. Use when Codex needs to document a meaningful technical decision or proposed architecture change.
---

# Architecture Decision Records

Use this skill when a decision deserves durable documentation because it affects module boundaries, persistence strategy, feature orchestration, testing strategy, security posture, or delivery sequencing.
'@
  "architecture-review" = @'
---
name: architecture-review
description: Review modular and distributed architecture-sensitive changes for structural risk, quality attributes, and evidence-backed corrective direction. Use when design-level review is needed beyond local correctness.
---

# Architect Review

Read `KnowledgeBase/INDEX.md`, then `architecture-review.knowledge.md` before producing findings. Use distributed review only for remote boundaries, asynchronous messaging, independent deployment, eventual consistency, resilience, or distributed observability; then read `distributed-architecture-review.knowledge.md`.

Route domain, security, performance, research, and ADR work to their specialist skills only when their trigger applies. Report Architecture Impact as High, Medium, or Low with affected quality attributes, then evidence-backed findings, residual risks, and verification gaps.
'@
  "backend-feature-development" = @'
---
name: backend-feature-development
description: Implement backend features in ResumeEnhancer using the existing .NET modular architecture, Minimal APIs, FluentValidation, Mediator handlers, Mapster, repositories, and tests. Use when Codex needs to add or modify backend behavior in a project-consistent way.
---

# Backend Feature Development

Use this skill to add or change backend behavior without breaking the repo’s layering and contract patterns.
'@
  "backend-security" = @'
---
name: backend-security
description: Implement and review secure backend code for ResumeEnhancer with OWASP-oriented practices around validation, authorization, data handling, and abuse resistance. Use when Codex is changing APIs, persistence flows, auth-sensitive logic, or other backend attack surfaces.
---

# Backend Security Coder

Use this skill when backend work must be correct and secure, especially around account state, document sharing, AI usage, billing, exports, or admin behavior.
'@
  "quality-clean-code" = @'
---
name: quality-clean-code
description: Keep ResumeEnhancer changes readable, cohesive, and easy to extend without introducing unnecessary abstractions or duplication. Use when Codex is implementing, refactoring, or reviewing code quality across backend or frontend areas.
---

# Clean Code

Use this skill to improve clarity and maintainability while respecting the repository’s existing architecture instead of fighting it.
'@
  "quality-refactoring" = @'
---
name: quality-refactoring
description: Reduce technical debt in ResumeEnhancer through scoped refactoring that preserves behavior while improving structure, duplication, and maintainability. Use when Codex needs to clean up existing code, prepare for new features, or address architectural friction.
---

# Code Refactoring Tech Debt

Use this skill when the goal is structural improvement with low behavioral risk.
'@
  "research-deep" = @'
---
name: research-deep
description: Perform deep, evidence-based research for ResumeEnhancer across product requirements, user stories, code, tests, and external sources when needed. Use when Codex needs a thorough answer before implementation, architecture, or planning decisions.
---

# Deep Research

Use this skill for broad investigations where the answer depends on multiple repository layers or on current external evidence.
'@
  "architecture-domain-modeling" = @'
---
name: architecture-domain-modeling
description: Apply pragmatic domain modeling to clarify business language, context boundaries, invariants, and model responsibilities. Use when business complexity materially affects architecture or behavior.
---

# Domain Driven Design

Read `KnowledgeBase/INDEX.md`, then `domain-modeling.knowledge.md` before proposing domain-modeling patterns. Retrieve modular architecture when dependencies, composition, or integration change, and project routing for local adaptation.

Stop after viability assessment when no meaningful invariant, divergent context, or lifecycle rule exists.
'@
  "backend-dotnet-architecture" = @'
---
name: backend-dotnet-architecture
description: Design .NET backend and modular-application architecture with explicit ownership, dependency, composition, integration, and verification decisions. Use when a change needs architecture judgment before implementation.
---

# Dotnet Architect

Read `KnowledgeBase/INDEX.md`, then `dotnet-modular-architecture.knowledge.md` before selecting a boundary, dependency, composition, or integration pattern. Retrieve domain-modeling and project-routing knowledge only when applicable.

State ownership, dependency direction, verification implications, and an ADR recommendation for durable decisions.
'@
  "backend-dotnet-patterns" = @'
---
name: backend-dotnet-patterns
description: Apply .NET backend implementation patterns for API boundaries, application behavior, persistence, and testing. Use when a change needs pattern selection or a framework-aware implementation review.
---

# Dotnet Backend Patterns

Use this skill to select and apply the smallest proven .NET backend pattern that fits the change and the target project.

Check `KnowledgeBase/INDEX.md`, then retrieve only the API/application, EF Core persistence, and project-adaptation topics that affect the task.
'@
  "frontend-guidelines" = @'
---
name: frontend-guidelines
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

Use this skill when the task is primarily about product design quality and user experience, not just component coding.
'@
  "frontend-development" = @'
---
name: frontend-development
description: Deliver production-ready ResumeEnhancer React features with correct routes, typed client data, accessible responsive behavior, and focused verification. Use when building or changing product UI, hooks, forms, or client integration.
---

# Frontend Developer

Use this skill to turn approved frontend behavior into implementation-ready code that fits the current React/Vite client. Read `$frontend-guidelines` for detailed shared implementation standards, preserve the typed shared API client, implement complete user-visible states, and report focused verification precisely.
'@
  "frontend-security" = @'
---
name: frontend-security
description: Implement and review secure frontend behavior in ResumeEnhancer with attention to auth flow, data exposure, user input handling, and safe API interaction. Use when Codex is changing client-side behavior with security or privacy implications.
---

# Frontend Security Coder

Use this skill when client work touches authentication, sensitive content, sharing, account state, or potentially unsafe rendering paths.
'@
  "frontend-presentations" = @'
---
name: frontend-presentations
description: Create presentation-ready frontend walkthroughs, implementation summaries, and UI narratives for ResumeEnhancer. Use when Codex needs to turn frontend work into stakeholder-friendly slides, demos, or visual explanations.
---

# Frontend Slides

Use this skill when the output is a presentation artifact or demo narrative rather than production code.
'@
  "delivery-full-stack-feature" = @'
---
name: delivery-full-stack-feature
description: Coordinate full-stack feature delivery in ResumeEnhancer across requirements, frontend, API, service layer, persistence, tests, and PR packaging. Use when Codex needs to drive an end-to-end feature from story to review-ready implementation.
---

# Full Stack Feature Orchestrator

Use this skill when a feature spans frontend, backend, and tests and needs coordinated execution instead of isolated edits.
'@
}

foreach ($name in $skillBodies.Keys) {
  $path = Join-Path $PSScriptRoot "..\\skills\\$name\\SKILL.md"
  $resolved = [System.IO.Path]::GetFullPath($path)
  Set-Content -LiteralPath $resolved -Value $skillBodies[$name] -NoNewline
}

$agentsDir = Join-Path $PSScriptRoot "..\\agents"
New-Item -ItemType Directory -Path $agentsDir -Force | Out-Null

$agentBodies = @{
  "code-reviewer.toml" = @'
name = "code-reviewer"
description = "Defect-first reviewer for ResumeEnhancer backend, frontend, and architecture-sensitive changes."
developer_instructions = """
Read AGENTS.md first. Review diffs, not intentions. Prioritize correctness, security, architecture drift, missing tests, and regression risk. Return severity-ordered findings with concrete file references and short rationale.
"""
model = "gpt-5"
model_reasoning_effort = "medium"
sandbox_mode = "workspace-write"
'@
  "backend-implementer.toml" = @'
name = "backend-implementer"
description = "Focused ResumeEnhancer backend implementer for Minimal APIs, handlers, persistence, and tests."
developer_instructions = """
Read AGENTS.md first. Follow ResumeEnhancer layering strictly: Web for validation and HTTP, SL for orchestration, PL for EF and repositories, DM for domain entities. Prefer focused diffs and verification notes.
"""
model = "gpt-5"
model_reasoning_effort = "medium"
sandbox_mode = "workspace-write"
'@
  "frontend-implementer.toml" = @'
name = "frontend-implementer"
description = "Single-lane ResumeEnhancer frontend implementer for React/TypeScript features, forms, routes, typed client data, and focused verification."
developer_instructions = """
Read AGENTS.md first, then `.codex/skills/frontend-guidelines/references/frontend-workflow-routing.md`. You are a single implementation lane: do not create subagents, delegate, invoke another frontend agent, or coordinate parallel work.

Load `$frontend-development` and `$frontend-guidelines` for assigned implementation. Consult a specialist only when the routing trigger applies, incorporate its result, and remain the sole implementer. Do not run `$research-deep` or `$workflow-user-interview`; return a material evidence gap or user decision to the parent with the exact question and evidence checked.

Work inside feature boundaries, reuse shared UI and model types, keep API interaction typed and centralized, implement meaningful user-visible states, preserve accessible responsive behavior, add focused tests, and report exactly which checks ran. If a requirement or contract is materially missing, return the blocker to the parent instead of inventing it.
"""
model = "gpt-5"
model_reasoning_effort = "medium"
sandbox_mode = "workspace-write"
'@
  "story-orchestrator.toml" = @'
name = "story-orchestrator"
description = "Coordinator for multi-story ResumeEnhancer delivery, sequencing, and parallel execution planning."
developer_instructions = """
Read AGENTS.md first. Group stories by dependency, identify conflict risk, require a human approval checkpoint before parallel execution, and keep cross-layer contract changes visible.
"""
model = "gpt-5"
model_reasoning_effort = "medium"
sandbox_mode = "workspace-write"
'@
  "security-auditor.toml" = @'
name = "security-auditor"
description = "OWASP-oriented security reviewer for ResumeEnhancer features, APIs, and sensitive flows."
developer_instructions = """
Read AGENTS.md first. Focus on auth, authorization, input validation, data exposure, token safety, secrets, logging, and abuse resistance. Ground claims in code evidence and requirements.
"""
model = "gpt-5"
model_reasoning_effort = "medium"
sandbox_mode = "workspace-write"
'@
}

foreach ($file in $agentBodies.Keys) {
  $path = Join-Path $agentsDir $file
  Set-Content -LiteralPath $path -Value $agentBodies[$file] -NoNewline
}

$promptDir = Join-Path $PSScriptRoot "..\\..\\Prompts"
Set-Content -LiteralPath (Join-Path $promptDir "PR_Creation_Starter.prompt.md") -Value @'
Use `$delivery-pull-request` to prepare a ResumeEnhancer pull request from the current changes.

- If the current branch is `main`, create a short branch name with a timestamp first.
- Summarize the change in reviewer-friendly prose.
- Include any matching user story IDs and requirement references.
- State exactly what was verified and what still needs verification.
'@ -NoNewline

Set-Content -LiteralPath (Join-Path $promptDir "US_Kickoff_Starter.prompt.md") -Value @'
Use `$delivery-user-story-kickoff` to evaluate the selected ResumeEnhancer user stories and prepare an approved GitHub issue handoff plan. After verified handoff, stop; the user must explicitly invoke `$delivery-issues-kickoff` later for top-10 issue intake and implementation.

- Only continue if the stories are ready to implement.
- Group by frontend, backend, full-stack, research, or architecture work.
- Stop for a short human approval checkpoint before parallel execution.
- Recommend branch or worktree isolation and suitable custom agents for each story.
'@ -NoNewline
