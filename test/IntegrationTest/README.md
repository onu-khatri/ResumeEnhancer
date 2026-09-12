# ResumeEnhancer Integration Tests

This project verifies module behavior through the real ASP.NET Core HTTP boundary. The current suite covers the Auth and Resume modules and uses a `WebApplicationFactory` configured for the `IntegrationTest` environment.

The shared test host uses SQLite in-memory persistence, fake authentication, a mocked cache provider, and deterministic test configuration. No external database or running API process is required.

## Prerequisites

- .NET 10 SDK
- A restored repository (`dotnet restore` from the repository root, if required)

Run the commands below from the repository root (`D:\RND\ResumeEnhancer`).

## Run tests

Run every integration test:

```powershell
dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore
```

If dependencies have not been restored, remove `--no-restore` from the command.

Integration test collections are configured not to run in parallel because the module fixtures share the test host and database lifecycle.

## Run one module

Use the module namespace in a `FullyQualifiedName` filter:

| Module | Command filter |
| --- | --- |
| Auth | `FullyQualifiedName~ResumeEnhancer.Tests.Integration.Modules.AuthModule` |
| Resume | `FullyQualifiedName~ResumeEnhancer.Tests.Integration.Modules.ResumeModule` |

For example, run only Auth integration tests:

```powershell
dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore --filter "FullyQualifiedName~ResumeEnhancer.Tests.Integration.Modules.AuthModule"
```

Run only Resume integration tests:

```powershell
dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore --filter "FullyQualifiedName~ResumeEnhancer.Tests.Integration.Modules.ResumeModule"
```

## Run a particular test

Filter by the test class or the full test method name. For example:

```powershell
dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore --filter "FullyQualifiedName~ResumeEnhancer.Tests.Integration.Modules.AuthModule.AuthHttpIntegrationTests.Register_AtomicSetup_ExercisesPersistenceBoundary"
```

To run the Resume command test class only:

```powershell
dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore --filter "FullyQualifiedName~ResumeEnhancer.Tests.Integration.Modules.ResumeModule.ResumeCommandIntegrationTests"
```

## Test layout

```text
IntegrationTest/
|-- Modules/
|   |-- AuthModule/
|   |   |-- AuthHttpIntegrationTests.cs
|   |   `-- TestSupport/
|   `-- ResumeModule/
|       |-- ResumeCommandIntegrationTests*.cs
|       |-- ResumeQueryIntegrationTests*.cs
|       `-- TestSupport/
|-- TestSupport/
|   `-- IntegrationTestAssemblyFixture.cs
`-- ResumeEnhancer.Tests.Integration.csproj
```

Auth tests exercise bootstrap, registration, idempotency, refresh, logout, verification resend, duplicate registration, malformed payload, and atomic registration boundaries. Resume tests exercise create, update, delete, bulk delete, get, existence, and search HTTP flows.

## Existing integration-test practices

These practices are already used by the current tests and should be retained when adding new coverage:

- Exercise the public HTTP boundary through the shared `WebApplicationFactory`; do not call handlers or repositories directly when the scenario is an integration test.
- Use `IntegrationTestAssemblyFixture.Utilities` as the shared host configuration. It selects the `IntegrationTest` environment, SQLite in-memory persistence, fake authentication, a mocked cache provider, and deterministic signing configuration.
- Arrange data through `ISetupper` and the module setup extensions. Dispose the setupper with `using var setupper`, and use `ResetDatabase` or the module fixture's reset-and-seed method when the scenario requires a clean database.
- Keep Auth and Resume tests in their existing xUnit collections (`Sequential_AuthModule` and `Sequential_ResumeModule`). These collections disable parallelization because fixtures share the host and database lifecycle.
- Use `[Theory]` with module-specific `[MemberData]` providers for endpoint setup matrices. Keep the setup data and expected behavior close to the module test area (`AuthApiTestData` or the Resume `*.Setup.cs` partial files).
- Pass `TestContext.Current.CancellationToken` through HTTP, setup, response-body, and persistence calls.
- Assert both the HTTP contract and the resulting state: status code, response body or error code, headers/claims where relevant, and persisted entities or audit values.
- Use `EndpointSetup` when the same arrange/request/assert lifecycle is shared by several endpoint scenarios.
- Keep module fixture and setup helpers under that module's `TestSupport` folder; keep assembly-wide host setup in `TestSupport/IntegrationTestAssemblyFixture.cs`.

## Adding integration tests

Add the test class under `Modules/<ModuleName>/`, use the module's sequential collection fixture, and add setup data under the module's `TestSupport` folder or a partial `*.Setup.cs` file. A new module should also configure its test host behavior in the shared integration support only when the behavior cannot be expressed through the existing setupper, fake authentication, cache, or database seams.
