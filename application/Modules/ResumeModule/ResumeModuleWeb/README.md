# ResumeModuleWeb Project

This project contains Resume module web-facing endpoints.

Keep transport concerns here, such as controllers or minimal APIs. Domain entities belong in `ResumeModuleDM`, service/use-case code belongs in `ResumeModuleSL`, application request/response models belong in `ResumeModuleAM`, and EF Core configuration belongs in `ResumeModulePL`.

This project is the Resume module HTTP entry boundary. The host application should enter modules through `WebSolution/ModulesComposition`, which calls `AddResumeModuleWeb()` and maps these endpoints.

`ResumeModuleWeb` should not reference `ResumeModulePL` or `ResumeModuleDM`. Endpoint code should stay transport-focused and reach use cases through Mediator handlers. `ResumeModuleSL` owns the persistence ports and must not reference PL.

Shared endpoint execution and HTTP helper code should come from `Core/WebLibrary`. Request validators should stay in this web module and use FluentValidation for request-specific rules.
