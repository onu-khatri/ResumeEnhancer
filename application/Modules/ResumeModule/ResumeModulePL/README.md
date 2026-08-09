# ResumeModulePL Project

This project contains the Resume module persistence layer.

## Purpose

ResumeModulePL owns:

- EF Core entity configurations.
- Resume module schema registration.
- `AppDbContext` extension methods for Resume `DbSet<T>` access.
- EF-backed implementations of service-layer persistence ports.
- Resume setup seed data.
- Dependency injection for module persistence registration.

## Project Layout

| Folder | Purpose |
| --- | --- |
| `Composition` | Dependency injection registration for Resume persistence services. |
| `Configurations` | EF Core entity type configurations. |
| `Context` | Resume schema, model configuration, and `AppDbContext` Resume `DbSet<T>` extensions. |
| `Repositories` | EF-backed implementations of ports from `ResumeModuleSL.Abstractions.Persistence`. |
| `Seeding` | Resume setup seed data and seeders. |

## Dependency Rule

`ResumeModulePL` may reference `ResumeModuleSL` only to implement persistence abstractions. `ResumeModuleSL` must not reference this project. Keep EF Core and `AppDbContext` code here or in shared `Infrastructure/Persistence`, and keep business rules in SL handlers.

## Table Mapping

Entity configurations should not hardcode table names. `ResumeModuleDbContextModelConfiguration` loads configurations and then calls `ApplyModuleTableMappings`, which applies:

- schema: `resume`
- `S_` prefix for setup entities
- `SR_` prefix for setup relations
- `B_` prefix for business entities
- `BR_` prefix for business relations

## Setup Seeding

`ResumeModuleSeeder` seeds `ResumeSectionSetup` through the shared setup seeding helper.

Setup seeding:

- matches by stable `Guid`
- can update an existing row when `Code` matches and `Guid` changes
- updates seed-owned values only when values changed
- marks removed seeder-managed setup rows as `ObsoleteFlag = true`

Do not use EF `HasData` for setup tables in this module.
