# ResumeEnhancer.Core.DomainLibrary Project

This project contains shared domain model base types used by application modules.

## Purpose

ResumeEnhancer.Core.DomainLibrary owns the common inheritance model:

| Type | Purpose | Table Prefix |
| --- | --- | --- |
| `SetupEntity` | Seedable setup/master data. | `S_` |
| `SetupRelation` | Seedable setup/config relationship data. | `SR_` |
| `BusinessEntity` | Operational business records. | `B_` |
| `BusinessRelation` | Operational relationship or child records. | `BR_` |

All four categories inherit `AuditEntity`.

## AuditEntity

`AuditEntity` provides:

- `Id`
- `App_CreateUserId`
- `App_UpdateUserId`
- `App_CreateDate`
- `App_UpdateDate`
- `App_Version`

`App_Version` is a `byte[]` rowversion value. It is managed by the database and used by EF Core for optimistic concurrency checks. Application code should not treat it as a business version number or edit it manually.

## SetupData

`SetupEntity` and `SetupRelation` inherit `SetupData`, which adds:

- `Code`
- `Description`
- `Guid`
- `ObsoleteFlag`

Setup data is eligible for seeding. Seed data should use stable `Guid` values and stable `Code` values.

## BusinessData

`BusinessEntity` and `BusinessRelation` inherit `BusinessData`.

Business data does not include setup GUID or obsolete semantics. It follows the normal application CRUD lifecycle.

## Dependency Rule

ResumeEnhancer.Core.DomainLibrary should stay infrastructure-free. Do not reference EF Core, web frameworks, caching providers, or module persistence projects from this project.

