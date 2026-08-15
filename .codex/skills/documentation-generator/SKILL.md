---
name: documentation-generator
description: Generate accurate, project-grounded documentation for ResumeEnhancer: code explainers, API references, module guides, ADRs, and onboarding notes. Use when creating or updating docs from repository evidence.
---

# Documentation Generator

Use this skill to produce documentation that reflects the real codebase, not generic templates. It pairs with `project-knowledge-builder` for reusable knowledge and `architecture-decision-records` for decisions.

## Use this skill when

- generating or updating docs for a module, endpoint, or feature
- writing an API reference or onboarding guide
- documenting a workflow or migration step for other developers

## Do not use this skill when

- the deliverable is durable knowledge for agents (use `project-knowledge-builder`)
- the deliverable is a formal decision record (use `architecture-decision-records`)

## Workflow

1. Confirm the audience (onboarding, implementation, review, or release notes).
2. Gather evidence from `README.md`, `Business-Requirements/`, `User-Stories/`, `application/`, and `test/`.
3. Describe symbols, responsibilities, and flows using the repository's own terms.
4. Include concrete, copy-pasteable commands and file references.
5. Separate observed facts from inference; flag anything incomplete.

## ResumeEnhancer content map

- Module responsibilities: `<ModuleName>ModuleWeb` (HTTP/validation), `<ModuleName>ModuleAM` (contracts), `<ModuleName>ModuleSL` (handlers/mapping), `<ModuleName>ModulePL` (EF/repositories), `<ModuleName>ModuleDM` (entities).
- API surface: Minimal API groups under `/api/...` and their command/query endpoints.
- Persistence: shared `AppDbContext`, unit of work, repository base, seeding, migrations.
- Verification commands: `dotnet build application\ResumeEnhancerApp.slnx`, `dotnet test ...`, `npm run check`.

## Quality bar

- Every command is copy-pasteable and every path is real.
- Claims are backed by repository evidence.
- Structure is scannable (headings, tables, code blocks with language hints).
- Docs stay specific to ResumeEnhancer rather than generic .NET/React advice.

## Definition of Done

- The doc is grounded in the current codebase.
- Internal cross-references and file paths are valid.
- A new reader could follow it without additional context.
