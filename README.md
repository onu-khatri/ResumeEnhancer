# ResumeEnhancer

ResumeEnhancer is a modular resume platform for creating, improving, organizing, and eventually monetizing professional career documents. The product is being built as a modern resume workspace: users should be able to maintain structured resume content, tailor it for roles, preview polished templates, and use guided intelligence without losing ownership of their story.

The repository currently implements the platform foundation with a .NET modular monolith, a React/Vite frontend, shared persistence infrastructure, module composition boundaries, and documented product backlog artifacts for the next product domains.

## Product Purpose

Resume work is high effort and high trust. Users need more than a document editor; they need a workspace that helps them turn experience into credible, targeted, and presentation-ready material.

ResumeEnhancer exists to support that workflow by focusing on:

- Structured resume data that can survive template, layout, and language changes.
- Guided resume creation and editing instead of blank-page authoring.
- Practical analysis that helps users understand resume strength for a target role.
- Clear customization that improves presentation without damaging content or ATS suitability.
- A modular product architecture that can grow into billing, entitlements, AI analysis, multi-language documents, and sharing.

## Product Vision

The long-term direction is a career-document platform where users can manage resumes and related documents across multiple roles, markets, and versions. ResumeEnhancer should become a trusted workspace for:

- Building and editing resumes with autosave and version-aware persistence.
- Selecting professional templates and safely changing visual presentation.
- Tailoring documents for job descriptions with honest, reviewable AI support.
- Organizing language variants, role-specific versions, and future cover-letter flows.
- Offering premium capabilities through transparent entitlements and fair upgrade boundaries.

The guiding product principle is that the system should make resumes better without silently inventing, overwriting, or obscuring the user's own experience.

## Current Status

The platform foundation is in active development. The current implementation includes:

- ASP.NET Core host with a modular-monolith backend structure.
- Resume module layers for API contracts, domain model, service logic, persistence, and Minimal API endpoints.
- Shared EF Core persistence, migrations, setup-data seeding, repositories, unit of work, and query helpers.
- React, TypeScript, Vite frontend shell with shared UI foundation work in progress.
- Unit, integration, and architecture tests around backend behavior and module boundaries.
- Product documentation under `Business-Requirements`, `User-Stories`, `prd`, `openspec`, and `KnowledgeBase`.

For detailed architecture, dependency rules, module layering, API surface, and persistence notes, see [ARCHITECTURE.md](ARCHITECTURE.md).

## Tech Stack

| Area | Technology |
| --- | --- |
| Backend | .NET 10, ASP.NET Core, Minimal APIs |
| Application flow | Mediator, CQRS-style command/query handlers |
| Validation and mapping | FluentValidation, Mapster |
| Persistence | EF Core, SQL Server, shared `AppDbContext`, explicit migration console |
| Frontend | React 19, TypeScript, Vite, React Router, TanStack Query, Tailwind CSS |
| Testing | xUnit, Shouldly, NetArchTest, ASP.NET Core integration testing, Vitest |
| Documentation | Business requirements, user stories, OpenSpec changes, ADRs, repository READMEs |

## Repository Layout

| Path | Purpose |
| --- | --- |
| `application` | Main source code for core libraries, infrastructure, modules, host, and frontend client. |
| `application/Core` | Shared domain, common, and web helper libraries. |
| `application/Infrastructure` | Cross-cutting persistence, caching, and migration infrastructure. |
| `application/Modules` | Business modules. The Resume module is the current primary implementation. |
| `application/WebSolution` | ASP.NET Core host, module composition boundary, and React/Vite client. |
| `test` | Unit tests, integration tests, and reusable test utilities. |
| `Business-Requirements` | Product and domain business requirements. |
| `User-Stories` | Implementation-ready story packs and supporting research. |
| `openspec` | OpenSpec-driven change specifications. |
| `KnowledgeBase` | Durable architecture and project knowledge artifacts. |
| `prd` | Product requirement documents. |

## Getting Started

### Prerequisites

- .NET SDK compatible with `net10.0`.
- Node.js and npm compatible with the frontend toolchain.
- SQL Server for normal local persistence workflows.
- PowerShell or a shell capable of running the documented commands.

### Build The Solution

```powershell
dotnet build application\ResumeEnhancerApp.slnx
```

### Run Backend Unit Tests

```powershell
dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore
```

### Run Backend Integration Tests

```powershell
dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore
```

### Run The API Host

```powershell
dotnet run --project application\WebSolution\WebSolution.Server\ResumeEnhancer.WebSolution.Server.csproj
```

In development, the host exposes OpenAPI and Scalar API reference endpoints.

### Work With The Frontend

```powershell
cd application\WebSolution\websolution.client
npm install
npm run dev
```

Useful frontend checks:

```powershell
npm run check
npm run build
npm run test
npm run test:coverage
```

### Migration Console

Show migration help:

```powershell
dotnet run --project application\Infrastructure\Migration\ResumeEnhancer.Infrastructure.Migration.csproj -- --help
```

Apply migrations and seed data:

```powershell
dotnet run --project application\Infrastructure\Migration\ResumeEnhancer.Infrastructure.Migration.csproj -- -a -s
```

## Product Roadmap Direction

The repository already contains requirement and story material for several future product areas:

- Resume builder persistence and autosave orchestration.
- Theme, layout, and accessibility improvements.
- Multi-language document management.
- AI-assisted resume analysis and job matching.
- Billing, plans, and entitlement-aware premium features.
- Stronger frontend foundation and shared component behavior.

These areas should continue to be delivered through traceable business requirements, user stories, OpenSpec changes, implementation tasks, and validation evidence.

## Engineering Principles

- Preserve module boundaries and enter business modules through `application/WebSolution/ModulesComposition`.
- Keep HTTP behavior in module Web projects, use-case orchestration in SL, domain concepts in DM, and EF-specific behavior in PL.
- Treat setup data by stable semantic code and resolve database IDs only when assigning foreign keys.
- Prefer small, verifiable changes that connect back to product intent.
- Report validation honestly: distinguish local builds, automated tests, coverage gates, hosted workflow results, and PR status.

## Documentation Map

- [ARCHITECTURE.md](ARCHITECTURE.md) explains current architecture, dependency rules, module layout, API surface, migrations, and test structure.
- [AGENTS.md](AGENTS.md) defines repository-level agent instructions and architecture guardrails.
- [HOW_TO_USE_CODEX_AGENTS.md](HOW_TO_USE_CODEX_AGENTS.md) explains how specialized Codex agents are used in this repository.
- `Business-Requirements/*.BR.md` captures product and domain intent.
- `User-Stories/*.US.md`, `*.SI.md`, and `*.Research.md` capture implementation-ready stories and supporting evidence.
- `openspec` tracks formal change proposals and specifications.
- `KnowledgeBase` stores durable architecture decisions and project knowledge.

## Contribution Expectations

Before changing behavior, read the relevant README, business requirement, user story, OpenSpec change, and implementation files. Keep changes scoped, preserve existing ownership boundaries, and run the smallest meaningful validation for the area touched.

For code changes, include the commands that were run and any validation that remains outstanding. For documentation-only changes, verify links, paths, and internal consistency.

## License

This repository includes a [LICENSE](LICENSE) file. Review it before distributing or reusing the project.
