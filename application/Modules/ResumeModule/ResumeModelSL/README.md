# ResumeModuleSL Project

This project contains the Resume module service layer.

Service-layer code should coordinate use cases and application workflows. Shared request/response application models belong in `ResumeModuleAM`, domain entities belong in `ResumeModuleDM`, and persistence-specific behavior stays in `ResumeModulePL`.

Persistence contracts needed by handlers live here under `Abstractions/Persistence`. That lets the service layer depend on its own ports while the persistence layer implements them.

The project is grouped by CQRS responsibility:

- `Abstractions/Persistence`: repository ports plus persistence criteria/result contracts consumed by handlers.
- `Composition`: dependency registration through `AddResumeModuleApplication()`, which is called by `ResumeModuleWeb`.
- `Contracts`: command and query contracts consumed by Mediator.
- `Handlers`: command and query handlers that coordinate use cases.
- `Mapping`: Mapster-backed mapping and update helpers used by handlers.

`ResumeModuleSL` depends on `ResumeModuleAM` and `ResumeModuleDM`. It does not reference `ResumeModulePL`; PL implements the persistence ports declared in this project and is registered by the outer composition root.

Object-to-object mapping between AM contracts, DM entities, and persistence result models should use Mapster configuration in `Mapping`. Keep custom code there limited to workflow concerns such as access checks and EF collection synchronization.
