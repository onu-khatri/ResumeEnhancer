# WebSolution.Server Project

This project is the API host for the application.

It should compose shared infrastructure through dependency injection, expose HTTP endpoints, and read runtime configuration. Business modules should be entered through `ModulesComposition` methods, such as `AddApplicationModules()` and `MapApplicationModuleApis()`, rather than direct host references to module AM, SL, PL, DM, or Web projects.

Database migrations and seed execution are handled by `Infrastructure/Migration`, not by normal web startup.
