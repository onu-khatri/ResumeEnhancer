# ResumeEnhancer

ResumeEnhancer is a modular .NET solution for resume management. It combines a
layered modular-monolith architecture with CQRS-style application handlers,
Minimal APIs, shared EF Core persistence infrastructure, explicit migrations,
and a React/Vite client shell.

## Architecture Summary

The solution is organized by platform area first, then by business module:

- `Core` contains shared domain and common library projects.
- `Infrastructure` contains reusable technical capabilities such as caching,
  persistence, and migrations.
- `Modules` contains business modules. The current implemented module is
  `ResumeModule`.
- `WebSolution` contains the ASP.NET Core host and React/Vite client.

The Resume module is split into application model, domain model, service layer,
persistence layer, and web/API layer projects. The host composes shared
infrastructure and enters all modules through `WebSolution/ModulesComposition`;
that boundary registers module web layers and persistence adapters.

## Architecture Fit

These scores are a qualitative snapshot of the current structure, not a claim
that every pattern is implemented in its strictest form.

| Architecture or practice | Current fit | How the project fulfills it | Remaining gap or tradeoff |
| --- | --- | --- | --- |
| Layered Architecture | Strong, about 92% | The solution has clear Core, Infrastructure, Modules, and WebSolution areas. Resume module code is split into AM, DM, SL, PL, Web, and application composition responsibilities. | Some shared infrastructure is still registered directly by the host, which is normal for the current composition style. |
| Modular Architecture | Strong, about 92% | Resume functionality is isolated under `Modules/ResumeModule`; host-facing registration starts through `WebSolution/ModulesComposition`, so the host does not reference module AM, SL, PL, DM, or Web projects directly. | Cross-module communication rules are not yet needed or formalized because there is only one business module. |
| Vertical Slice Architecture | Moderate to strong, about 72% | Resume command/query contracts, handlers, validators, and Minimal API endpoints are split per use case, with one endpoint operation per file. | The solution is still primarily layered by project. A stricter vertical-slice structure would group all files for a use case together across Web/SL/PL boundaries. |
| Clean Architecture | Strong, about 90% | Domain model projects are infrastructure-free, `ResumeModuleSL` owns use cases and persistence ports, `ResumeModulePL` implements those ports, Web does not reference PL, and module composition is isolated in an outer application-level project. | Tests or analyzers would make the dependency rule easier to enforce automatically. |
| Clean Code principles | Strong, about 88% | Responsibilities are grouped into small folders/files, sample host code has been removed, dependencies are registered explicitly, validation and mapping are centralized, and README files document local rules. | Automated tests would strengthen maintainability further. |

## Current Dependency Rule

The dependency direction is the main architectural guardrail:

```text
WebSolution.Server
  -> Infrastructure/Caching
  -> Infrastructure/Persistence
  -> WebSolution/ModulesComposition

ModulesComposition
  -> ResumeModuleWeb
  -> ResumeModulePL

ResumeModuleWeb
  -> Core/WebLibrary
  -> ResumeModuleAM
  -> ResumeModuleSL

ResumeModuleSL
  -> ResumeModuleAM
  -> ResumeModuleDM

ResumeModulePL
  -> ResumeModuleSL (persistence abstractions)
  -> ResumeModuleDM
  -> Infrastructure/Persistence

Infrastructure/Migration
  -> Infrastructure/Persistence
  -> Modules/ResumeModule/ResumeModulePL

ResumeModuleDM
  -> Core/DomainLibrary

Infrastructure/Persistence
  -> Core/DomainLibrary
```

The host should not reference Resume module AM, DM, SL, PL, or Web projects
directly; it should enter modules through `WebSolution/ModulesComposition`.
`ModulesComposition` owns module registration, so it may reference module Web
and PL projects. `ResumeModuleWeb` must not reference PL. `ResumeModuleSL` must
not reference PL; it defines persistence ports under application abstractions,
and PL implements them. Domain entities stay in DM, use-case orchestration stays
in SL, EF/database work stays in PL and shared Persistence, and HTTP concerns
stay in Web.

## Outer-To-Domain Dependency Flow

Solid arrows show current compile-time dependency flow from outer layers toward
domain types. Domain projects do not depend back on web, application,
persistence, caching, or migration projects.

```mermaid
flowchart LR
    subgraph Outer["Outer Layer"]
        Browser["React/Vite client"]
        Host["WebSolution.Server<br/>ASP.NET Core host"]
        MigrationConsole["Migration console"]
    end

    subgraph WebBoundary["Web/API Boundary"]
        ModuleComposition["ModulesComposition<br/>module DI + endpoint facade"]
        ModuleWeb["ResumeModuleWeb<br/>Minimal APIs + validators"]
        WebLibrary["Core/WebLibrary"]
        AM["ResumeModuleAM<br/>request/response contracts"]
    end

    subgraph Application["Application / Use Cases"]
        SL["ResumeModuleSL<br/>CQRS contracts + handlers"]
        Ports["SL Abstractions/Persistence<br/>repository ports + result models"]
        Mapping["Mapster mapping helpers"]
    end

    subgraph InfrastructureAdapters["Persistence / Infrastructure Adapters"]
        PL["ResumeModulePL<br/>EF adapters + configuration"]
        Persistence["Infrastructure/Persistence<br/>AppDbContext + UoW"]
        Caching["Infrastructure/Caching"]
    end

    subgraph Domain["Domain Layer"]
        DM["ResumeModuleDM<br/>Resume entities"]
        DomainLibrary["Core/DomainLibrary<br/>domain base types"]
    end

    Browser --> Host
    Host --> ModuleComposition
    Host --> Persistence
    Host --> Caching

    ModuleComposition --> ModuleWeb
    ModuleComposition --> PL

    ModuleWeb --> WebLibrary
    ModuleWeb --> AM
    ModuleWeb --> SL

    SL --> AM
    SL --> Ports
    SL --> Mapping
    SL --> DM

    PL --> Ports
    PL --> DM
    PL --> Persistence

    Persistence --> DomainLibrary
    DM --> DomainLibrary
    MigrationConsole --> Persistence
    MigrationConsole --> PL
```

## High-Level Graph

```mermaid
flowchart TB
    subgraph WebSolution["WebSolution"]
        Client["websolution.client<br/>React + Vite"]
        Server["WebSolution.Server<br/>ASP.NET Core host"]
    end

    subgraph ResumeModule["ResumeModule"]
        Composition["ModulesComposition<br/>module facade"]
        Web["ResumeModuleWeb<br/>Minimal APIs + validation"]
        AM["ResumeModuleAM<br/>Requests + responses"]
        SL["ResumeModuleSL<br/>CQRS handlers + mapping"]
        Ports["SL Persistence Ports<br/>IResumeRepository + criteria/results"]
        PL["ResumeModulePL<br/>EF adapters + configuration"]
        DM["ResumeModuleDM<br/>Domain entities"]
    end

    subgraph Infrastructure["Infrastructure"]
        Caching["Caching<br/>ICacheProvider + strategies"]
        Persistence["Persistence<br/>AppDbContext + UoW"]
        Migration["Migration<br/>EF migration console"]
    end

    subgraph Core["Core"]
        DomainLibrary["DomainLibrary<br/>Audit/domain bases"]
        CommonLibrary["CommonLibrary"]
        WebLibrary["WebLibrary<br/>ASP.NET helpers"]
    end

    Database["SQL Server"]

    Client --> Server
    Server --> Composition
    Server --> Caching
    Server --> Persistence

    Composition --> Web
    Composition --> PL

    Web --> WebLibrary
    Web --> AM
    Web --> SL

    SL --> AM
    SL --> Ports
    SL --> DM

    PL --> Ports
    PL --> DM
    PL --> Persistence

    DM --> DomainLibrary
    Persistence --> DomainLibrary
    Persistence --> Database
    Migration --> Persistence
    Migration --> PL
```

## Solution Layout

| Path | Purpose |
| --- | --- |
| `application/Core/DomainLibrary` | Shared domain base classes such as `AuditEntity`, `SetupEntity`, `BusinessEntity`, and relation bases. |
| `application/Core/CommonLibrary` | Reserved for framework-neutral shared utilities. It currently has no active shared helpers. |
| `application/Core/WebLibrary` | ASP.NET Core-specific endpoint execution and request/header helpers. |
| `application/Infrastructure/Caching` | Provider-neutral `ICacheProvider` with in-memory, Redis/distributed cache, and MemCache strategies. |
| `application/Infrastructure/Persistence` | Shared `AppDbContext`, unit of work, repositories, model loading, query specifications, transaction wrappers, and setup seeding helpers. |
| `application/Infrastructure/Migration` | Console project for creating EF migrations, applying migrations, and running seeders. |
| `application/Modules/ResumeModule/ResumeModuleAM` | Resume request and response contracts shared by Web and SL. |
| `application/Modules/ResumeModule/ResumeModuleDM` | Resume domain entities and setup/business relation model. |
| `application/Modules/ResumeModule/ResumeModelSL` | Resume service layer project named `ResumeModuleSL`; owns CQRS contracts, Mediator handlers, persistence abstractions, and Mapster mapping helpers. |
| `application/Modules/ResumeModule/ResumeModulePL` | Resume persistence layer; owns EF configurations, module schema, implementations of SL-owned persistence ports, and seed data. |
| `application/Modules/ResumeModule/ResumeModuleWeb` | Resume module web boundary; owns Minimal API endpoints and FluentValidation validators. |
| `application/WebSolution/ModulesComposition` | Application-level module composition boundary; registers module Web/PL projects and exposes endpoint mapping. |
| `application/WebSolution/WebSolution.Server` | ASP.NET Core host, OpenAPI/Scalar setup, SPA hosting, and dependency composition. |
| `application/WebSolution/websolution.client` | React + TypeScript + Vite client shell. |

## Responsibility-Based Folder Layout

`Infrastructure/Persistence` is grouped by persistence responsibility:

```text
Audit/
Composition/
Context/
Loading/
Querying/
Repositories/
Seeding/
Transactions/
UnitOfWork/
```

`ResumeModelSL` is grouped by CQRS/service-layer responsibility:

```text
Composition/
Abstractions/Persistence/
Contracts/Commands/
Contracts/Queries/
Handlers/Commands/
Handlers/Queries/
Mapping/
```

`ResumeModulePL` is grouped by persistence responsibility:

```text
Composition/
Configurations/
Context/
Repositories/
Seeding/
```

`ModulesComposition` is grouped as a small host-facing facade:

```text
DependencyInjection.cs
```

`ResumeModuleWeb` separates route registration, command endpoints, query
endpoints, and request validators:

```text
MiniApis/Commands/
MiniApis/Queries/
Validation/PersonalInformation/
Validation/Resumes/
Validation/Sections/
Validation/Shared/
```

## Patterns In Use

| Pattern | Where | Notes |
| --- | --- | --- |
| Modular monolith | Whole solution | One deployable host with isolated business modules. |
| Module composition boundary | `WebSolution/ModulesComposition` | Host references one module composition project; it composes Resume and future module Web/PL projects. |
| Layered module architecture | Resume module AM/DM/SL/PL/Web projects | Keeps transport, contracts, use cases, domain, and persistence separate. |
| CQRS-style handlers | `ResumeModelSL/Contracts` and `ResumeModelSL/Handlers` | Commands and queries are separate contracts handled through Mediator. |
| Mediator pattern | `ResumeModuleWeb` + `ResumeModelSL` | Minimal APIs send commands/queries to SL handlers via the martinothamar `Mediator` package. |
| Repository pattern | `ResumeModelSL/Abstractions/Persistence`, `ResumeModulePL/Repositories`, and `Infrastructure/Persistence/Repositories` | SL defines Resume-specific persistence ports, PL implements them, and shared Persistence exposes common audited-entity repositories. |
| Unit of Work | `Infrastructure/Persistence/UnitOfWork` | One scoped `AppDbContext` and one scoped `UnitOfWork<AppDbContext>` per DI scope. |
| Query Specification | `Infrastructure/Persistence/Querying` | Reusable criteria/include/order/projection query shapes for audited entities. |
| Model Loader | `Infrastructure/Persistence/Loading` | Typed nested include-path builder for repository queries. |
| FluentValidation | `ResumeModuleWeb/Validation` | Request validation is a web-layer concern. SL handlers assume valid request contracts. |
| Mapster mapping | `ResumeModelSL/Mapping` | Maps AM contracts, DM entities, and persistence result models. |
| Strategy pattern | `Infrastructure/Caching/Strategies` | Cache provider behavior is selected by configuration. |
| Code-first migrations | `Infrastructure/Migration` | EF Core migrations live outside normal web startup. |

Auto-registration attributes, when introduced, should be used only in PL
implementation projects. Web and SL registrations should stay explicit.

## Resume API Surface

Resume Minimal APIs are grouped under:

```text
/api/resumes
```

Current endpoints:

| Method | Route | Use case |
| --- | --- | --- |
| `POST` | `/api/resumes/` | Create a resume. |
| `PUT` | `/api/resumes/{resumeId}` | Update a resume. |
| `DELETE` | `/api/resumes/{resumeId}` | Delete one resume. |
| `POST` | `/api/resumes/delete` | Delete multiple resumes. |
| `GET` | `/api/resumes/{resumeId}` | Get resume detail. |
| `POST` | `/api/resumes/search` | Search resumes with paging and filters. |
| `GET` | `/api/resumes/{resumeId}/exists` | Check resume existence. |

Endpoint handlers live one operation per file under
`ResumeModuleWeb/MiniApis/Commands` and `ResumeModuleWeb/MiniApis/Queries`.

## Persistence Model

`Infrastructure/Persistence` provides a shared `AppDbContext` and persistence
toolkit:

- audit-aware `SaveChangesAsync(IAudit)`
- optimistic concurrency retry for rowversion-backed `AuditEntity.App_Version`
- common `IAuditEntityRepository<T>` operations
- paged query results
- query specifications
- typed nested model loaders
- relational, nested, and non-relational transaction wrappers
- setup-data seeding helpers
- module table/schema mapping conventions

`UnitOfWork<AppDbContext>` is scoped with the `AppDbContext` and coordinates
repositories, save operations, setup entity preloading, and transaction entry.
It is infrastructure only; business rules belong in module handlers/services.

## Domain Table Categories

Shared domain base types in `DomainLibrary.DomainModel` drive table categories:

| Base type | Table prefix | Use |
| --- | --- | --- |
| `SetupEntity` | `S_` | Seedable setup/master data. |
| `SetupRelation` | `SR_` | Seedable setup/config relationships. |
| `BusinessEntity` | `B_` | Operational root business records. |
| `BusinessRelation` | `BR_` | Operational child/relationship records. |

The Resume module schema is `resume`, so examples include
`resume.B_Resume`, `resume.BR_Education`, and
`resume.S_ResumeSectionSetup`.

## Validation And Mapping Rules

- Web request validation belongs in `ResumeModuleWeb`.
- FluentValidation validators live beside the web endpoints by request area.
- Simple and cross-field request rules should be expressed in validators.
- SL handlers should not duplicate request validation.
- Mapping belongs in `ResumeModelSL/Mapping` and uses Mapster.
- Custom mapping code should stay focused on workflow concerns such as access
  checks, normalization, and EF collection synchronization.

## Migration Workflow

Migrations are handled by the console project, not by normal web startup.

Show migration help:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- --help
```

Create a migration:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -c AddResumeFields
```

Apply pending migrations:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -a
```

Run seeders:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -s
```

Create, apply, and seed:

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- -c AddResumeFields -a -s
```

The migration console is verbose by default. It prints EF CLI details, pending
migrations, registered seeders, EF diagnostics, colored severity messages, and
full exception stack traces on failure.

Default development connection string:

```text
Data Source=localhost;Integrated Security=True;Persist Security Info=False;Server=TLG-PF5R29H7;Encrypt=True;TrustServerCertificate=True;Initial Catalog=ResumeEnhancer
```

## Main Dependencies

| Area | Dependencies |
| --- | --- |
| Runtime | .NET `net10.0` projects with nullable reference types enabled. |
| Web/API | ASP.NET Core, `Microsoft.AspNetCore.OpenApi`, `Scalar.AspNetCore`. |
| Persistence | EF Core `Microsoft.EntityFrameworkCore`, `Relational`, `SqlServer`, and `Design` for migrations. |
| CQRS/Mediator | `Mediator.Abstractions` and `Mediator.SourceGenerator`. |
| Mapping | `Mapster`. |
| Validation | `FluentValidation` and `FluentValidation.DependencyInjectionExtensions`. |
| Caching | `Microsoft.Extensions.Caching.*`, including in-memory and StackExchangeRedis support. |
| Client | React + TypeScript + Vite through the `.esproj` client project. |

## Build

Build the full solution:

```powershell
dotnet build application\ResumeEnhancerApp.slnx
```

Run the API host:

```powershell
dotnet run --project application\WebSolution\WebSolution.Server\WebSolution.Server.csproj
```

In development, the host maps OpenAPI and Scalar API reference UI through
`MapOpenApi()` and `MapScalarApiReference()`.

## Adding A New Module

Use the Resume module as the template:

1. Create `<ModuleName>DM` for domain entities.
2. Create `<ModuleName>AM` for request and response contracts.
3. Create `<ModuleName>SL` for CQRS contracts, handlers, mapping, and persistence ports.
4. Create `<ModuleName>PL` for EF configuration, schema, seeders, and implementations of SL-owned ports.
5. Create `<ModuleName>Web` for Minimal APIs, controllers, validators, and module web registration.
6. Add the module to `WebSolution/ModulesComposition` for host-facing registration and endpoint mapping.
7. Register persistence with `Add<ModuleName>Persistence()` inside `ModulesComposition`.
8. Register web/application dependencies with `Add<ModuleName>Web()` inside `ModulesComposition`.
9. Expose module registration through `AddApplicationModules()`.
10. Reference the module PL project from `Infrastructure/Migration`.
11. Reference only `WebSolution/ModulesComposition` from the host.
12. Create and review an EF migration.

## Current Notes

- Resume CRUD/search/exists API flows are implemented through Minimal APIs,
  Mediator handlers, Mapster mapping, SL-owned repository ports, and PL
  repository adapters.
- `Core/CommonLibrary` is intentionally light and currently has no active
  shared helpers.
