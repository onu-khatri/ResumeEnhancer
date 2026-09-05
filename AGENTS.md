# ResumeEnhancer Agent Instructions

These instructions apply to the entire repository unless a more specific `AGENTS.md` overrides them in a nested subtree.

## Purpose

This repository is a modular resume platform built as a .NET modular monolith with a React/Vite client. Agents working here should preserve the current architectural direction, prefer evidence over assumption, and keep cross-layer changes traceable.

## Primary Architecture Rules

- Treat `application/WebSolution/ModulesComposition` as the host-facing module composition boundary.
- Do not bypass module boundaries by wiring the host directly to module internals when an existing composition path already exists.
- Keep HTTP concerns in `ResumeEnhancer.ResumeModule.Web`.
- Keep request/response contracts in `ResumeEnhancer.ResumeModule.AM`.
- Keep use-case orchestration, Mediator contracts, handlers, and mapping workflow logic in `ResumeEnhancer.ResumeModule.SL`.
- Keep domain entities and domain-only concepts in `ResumeEnhancer.ResumeModule.DM`.
- Keep EF configuration, repository adapters, and schema-specific persistence behavior in `ResumeEnhancer.ResumeModule.PL`.
- Keep shared infrastructure behavior inside `application/Infrastructure`.
- Keep frontend feature logic inside `application/WebSolution/websolution.client/src/features`.

## Working Style

- Read the relevant README, user stories, business requirements, and implementation files before changing behavior.
- Check `KnowledgeBase/INDEX.md` before planning, implementing, reviewing, or documenting work; then read only the linked knowledge topics that apply.
- Follow existing naming, folder, and dependency patterns before introducing new structure.
- Prefer extending current flows over inventing parallel abstractions.
- Keep changes scoped to the user request.
- Make business intent traceable when possible by connecting code changes back to existing requirement artifacts.
- Before asking the user an interview or optional clarification question, use the host's structured question UI as the first choice when available. Use one-at-a-time chat questions only when it is unavailable.

## Skills And Agents

- Discover available repository skills under `.codex/skills/<skill-name>/SKILL.md` and custom agents under `.codex/agents/*.toml` before choosing a workflow. Read a selected skill's `SKILL.md` before acting; read only its routed references that apply.
- Use `AGENTS.md` for durable repository-wide constraints, a skill for reusable task guidance, and a custom agent for a focused delegated workstream. A delegated agent does not replace the main agent's repository inspection or ownership of the final result.
- Keep these mechanisms separate: skill UI metadata is `.codex/skills/<skill-name>/agents/openai.yaml`; OpenSpec workflows live in `.agents/skills/`; custom-agent definitions live in `.codex/agents/`.

### Skill Selection

- Backend delivery: `$backend-feature-development`, `$dotnet-backend-patterns`, `$ef-core-database-architect`, `$backend-security-coder`, and `$performance-optimization` as the change requires.
- Architecture and domain: `$architect-review`, `$dotnet-architect`, `$domain-driven-design`, and `$architecture-decision-records`.
- Frontend delivery: `$frontend-developer`, `$frontend-dev-guidelines`, `$react-patterns`, `$frontend-design`, `$production-ui-generator`, and `$frontend-security-coder` as applicable.
- Full-stack and story delivery: `$full-stack-feature-orchestrator`, `$us-kickoff` for readiness and GitHub handoff, `$issues-kickoff` for implementation execution, `$plan-writing`, and the OpenSpec skills in `.agents/skills/` when the user explicitly requests that workflow.
- Review, quality, and security: `$production-code-reviewer`, `$ai-code-review`, `$code-review-checklist`, `$clean-code`, `$code-refactoring-tech-debt`, `$security-manager`, and `$design-review` as relevant.
- Research and durable guidance: `$deep-research`, `$project-knowledge-builder`, `$documentation-generator`, `$readme-generator`, `$prd-manager`, and `$user-story-creator`. Use `$project-knowledge-builder` for reusable `KnowledgeBase/` artifacts, not ordinary documentation.
- Delivery operations: `$git-workflows`, `$git-worktrees`, `$git-commit`, and `$pr-creator` when managing branches, commits, worktrees, or pull requests.
- Use `$user-interview` when a selected workflow requires user-confirmed material decisions; do not silently replace a required interview with assumptions.

### Frontend Operating Model

- Use [frontend workflow routing](.codex/skills/frontend-dev-guidelines/references/frontend-workflow-routing.md) to select frontend skills and the `frontend-implementer` agent.
- Resolve material uncertainty before frontend delivery: use `$deep-research` for significant evidence gaps and `$user-interview` for a material user decision that evidence cannot answer.
- Assign one primary implementation owner for a workstream. `$frontend-developer` is the default for frontend code; `$production-ui-generator` is an alternative only for an explicitly UI-dominant surface, not a parallel implementer.
- Treat `$frontend-dev-guidelines` as the non-delegating standards authority. Use design, React-pattern, security, performance, review, and slides skills only when their stated trigger applies.
- Specialist skills return constraints, findings, or a handoff to the primary owner. They do not recursively invoke other frontend skills, spawn the `frontend-implementer`, or duplicate implementation work.

### Custom Agent Selection

- `backend-implementer`: assigned backend slices across Minimal APIs, validation, Mediator, persistence, and tests.
- `frontend-implementer`: assigned React/TypeScript feature slices, routes, forms, and typed API integration.
- `knowledge-researcher`: evidence gathering and approval-driven reusable knowledge work; it does not implement product code.
- `security-auditor`: OWASP-oriented review of trust boundaries, authorization, validation, data exposure, and abuse resistance.
- `code-reviewer`: defect-first review of real diffs across correctness, security, architecture, and tests.
- `story-orchestrator`: approved multi-story dependency ordering, parallel-work planning, and review-ready handoff.

Implementation agents receive their branch or worktree from `$issues-kickoff`. Use `$git-worktrees` for creating or cleaning isolated workspaces, `$git-commit` for staging/commit/push decisions, and `$git-workflows` only for branch synchronization, history rewriting, cherry-picking, bisect, or recovery. Do not let multiple agents independently mutate shared contracts, migrations, composition, or shared UI primitives.

For parallel work, assign explicit ownership before editing shared contracts, migrations, composition, or shared UI primitives. Keep cross-layer contract changes in one coordinating lane, and use agents only when their role materially improves the task.

## Verification Expectations

- For code changes, run the smallest meaningful tests for the touched area, then state clearly what was and was not verified.
- For documentation, prompt, skill, or agent changes, verify structure and internal references instead of claiming runtime behavior you did not test.
- Do not claim a skill or custom agent is auto-discovered in the current session unless you actually observed it in a fresh Codex task.

## Commands

- Full solution build: `dotnet build application\ResumeEnhancerApp.slnx`
- Unit tests: `dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore`
- Integration tests: `dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore`
- API host: `dotnet run --project application\WebSolution\WebSolution.Server\ResumeEnhancer.WebSolution.Server.csproj`
- Migration help: `dotnet run --project application\Infrastructure\Migration\ResumeEnhancer.Infrastructure.Migration.csproj -- --help`

## Checks Required For Instruction-Oriented Changes

When changing `AGENTS.md`, `.codex/`, or `Prompts/`:

- review the edited files for broken paths, broken cross-references, and contradictory guidance
- run `Get-ChildItem -Recurse .codex, Prompts`
- run `git diff -- AGENTS.md .codex Prompts`

## Knowledge Artifact Location

- Save durable project knowledge under `KnowledgeBase/` unless the user explicitly requests another location.



