# ResumeEnhancer Agent Instructions

These instructions apply to the entire repository unless a more specific `AGENTS.md` overrides them in a nested subtree.

## Purpose

This repository is a modular resume platform built as a .NET modular monolith with a React/Vite client. Agents working here should preserve the current architectural direction, prefer evidence over assumption, and keep cross-layer changes traceable.

## Primary Architecture Rules

- Treat `application/WebSolution/ModulesComposition` as the host-facing module composition boundary.
- Do not bypass module boundaries by wiring the host directly to module internals when an existing composition path already exists.
- Keep HTTP concerns in `ResumeModuleWeb`.
- Keep request/response contracts in `ResumeModuleAM`.
- Keep use-case orchestration, Mediator contracts, handlers, and mapping workflow logic in `ResumeModelSL`.
- Keep domain entities and domain-only concepts in `ResumeModuleDM`.
- Keep EF configuration, repository adapters, and schema-specific persistence behavior in `ResumeModulePL`.
- Keep shared infrastructure behavior inside `application/Infrastructure`.
- Keep frontend feature logic inside `application/WebSolution/websolution.client/src/features`.

## Working Style

- Read the relevant README, user stories, business requirements, and implementation files before changing behavior.
- Follow existing naming, folder, and dependency patterns before introducing new structure.
- Prefer extending current flows over inventing parallel abstractions.
- Keep changes scoped to the user request.
- Make business intent traceable when possible by connecting code changes back to existing requirement artifacts.

## Skills And Agents

- Use the project skill at `.codex/skills/project-knowledge-builder/` when the task is to build reusable project knowledge, trace architecture, or document conventions.
- Use project custom agents from `.codex/agents/` as focused helpers, not as replacements for reading the repo yourself.
- If a task needs durable repository guidance, prefer `AGENTS.md`.
- If a task needs a reusable workflow, prefer a skill.
- If a task needs a reusable specialist subagent, prefer `.codex/agents/*.toml`.

## Verification Expectations

- For code changes, run the smallest meaningful tests for the touched area, then state clearly what was and was not verified.
- For documentation, prompt, skill, or agent changes, verify structure and internal references instead of claiming runtime behavior you did not test.
- Do not claim a skill or custom agent is auto-discovered in the current session unless you actually observed it in a fresh Codex task.

## Commands

- Full solution build: `dotnet build application\ResumeEnhancerApp.slnx`
- Unit tests: `dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.csproj --no-restore`
- Integration tests: `dotnet test test\IntegrationTest\ResumeEnhancer.IntegrationTests.csproj --no-restore`
- API host: `dotnet run --project application\WebSolution\WebSolution.Server\WebSolution.Server.csproj`
- Migration help: `dotnet run --project application\Infrastructure\Migration\Migration.csproj -- --help`

## Checks Required For Instruction-Oriented Changes

When changing `AGENTS.md`, `.codex/`, or `Prompts/`:

- review the edited files for broken paths, broken cross-references, and contradictory guidance
- run `Get-ChildItem -Recurse .codex, Prompts`
- run `git diff -- AGENTS.md .codex Prompts`

## Knowledge Artifact Location

- Save durable project knowledge under `KnowledgeBase/` unless the user explicitly requests another location.
