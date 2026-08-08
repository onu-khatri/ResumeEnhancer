# ResumeEnhancer

ResumeEnhancer is a modular .NET solution for building resume-related domain, persistence, migration, caching, API, and web-client components.

## Solution Layout

| Path | Purpose |
| --- | --- |
| `application/Core/DomainLibrary` | Shared domain base types such as audit, setup, and business entities. |
| `application/Core/CommonLibrary` | General-purpose shared helpers that are not domain model concepts. |
| `application/Infrastructure/Persistence` | Shared EF Core `AppDbContext`, module mapping conventions, and setup seeding helpers. |
| `application/Infrastructure/Migration` | EF Core migration console for creating, applying, and seeding database changes. |
| `application/Infrastructure/Caching` | Provider-neutral cache abstractions and implementations. |
| `application/Modules/ResumeModule` | Resume module domain, persistence, service, and web layers. |
| `application/WebSolution` | Host API project and React client. |

## Domain Table Categories

Domain entities inherit from `DomainLibrary.DomainModel` base classes:

- `SetupEntity` maps to `S_*` tables and represents seedable setup/master data.
- `SetupRelation` maps to `SR_*` tables and represents seedable setup/config relationships.
- `BusinessEntity` maps to `B_*` tables and represents live business records.
- `BusinessRelation` maps to `BR_*` tables and represents live relationship/child records.

`SetupData` types carry `Code`, `Description`, `Guid`, and `ObsoleteFlag`. `AuditEntity.App_Version` is a SQL Server rowversion byte array used by EF Core optimistic concurrency checks.

## Build

```powershell
dotnet build application\ResumeEnhancerApp.slnx
```

## Migrations

```powershell
dotnet run --project application\Infrastructure\Migration\Migration.csproj -- --help
```
