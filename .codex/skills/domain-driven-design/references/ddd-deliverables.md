# DDD Deliverables Checklist (ResumeEnhancer)

Use this checklist to keep DDD adoption practical and measurable. Map strategic ideas to the existing modular monolith rather than imposing a new structure.

## Domain vocabulary observed in the repository

- `Resume` is the aggregate root (a `BusinessEntity` with `UserId` ownership).
- Sections (`Education`, `Certification`, `Skill`, `WorkExperience`, `Project`) are value-like children of `Resume`.
- `PersonalInformation` is a `BusinessRelation` (1:1 child) that owns `Address`, `Award`, `Language`, `Hobby`, and `SocialMediaLink`.
- `ResumeSectionSetup` is `SetupData` (lookup/config), seeded idempotently via `IAppDbContextSeeder`.
- `ResumeSectionType` enumerates the section ordering/visibility configuration.

## Strategic deliverables

- Subdomain map: core (resume authoring/storage), supporting (identity, profile), generic (caching, persistence).
- Bounded-context map aligned to modules: `<ModuleName>` (Web/AM/SL/PL/DM), `IdentityModule`, `ProfileModule`.
- Ubiquitous-language glossary for resume terms vs. code symbols.
- 1-2 ADRs documenting critical boundary decisions (e.g., module schema isolation, audit pipeline ownership).

## Tactical deliverables

- Aggregate list with invariants (e.g., a `Resume` owns its sections; section ids are owned by one resume).
- Value-object list where meaningful (e.g., `Address`, `ProficiencyLevel`, `TechnologiesUsed`).
- Repository contracts (`IResumeRepository`) and transaction boundaries (`IUnitOfWork`).
- Mapping boundaries: AM contracts -> DM entities via `ResumeModelMapper`; no DM entity leaks to the API.

## Evented deliverables (only when required)

- Command/query separation rationale (already enforced via Mediator `ICommand`/`IQuery`).
- Event schema versioning policy.
- Saga compensation matrix.
- Projection rebuild strategy.

## When NOT to deepen DDD

- Straightforward CRUD sections with no invariants.
- Setup/lookup data that is better modeled as `SetupData` than as a full aggregate.