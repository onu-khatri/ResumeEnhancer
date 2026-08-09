# ResumeModuleWeb Project

This project is intended for Resume module web-facing endpoints.

Keep transport concerns here, such as controllers or minimal APIs. Domain entities belong in `ResumeModuleDM`, service/use-case code belongs in `ResumeModuleSL`, application request/response models belong in `ResumeModuleAM`, and EF Core configuration belongs in `ResumeModulePL`.

This project is also the Resume module entry boundary. The host application should call `AddResumeModuleWeb()` from this project instead of referencing `ResumeModuleSL`, `ResumeModulePL`, or `ResumeModuleDM` directly.

`ResumeModuleWeb` should not reference `ResumeModuleDM` or `ResumeModulePL`. Domain and persistence dependencies are resolved by `ResumeModuleSL`.

Shared endpoint execution and HTTP helper code should come from `Core/WebLibrary`. Request validators should stay in this web module and use FluentValidation for request-specific rules.
