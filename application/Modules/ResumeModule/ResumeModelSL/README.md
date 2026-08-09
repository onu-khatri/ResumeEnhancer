# ResumeModuleSL Project

This project is intended for the Resume module service layer.

Service-layer code should coordinate use cases, validation, and application workflows. Shared request/response application models belong in `ResumeModuleAM`, domain entities belong in `ResumeModuleDM`, and persistence-specific behavior stays in `ResumeModulePL`.

The project is grouped by CQRS responsibility:

- `Composition`: dependency registration through `AddResumeModuleApplication()`, which is called by `ResumeModuleWeb`.
- `Contracts`: command and query contracts consumed by Mediator.
- `Handlers`: command and query handlers that coordinate use cases.
- `Mapping`: Mapster-backed mapping and update helpers used by handlers.

`ResumeModuleSL` owns the dependency from the service layer to `ResumeModuleDM` and `ResumeModulePL`, so web/API code does not reference domain or persistence directly.

Object-to-object mapping between AM contracts, DM entities, and PL result models should use Mapster configuration in `Mapping`. Keep custom code there limited to workflow concerns such as access checks and EF collection synchronization.
