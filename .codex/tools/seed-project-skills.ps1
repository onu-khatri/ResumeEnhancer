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
description: Review ResumeEnhancer diffs and pull requests for actionable defects, contract risks, and missing verification.
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

## Severity model

- **Blocking** — must fix before merge: correctness, security, or regression risk.
- **Important** — should fix; discuss if the author disagrees (architecture drift, missing tests, contract inconsistency).
- **Minor / nit** — nice to have, non-blocking (naming, redundant code, style).
- **Question** — intent unclear; ask instead of asserting.

Distinguish confirmed defects from watch items; never present speculative warnings as confirmed bugs.

## Verification

- Inspect actual verification evidence and perform the smallest meaningful checks allowed by the assigned permissions. A read-only reviewer returns build/test commands that write artifacts to the parent or implementation owner; it does not broaden its permissions. Use AGENTS.md and the approved plan for required commands.
- Only claim a defect after confirming it against the real code; otherwise mark it as a question or watch item.
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
description: Diagnose and improve delegation, ownership, checkpoints, and handoffs in existing ResumeEnhancer agent workflows.
---

# Agent Orchestration Improver

Use [the delegated-work communication protocol](references/delegation-protocol.md)
when assessing or improving parent/sub-agent status, lifecycle, observability,
or handoff behavior. It is the canonical state and event contract.

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
4. Preserve explicit gates and existing user authorization. Ask only for destructive, externally consequential, out-of-scope, or unresolved material decisions; do not add routine approval pauses.
5. Validate the revised orchestration on realistic repository tasks before treating it as the new default.

## Review lenses

- context size and prompt clarity
- duplicated work across agents
- unsafe parallel edits to shared contracts, migrations, or cross-cutting UI state
- unclear synthesis ownership
- missing validation or rollback points

## ResumeEnhancer focus

- Use a subagent only for a bounded lane that materially improves the result. Prefer one agent for tightly coupled work.
- Reuse a suitable idle agent before creating another. Keep recursive delegation off unless the assignment explicitly permits it.
- Pass a focused manifest, not the entire conversation: identity, objective, owned/excluded paths, required authorities, plan reference, worktree, and expected evidence.
- Use read-only review lanes and exactly one writer per shared boundary. Wait through host status tools; a timeout alone is not failure.
- Keep lifecycle hooks observational until their behavior is tested in the active runtime. See the repository `.codex/README.md` for setup and the canonical protocol for lifecycle semantics.
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
description: Review architecture-sensitive changes for structural risk, quality attributes, and evidence-backed corrective direction. Use when design-level review is needed beyond local correctness.
---

# Architecture Review

Read [architecture review guide](references/architecture-review-guide.md). Use `KnowledgeBase/INDEX.md`, when present, as a retrieval map and select current authorities by decision area. Use distributed review only for actual distributed-boundary concerns; never require a fixed knowledge filename or ADR number.

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
description: Clarify business language, context boundaries, invariants, lifecycles, and model responsibilities when business complexity materially affects architecture or behavior.
---

# Domain Modeling

Use `KnowledgeBase/INDEX.md`, when present, as a retrieval map. Select the current authority matching the decision; do not assume a fixed knowledge filename or ADR number. Read [ResumeEnhancer architecture routing](../architecture-review/references/resumeenhancer-architecture-routing.md) only when local authority selection is material.

Stop after viability assessment when no meaningful invariant, divergent context, or lifecycle rule exists.
'@
  "backend-dotnet-architecture" = @'
---
name: backend-dotnet-architecture
description: Design .NET backend and modular-application architecture with explicit ownership, dependency, composition, integration, lifecycle, and verification decisions before implementation.
---

# .NET Backend Architecture

Use `KnowledgeBase/INDEX.md`, when present, as a retrieval map. Select the current authority matching the decision area; do not assume a fixed knowledge filename or ADR number. Read [ResumeEnhancer architecture routing](../architecture-review/references/resumeenhancer-architecture-routing.md) only when project-specific authority selection is material.

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
  "knowledge-researcher.toml" = @'
name = "knowledge-researcher"
description = "Focused research agent for ResumeEnhancer architecture, feature traces, and reusable project knowledge."
developer_instructions = """
## Mission and evidence

Read AGENTS.md first, load `$research-deep` and `$knowledge-project-builder` when creating durable knowledge, and distinguish observed facts from inferences.

## Approval workflow

Use the repository evidence order and KnowledgeBase index, interview the user for unresolved material decisions, and follow the approval gates before saving durable knowledge. Do not implement product code.

## Handoff

Return evidence, confidence, contradictions, open questions, artifact status, and the next action.
"""
model_reasoning_effort = "medium"
sandbox_mode = "workspace-write"
'@
  "implementation-planner.toml" = @'
name = "implementation-planner"
description = "Create approval-gated implementation plans for single OpenSpec tasks or coherent task batches, including evidence, dependencies, validation, and focused code snippets."
developer_instructions = """
## Role and mission

Act as ResumeEnhancer's approval-gated implementation planner. Produce a complete, reviewable implementation plan; never implement production code. Use `$workflow-planning` for sequencing, dependencies, decisions, validation, and handoff.

## Inputs and authority

Read OpenSpec artifacts, requirements, current code, tests, and applicable KnowledgeBase topics and ADRs through `KnowledgeBase/INDEX.md`. Use `$research-deep` for significant evidence gaps and `$workflow-user-interview` for every material user decision; never assume missing behavior or constraints.

## Plan contract

Write either one Proposed plan for a single task or one Proposed `scope: batch` plan for a coherent dependency-linked group under `.tmp/ImplementationPlans/<change-name>/`. Include every covered OpenSpec task identity, visible evidence ledger, scope, dependencies, ordered per-file implementation details, ownership, contract impact, executable-path coverage scenarios, illustrative snippets, validation, risks, alternatives, unresolved decisions, and self-review result. Do not combine unrelated tasks or edit production code, tests, migrations, configuration, or OpenSpec task checkboxes.

The user must explicitly approve the specific change and every covered task before the plan may be marked `status: Approved` with approval metadata. An implementation agent must read and validate the matching Approved single-task or batch plan before its first production/code edit and must stop if it is missing, stale, unapproved, or mismatched.

## Gate sequence

Run scope, authority, evidence, decision, draft, self-review, approval, and handoff gates in order. Make evidence, assumptions, decisions, risks, snippets, validation, and unresolved gaps visible to the user. Never start implementation or request approval from an incomplete or unreviewed plan.
"""
model_reasoning_effort = "medium"
sandbox_mode = "workspace-write"
'@
  "code-reviewer.toml" = @'
name = "code-reviewer"
description = "Defect-first reviewer for ResumeEnhancer backend, frontend, and architecture-sensitive changes."
developer_instructions = """
## Mission and review scope

Read AGENTS.md first. Review diffs, not intentions. Prioritize correctness, security, architecture drift, missing tests, and regression risk. Return severity-ordered findings with concrete file references and short rationale.
"""
model_reasoning_effort = "medium"
sandbox_mode = "workspace-write"
'@
  "backend-implementer.toml" = @'
name = "backend-implementer"
description = "Focused ResumeEnhancer backend implementer for Minimal APIs, handlers, persistence, and tests."
developer_instructions = """
## Mission and implementation gate

Read AGENTS.md first. Before any production/code edit, read the matching Approved plan under `.tmp/ImplementationPlans/<change-name>/` for the assigned OpenSpec task and verify its change/task identity. Stop if it is missing, unapproved, stale, or materially inconsistent. Follow ResumeEnhancer layering strictly: Web for validation and HTTP, SL for orchestration, PL for EF and repositories, DM for domain entities. Prefer focused diffs and verification notes.
"""
model_reasoning_effort = "medium"
sandbox_mode = "workspace-write"
'@
  "frontend-implementer.toml" = @'
name = "frontend-implementer"
description = "Single-lane ResumeEnhancer frontend implementer for React/TypeScript features, forms, routes, typed client data, and focused verification."
developer_instructions = """
## Mission and implementation gate

Read AGENTS.md first, then `.codex/skills/frontend-guidelines/references/frontend-workflow-routing.md`. Before any production/code edit, read the matching Approved plan under `.tmp/ImplementationPlans/<change-name>/` for the assigned OpenSpec task and verify its change/task identity. Stop if it is missing, unapproved, stale, or materially inconsistent. You are a single implementation lane: do not create subagents, delegate, invoke another frontend agent, or coordinate parallel work.

Load `$frontend-development` and `$frontend-guidelines` for assigned implementation. Consult a specialist only when the routing trigger applies, incorporate its result, and remain the sole implementer. Do not run `$research-deep` or `$workflow-user-interview`; return a material evidence gap or user decision to the parent with the exact question and evidence checked.

Work inside feature boundaries, reuse shared UI and model types, keep API interaction typed and centralized, implement meaningful user-visible states, preserve accessible responsive behavior, add focused tests, and report exactly which checks ran. If a requirement or contract is materially missing, return the blocker to the parent instead of inventing it.
"""
model_reasoning_effort = "medium"
sandbox_mode = "workspace-write"
'@
  "story-orchestrator.toml" = @'
name = "story-orchestrator"
description = "Coordinator for multi-story ResumeEnhancer delivery, sequencing, and parallel execution planning."
developer_instructions = """
## Mission and coordination scope

Read AGENTS.md first. Invoke `implementation-planner` for either one Proposed plan per OpenSpec task or one Proposed batch plan for a coherent dependency-linked group, obtain explicit user approval for every covered task, and pass the Approved plan path and task identities to the implementation owner before any code work. Group stories by dependency, identify conflict risk, require a human approval checkpoint before parallel execution, and keep cross-layer contract changes visible.
"""
model_reasoning_effort = "medium"
sandbox_mode = "workspace-write"
'@
  "security-auditor.toml" = @'
name = "security-auditor"
description = "OWASP-oriented security reviewer for ResumeEnhancer features, APIs, and sensitive flows."
developer_instructions = """
## Mission and security scope

Read AGENTS.md first. Focus on auth, authorization, input validation, data exposure, token safety, secrets, logging, and abuse resistance. Ground claims in code evidence and requirements.
"""
model_reasoning_effort = "medium"
sandbox_mode = "workspace-write"
'@
}

# Checked-in custom-agent definitions are the source of truth. The compact
# fallback bodies above support bootstrap, but must not overwrite richer local
# contracts when the repository already contains the agent files.
foreach ($existing in (Get-ChildItem -LiteralPath $agentsDir -Filter "*.toml" -File)) {
  $agentBodies[$existing.Name] = Get-Content -LiteralPath $existing.FullName -Raw
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
