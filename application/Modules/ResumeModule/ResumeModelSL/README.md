# ResumeEnhancer.ResumeModule.SL Project

This project contains the Resume module service layer.

Service-layer code should coordinate use cases and application workflows. Shared request/response application models belong in `ResumeEnhancer.ResumeModule.AM`, domain entities belong in `ResumeEnhancer.ResumeModule.DM`, and persistence-specific behavior stays in `ResumeEnhancer.ResumeModule.PL`.

Persistence contracts needed by handlers live here under `Abstractions/Persistence`. That lets the service layer depend on its own ports while the persistence layer implements them.

The project is grouped by CQRS responsibility:

- `Abstractions/Persistence`: repository ports plus persistence criteria/result contracts consumed by handlers.
- `Composition`: dependency registration through `AddResumeModuleApplication()`, which is called by `ResumeEnhancer.ResumeModule.Web`.
- `Contracts`: command and query contracts consumed by Mediator.
- `Handlers`: command and query handlers that coordinate use cases.
- `Mapping`: Mapster-backed mapping and update helpers used by handlers.

`ResumeEnhancer.ResumeModule.SL` depends on `ResumeEnhancer.ResumeModule.AM` and `ResumeEnhancer.ResumeModule.DM`. It does not reference `ResumeEnhancer.ResumeModule.PL`; PL implements the persistence ports declared in this project and is registered by the outer composition root.

Object-to-object mapping between AM contracts, DM entities, and persistence result models should use Mapster configuration in `Mapping`. Keep custom code there limited to workflow concerns such as access checks and EF collection synchronization.


