# ResumeModuleAM Project

This project contains Resume module application models.

Application models are request and response contracts shared between the module web/API layer and the module service layer. Keep transport-facing DTOs here when both `ResumeModuleWeb` and `ResumeModuleSL` need to understand the same shape.

## Rules

- Keep application models persistence-free.
- Do not reference `ResumeModuleDM`, `ResumeModulePL`, or ASP.NET Core types from this project.
- Put request contracts in `Requests/`.
- Put response contracts in `Responses/`.
- Keep EF Core entities in `ResumeModuleDM`, not in application models.
