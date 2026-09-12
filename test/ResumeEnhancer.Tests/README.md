# ResumeEnhancer Unit Tests

This project contains fast, isolated xUnit tests for shared infrastructure, composition, core libraries, and the application modules. Tests use substitutes and local SQLite where appropriate; they do not require the API host or an external database.

## Prerequisites

- .NET 10 SDK
- A restored repository (`dotnet restore` from the repository root, if required)

Run the commands below from the repository root (`D:\RND\ResumeEnhancer`).

## Run tests

Run every unit test:

```powershell
dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore
```

Run all tests with coverage:

```powershell
dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore --settings test\coverlet.runsettings --collect:"XPlat Code Coverage"
```

If dependencies have not been restored, remove `--no-restore` from the command.

## Run one module

Use the module namespace in a `FullyQualifiedName` filter:

| Module | Command filter |
| --- | --- |
| Auth | `FullyQualifiedName~ResumeEnhancer.Tests.Unit.Modules.AuthModule` |
| Billing | `FullyQualifiedName~ResumeEnhancer.Tests.Unit.Modules.BillingModule` |
| Profiling | `FullyQualifiedName~ResumeEnhancer.Tests.Unit.Modules.ProfilingModule` |
| Resume | `FullyQualifiedName~ResumeEnhancer.Tests.Unit.Modules.ResumeModule` |
| Template | `FullyQualifiedName~ResumeEnhancer.Tests.Unit.Modules.TemplateModule` |

For example, run only Auth module tests:

```powershell
dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore --filter "FullyQualifiedName~ResumeEnhancer.Tests.Unit.Modules.AuthModule"
```

Replace the filter value with another module namespace from the table.

## Run a particular test

Filter by the test class or the full test method name. For example:

```powershell
dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore --filter "FullyQualifiedName~ResumeEnhancer.Tests.Unit.Modules.AuthModule.AuthRegistrationServiceTests.Register_normalizes_email_and_persists_two_side_effects"
```

To run a whole test class, stop the filter at the class name:

```powershell
dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore --filter "FullyQualifiedName~ResumeEnhancer.Tests.Unit.Modules.ResumeModule.Application.ResumeHandlerTests"
```

## Test layout

```text
ResumeEnhancer.Tests/
|-- Architecture/
|-- Composition/
|-- Core/
|-- Infrastructure/
|-- Modules/
|   |-- AuthModule/
|   |-- BillingModule/
|   |-- ProfilingModule/
|   |-- ResumeModule/
|   `-- TemplateModule/
`-- TestInfrastructure/
```

## Existing test practices

These practices are already used by the current tests and should be retained when adding new coverage:

- Use xUnit v3 `[Fact]` for a single scenario and `[Theory]` with `[MemberData]` for a scenario matrix or reusable setup data.
- Keep the test namespace aligned with the folder and module, for example `ResumeEnhancer.Tests.Unit.Modules.ResumeModule.Application`.
- Use `TestContext.Current.CancellationToken` for asynchronous calls instead of creating a new token in a test.
- Use Shouldly assertions such as `ShouldBe`, `ShouldBeNull`, `ShouldBeEmpty`, `ShouldHaveSingleItem`, and `ShouldThrowAsync` so failures describe the expected behavior.
- Use NSubstitute for collaborators. Verify important calls with `Received`, while asserting the returned behavior as well.
- Use `SqliteAppDbContextScope` for repository, persistence, schema, transaction, and relational query tests. Seed through the scope and clear the EF change tracker when the test needs to verify a fresh read.
- Use `TestInfrastructure` helpers such as `ResumeTestData`, `TestAudit`, `ValidationAssertions`, and `ResultAssertionHelpers` instead of duplicating common setup and assertions.
- Test validators, handlers, mappers, endpoint delegates, repositories, seeders, composition, and architecture rules at their appropriate boundary.
- Prefer deterministic inputs and explicit edge cases: invalid identifiers, missing values, ownership mismatches, duplicate data, empty collections, pagination, and failure paths are represented in the existing module tests.
- Do not require a running API, external database, network service, wall-clock delay, or shared mutable state for a unit test.

Unit tests run in parallel according to `xunit.runner.json`. A test that depends on shared state should be isolated or use a dedicated scope rather than changing the assembly-wide parallelization setting.

## Adding unit tests

Place tests under the matching module or shared area. Keep the production boundary visible in the test name using the existing pattern:

```text
<MethodOrUnit>_<StateOrScenario>_<ExpectedBehavior>
```

For a new module, mirror the module's behavior areas under `Modules/<ModuleName>/`, add the required production project references to `ResumeEnhancer.Tests.Unit.csproj`, and reuse the existing test infrastructure before adding a new helper.
