# ResumeEnhancer.WebSolution.Server Project

This project is the API host for the application.

It should compose shared infrastructure through dependency injection, expose HTTP endpoints, and read runtime configuration. Business modules should be entered through `ResumeEnhancer.WebSolution.ModulesComposition` methods, such as `AddApplicationModules()` and `MapApplicationModuleApis()`, rather than direct host references to module AM, SL, PL, DM, or Web projects.

Database migrations and seed execution are handled by `Infrastructure/Migration`, not by normal web startup.

## Authentication key-ring deployment requirement

Production must set `Auth:Security:DataProtectionKeyRingPath` to an existing, writable, persistent directory shared by every host instance, and keep `Auth:Security:DataProtectionApplicationName` and `Auth:Security:DataProtectionPurpose` identical across deployments. The directory must be backed up and protected with deployment-specific filesystem ACLs; it must not be ephemeral container storage. Web startup fails closed when the key-ring binding is missing or invalid. Migration/initial provisioning also requires `AUTH_DATA_PROTECTION_KEY_RING_PATH` to point at the same persisted key ring.

