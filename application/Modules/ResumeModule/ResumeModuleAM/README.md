# ResumeEnhancer.ResumeModule.AM Project

This project contains Resume module application models.

Application models are request and response contracts shared between the module web/API layer and the module service layer. Keep transport-facing DTOs here when both `ResumeEnhancer.ResumeModule.Web` and `ResumeEnhancer.ResumeModule.SL` need to understand the same shape.

## Rules

- Keep application models persistence-free.
- Do not reference `ResumeEnhancer.ResumeModule.DM`, `ResumeEnhancer.ResumeModule.PL`, or ASP.NET Core types from this project.
- Put request contracts in `Requests/`.
- Put response contracts in `Responses/`.
- Keep EF Core entities in `ResumeEnhancer.ResumeModule.DM`, not in application models.

