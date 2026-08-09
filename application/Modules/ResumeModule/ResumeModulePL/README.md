# ResumeModulePL Project

This project contains the Resume module persistence layer.

## Purpose

ResumeModulePL owns:

- EF Core entity configurations.
- Resume module schema registration.
- `AppDbContext` extension methods for Resume `DbSet<T>` access.
- Repository contracts and EF-backed repository implementations.
- Resume setup seed data.
- Dependency injection for module persistence registration.

## Project Layout

| Folder | Purpose |
| --- | --- |
| `Composition` | Dependency injection registration for Resume persistence services. |
| `Configurations` | EF Core entity type configurations. |
| `Context` | Resume schema, model configuration, and `AppDbContext` Resume `DbSet<T>` extensions. |
| `Contracts` | Persistence-layer repository contracts and result/criteria models. |
| `Repositories` | EF-backed repository implementations. |
| `Seeding` | Resume setup seed data and seeders. |

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
