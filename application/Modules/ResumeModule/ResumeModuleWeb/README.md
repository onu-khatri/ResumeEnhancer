# ResumeEnhancer.ResumeModule.Web Project

This project contains Resume module web-facing endpoints.

Keep transport concerns here, such as controllers or minimal APIs. Domain entities belong in `ResumeEnhancer.ResumeModule.DM`, service/use-case code belongs in `ResumeEnhancer.ResumeModule.SL`, application request/response models belong in `ResumeEnhancer.ResumeModule.AM`, and EF Core configuration belongs in `ResumeEnhancer.ResumeModule.PL`.

This project is the Resume module HTTP entry boundary. The host application should enter modules through `WebSolution/ModulesComposition`, which calls `AddResumeModuleWeb()` and maps these endpoints.

`ResumeEnhancer.ResumeModule.Web` should not reference `ResumeEnhancer.ResumeModule.PL` or `ResumeEnhancer.ResumeModule.DM`. Endpoint code should stay transport-focused and reach use cases through Mediator handlers. `ResumeEnhancer.ResumeModule.SL` owns the persistence ports and must not reference PL.

Shared endpoint execution and HTTP helper code should come from `Core/WebLibrary`. Request validators should stay in this web module and use FluentValidation for request-specific rules.

