# ResumeEnhancer.WebSolution.ModulesComposition Project

This project is the application-level module composition boundary.

`ResumeEnhancer.WebSolution.Server` references this project instead of referencing individual module AM, SL, PL, DM, or Web projects. This keeps the host startup small while giving the application one common place to compose current and future modules.

## Responsibilities

- Register module persistence adapters.
- Register module web/application services.
- Expose host-facing endpoint mapping for all module APIs.

For the Resume module, this project currently calls `AddResumeModulePersistence()`, `AddResumeModuleWeb()`, and `ResumeMinimalApis.MapResumeModuleApis(...)`.

Keep business rules in module SL projects, HTTP endpoint logic in module Web projects, and EF Core implementation details in module PL projects.


