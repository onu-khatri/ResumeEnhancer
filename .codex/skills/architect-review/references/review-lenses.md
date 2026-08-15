# Architecture Review Lenses (ResumeEnhancer)

Use these lenses when a change is large enough that line-by-line correctness is not the whole story. They map to the actual module boundaries in this repository.

## Boundary checks

- Do dependencies still point the right way (`Web -> SL -> AM/DM -> DomainLibrary`, `PL -> Persistence/DM/SL`)?
- Did transport concerns leak into business or persistence logic (e.g., `HttpContext` or `IResult` in a handler)?
- Are request/response contracts (AM) intentionally shaped, and are DM entities never serialized to the client?
- Is cross-module wiring confined to `ModulesComposition`?

## Evolution checks

- Will future features get easier or harder after this change?
- Is the abstraction sized to the real problem (no speculative `QuerySpecification`/`ModelLoader` indirection where a plain query suffices)?
- Does the change create hidden coupling or duplicate policy (e.g., ownership filtering re-implemented per handler instead of in the repository)?

## Risk checks

- Migration and rollout complexity: is schema change isolated to a module and covered by a migration?
- Testability and observability: can the change be verified through unit or integration tests?
- Cross-layer naming and responsibility drift: do symbols stay in their owning project (`<ModuleName>ModuleWeb`, `<ModuleName>ModuleSL`, `<ModuleName>ModulePL`, `<ModuleName>ModuleDM`)?

## Review output

- findings first
- severity and impact
- what should change now
- what should be documented or monitored later