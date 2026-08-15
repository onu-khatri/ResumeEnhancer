---
title: ResumeEnhancer.Infrastructure.Persistence Project Knowledge Base Plan
topic_slug: persistence-project
audience: combination
status: draft
plan_date: 2026-08-15
depends_on_interview: true
approved_to_compose: false
---

# ResumeEnhancer.Infrastructure.Persistence Project Knowledge Base Plan

## Purpose
- Produce a durable, AI-agent-oriented knowledge base for the shared ResumeEnhancer.Infrastructure.Persistence project and its integration seams.

## Interview Outcome
- Audience: AI agent
- Depth: comprehensive, sectioned by discovered project responsibilities
- Primary use: implementation guidance and architectural understanding for agents
- Required sections: sections should follow the discovered structure of the project, cover all required working knowledge, include concrete extension recipes for adding entities, repositories, seeders, and migrations, and prefer short code snippets over file-only evidence references
- Scope constraints: focus only on `application/Infrastructure/Persistence/ResumeEnhancer.Infrastructure.Persistence.csproj`; mention other projects only when needed as file-level evidence for how the shared persistence project is consumed
- Open assumptions: none

## Scope
- In scope:
  - Shared ResumeEnhancer.Infrastructure.Persistence architecture and responsibilities
  - `AppDbContext` composition and save pipeline
  - dependency injection and unit-of-work lifecycle
  - table/schema mapping conventions
  - repository, query specification, and model loader patterns
  - setup seeding and migration integration boundaries
  - extension recipes for adding entities, repositories, seeders, and migrations
- Out of scope:
  - `ResumeEnhancer.ResumeModule.PL` internals except where they serve as file-level examples of ResumeEnhancer.Infrastructure.Persistence extension seams
  - `Infrastructure/Migration` internals except where they show how shared ResumeEnhancer.Infrastructure.Persistence is used
  - detailed business behavior of Resume handlers and APIs
  - frontend usage patterns
  - non-persistence module internals

## Planned Output Files
- `KnowledgeBase/persistence-project.kb_plan.md`
- `KnowledgeBase/persistence-project.pre-knowledge.md`
- `KnowledgeBase/persistence-project.knowledge.md` after approval

## Planned Sections
### 1. Intent and when to use this knowledge
- Goal:
  - help an agent recognize when the shared ResumeEnhancer.Infrastructure.Persistence artifact is the right reference
- Evidence to gather:
  - shared repo and project README positioning
- Expected symbols / files:
  - `README.md`
  - `application/Infrastructure/Persistence/README.md`
  - `application/Infrastructure/Persistence/ResumeEnhancer.Infrastructure.Persistence.csproj`

### 2. Architectural placement and boundaries
- Goal:
  - explain where shared ResumeEnhancer.Infrastructure.Persistence sits relative to module PL projects, the host, and migrations
- Evidence to gather:
  - composition-root registrations and project dependency rules
- Expected symbols / files:
  - `application/WebSolution/ModulesComposition/DependencyInjection.cs`
  - `application/Modules/ResumeModule/ResumeModulePL/README.md`
  - `application/Infrastructure/Migration/README.md`

### 3. AppDbContext composition and save pipeline
- Goal:
  - document how `AppDbContext` receives module configurations, validates entities, applies audit values, and retries concurrency conflicts
- Evidence to gather:
  - `AppDbContext` constructors, `OnModelCreating`, save overloads, validation, concurrency retry logic
- Expected symbols / files:
  - `application/Infrastructure/Persistence/Context/AppDbContext.cs`
  - `application/Infrastructure/Persistence/Context/IAppDbContextModelConfiguration.cs`

### 4. Dependency injection and unit-of-work lifecycle
- Goal:
  - show how `AddAppDbContext` wires the scoped persistence boundary and how repositories are resolved and cached
- Evidence to gather:
  - DI registration, unit-of-work APIs, transaction behavior, disposal rules
- Expected symbols / files:
  - `application/Infrastructure/Persistence/Composition/DependencyInjection.cs`
  - `application/Infrastructure/Persistence/UnitOfWork/UnitOfWork.cs`
  - `application/Infrastructure/Persistence/UnitOfWork/UnitOfWorkFactory.cs`
  - `application/Infrastructure/Persistence/Transactions/`

### 5. Table, schema, and entity mapping conventions
- Goal:
  - capture the naming rules and anti-patterns an agent must preserve when extending EF mappings
- Evidence to gather:
  - schema normalization, table prefixing, base-column conventions, module configuration usage
- Expected symbols / files:
  - `application/Infrastructure/Persistence/Context/ModelBuilderModuleMappingExtensions.cs`
  - `application/Infrastructure/Persistence/Context/ModuleSchemaName.cs`
  - `application/Modules/ResumeModule/ResumeModulePL/Context/ResumeModuleDbContextModelConfiguration.cs`

### 6. Repository, query, and loading extension patterns
- Goal:
  - document the safe extension points for common audited repositories, module-specific repositories, query specifications, and include-path loading
- Evidence to gather:
  - repository contracts, query specification helpers, model loader behavior, existing Resume repository example
- Expected symbols / files:
  - `application/Infrastructure/Persistence/Repositories/`
  - `application/Infrastructure/Persistence/Querying/`
  - `application/Infrastructure/Persistence/Loading/`
  - `application/Modules/ResumeModule/ResumeModulePL/Repositories/ResumeRepository.cs`
  - `application/Modules/ResumeModule/ResumeModelSL/Abstractions/Persistence/`

### 7. Seeding and migration integration
- Goal:
  - explain how seeders plug in, how setup data is managed, and how the migration console consumes the shared ResumeEnhancer.Infrastructure.Persistence model
- Evidence to gather:
  - seed contracts, setup data helper rules, migration console registration and workflow
- Expected symbols / files:
  - `application/Infrastructure/Persistence/Seeding/IAppDbContextSeeder.cs`
  - `application/Infrastructure/Persistence/Seeding/SetupDataSeedingExtensions.cs`
  - `application/Modules/ResumeModule/ResumeModulePL/Seeding/ResumeModuleSeeder.cs`
  - `application/Infrastructure/Migration/Program.cs`
  - `application/Infrastructure/Migration/README.md`

### 8. Extension recipes
- Goal:
  - give agents concrete, ordered recipes for adding a new entity, repository, seeder, and migration without breaking project boundaries
- Evidence to gather:
  - shared ResumeEnhancer.Infrastructure.Persistence README guidance plus existing module examples that exercise ResumeEnhancer.Infrastructure.Persistence extension points
- Expected symbols / files:
  - `application/Infrastructure/Persistence/README.md`
  - `application/Modules/ResumeModule/ResumeModulePL/Context/ResumeModuleDbContextModelConfiguration.cs`
  - `application/Modules/ResumeModule/ResumeModulePL/Repositories/ResumeRepository.cs`
  - `application/Modules/ResumeModule/ResumeModulePL/Seeding/ResumeModuleSeeder.cs`
  - `application/Infrastructure/Migration/README.md`

### 9. Rules, invariants, pitfalls, and verification
- Goal:
  - make boundaries explicit and give a cold-start agent concrete verification commands and review checks
- Evidence to gather:
  - persistence README rules, migration workflow, repo-level verification commands
- Expected symbols / files:
  - `AGENTS.md`
  - `application/Infrastructure/Persistence/README.md`
  - `application/Infrastructure/Migration/README.md`

## Evidence Collection Plan
- README / requirements:
  - `README.md`
  - persistence- and migration-related README files
- backend:
  - shared ResumeEnhancer.Infrastructure.Persistence code under `application/Infrastructure/Persistence`
  - Resume persistence adapter examples under `application/Modules/ResumeModule/ResumeModulePL`
- frontend:
  - none expected beyond confirming it is out of scope
- tests:
  - inspect tests that cover `AppDbContext`, repository behavior, or migration/persistence conventions if present
  - use short code snippets in the final artifact rather than line-level citations

## Validation Plan
- Pass the sequential gates A–H from `references/knowledge-quality-gates.md` (Grounding -> Specificity -> Reproducibility -> User Interview -> Consistency -> Boundary -> Currency -> Record).
- User approvals required:
  - user approved `*.kb_plan.md`
  - user answered Gate D interview questions
  - user approved `*.pre-knowledge.md`
- Companion checks: `references/artifact-validator-checklist.md` and `references/artifact-cross-examination.md`.

## Approval Gate
- User may edit this plan file directly before composition starts.
- Do not create `*.pre-knowledge.md` until the user explicitly approves this plan.



