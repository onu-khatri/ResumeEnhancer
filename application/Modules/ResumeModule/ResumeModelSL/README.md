# ResumeModuleSL Project

This project is intended for the Resume module service layer.

Service-layer code should coordinate use cases, validation, and application workflows. Shared request/response application models belong in `ResumeModuleAM`, domain entities belong in `ResumeModuleDM`, and persistence-specific behavior stays in `ResumeModulePL`.

The current project structure reserves folders for contracts and implementations. Register service-layer dependencies through `AddResumeModuleApplication()`, which is called by `ResumeModuleWeb`.

`ResumeModuleSL` owns the dependency from the service layer to `ResumeModuleDM` and `ResumeModulePL`, so web/API code does not reference domain or persistence directly.
