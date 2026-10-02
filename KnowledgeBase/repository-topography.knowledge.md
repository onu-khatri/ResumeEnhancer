---
title: Repository Topography
intent: Provide a fast map of where to gather evidence across the ResumeEnhancer repository layers.
scope: ResumeEnhancer repository layout, module names, evidence zones, and verification commands. Excludes generic architecture and pattern guidance.
audience: Autonomous Codex implementation and knowledge-building agents
last_reviewed: 2026-10-02
---

# Repository Topography

Read this knowledge when the requested work spans multiple layers or when you need a fast map of where to gather evidence.

## Main evidence zones

### Product intent

- `README.md`
- `Business-Requirements/` (e.g., `builder-domain.BR.md`, `ai-analysis-domain.BR.md`, `billing-entitlements-domain.BR.md`)
- `User-Stories/` (each slice is a trio: `<epic>.<n> <slug>.US.md`, `.SI.md`, `.Research.md`)

Use these to understand business vocabulary, scope boundaries, and expected behaviors before inferring how the implementation should work.

### Backend implementation

- `application/WebSolution/WebSolution.Server/`
- `application/WebSolution/ModulesComposition/`
- `application/Modules/<ModuleName>/` — the modules: `IdentityModule`, `ProfileModule`, `ResumeModule`.
- Each module splits into sub-layers: `ResumeEnhancer.<ModuleName>.Web`, `ResumeEnhancer.<ModuleName>.AM`, `<ModuleName>ModelSL`, `ResumeEnhancer.<ModuleName>.DM`, `ResumeEnhancer.<ModuleName>.PL`.
- `application/Infrastructure/` (ResumeEnhancer.Infrastructure.Caching, Migration, ResumeEnhancer.Infrastructure.Persistence)
- `application/Core/`

Use these to trace request flow, dependency direction, validation, mapping, persistence, and module composition.

Layer responsibilities (from `AGENTS.md`, expressed generically per module):

- `ResumeEnhancer.<ModuleName>.Web` — HTTP concerns.
- `ResumeEnhancer.<ModuleName>.AM` — request/response contracts.
- `<ModuleName>ModelSL` — use-case orchestration, Mediator contracts, handlers, mapping workflow.
- `ResumeEnhancer.<ModuleName>.DM` — domain entities and domain-only concepts.
- `ResumeEnhancer.<ModuleName>.PL` — EF configuration, repository adapters, schema-specific persistence.
- `application/Infrastructure` — shared infrastructure (caching, migration, persistence/UnitOfWork).

### Frontend implementation

- `application/WebSolution/websolution.client/src/features/auth/`
- `application/WebSolution/websolution.client/src/features/resume/` (api, builder, dashboard, hooks, layout, model, pages, preview, state)
- `application/WebSolution/websolution.client/src/shared/`
- `application/WebSolution/websolution.client/src/routes/`
- `application/WebSolution/websolution.client/src/pages/`

Use these to understand routing, feature boundaries, API usage, page composition, local state, and reusable UI patterns.

### Verification

- `test/ResumeEnhancer.Tests/`
- `test/IntegrationTest/`
- `test/TestUtilities/IntegrationSupport/`

Use these to validate intended behavior, dependency constraints, integration boundaries, and test helpers.

## Common cross-layer topics

- Resume CRUD flow
- Search and paging flow
- Minimal API endpoint conventions
- FluentValidation request rules
- Mediator contract and handler patterns
- Mapster mapping responsibilities
- Repository and unit-of-work usage
- Frontend form schema and draft state
- Integration test setup and fake authentication
- Requirement-to-implementation traceability

## Knowledge artifact target

Unless the user explicitly asks for another location, save generated knowledge artifacts under:

- `KnowledgeBase/`

Suggested file naming (one topic → three files across the workflow):

- `<topic-name>.kb_plan.md` for the reviewable plan
- `<topic-name>.pre-knowledge.md` for the filled draft awaiting approval
- `<topic-name>.knowledge.md` for the final saved artifact

## Verification commands (for Gate C reproducibility)

- Backend build: `dotnet build application\ResumeEnhancerApp.slnx`
- Unit tests: `dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore`
- Integration tests: `dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore`
- Frontend: `npm run check` and `npm run build` in `application/WebSolution/websolution.client/`
