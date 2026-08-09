# ResumeEnhancer Test Guide

This folder contains the xUnit.net test suite for ResumeEnhancer. The suite is organized to keep tests fast, isolated, readable, and useful during refactoring.

## Project Structure

```text
test/
|-- Directory.Build.props
|-- coverlet.runsettings
|-- .gitignore
|-- README.md
`-- ResumeEnhancer.Tests/
    |-- Composition/
    |-- Core/
    |   |-- DomainLibrary/
    |   `-- WebLibrary/
    |-- Infrastructure/
    |   |-- Caching/
    |   |-- Migration/
    |   `-- Persistence/
    |-- Modules/
    |   `-- ResumeModule/
    |       |-- Application/
    |       |-- ApplicationModel/
    |       |-- DomainModel/
    |       |-- Persistence/
    |       `-- Web/
    |           `-- Validation/
    |-- TestInfrastructure/
    |-- ResumeEnhancer.Tests.csproj
    `-- xunit.runner.json
```

## How To Run

Fast local test run:

```powershell
dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.csproj --no-restore
```

Run with coverage:

```powershell
dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.csproj --no-restore --settings test\coverlet.runsettings --collect:"XPlat Code Coverage"
```

Validate the whole solution:

```powershell
dotnet build application\ResumeEnhancerApp.slnx --no-restore
```

## Standards

- Use xUnit.net `[Fact]` tests by default.
- Follow Arrange, Act, Assert.
- Name tests as `MethodOrUnit_StateOrScenario_ExpectedBehavior`.
- Keep each test focused on one behavior.
- Use `Shouldly` for readable assertions.
- Use `NSubstitute` for mocks and spies.
- Pass `TestContext.Current.CancellationToken` to async APIs.
- Prefer deterministic test data from `TestInfrastructure` helpers.
- Avoid sleeps, wall-clock assumptions, shared mutable state, and external services.
- Use SQLite in-memory for EF Core repository tests when relational behavior matters.
- Do not use EF Core InMemory for relational behavior such as constraints, includes, transactions, or SQL translation.
- Keep tests parallel-safe. Do not rely on execution order.
- Do not assert implementation details unless the behavior is part of the module contract.

## Coverage Rules

Coverage should guide quality, not replace review. Try to cover line, branch, edge cases, failures, cancellation, and null/invalid inputs where they represent real behavior.

Use `[ExcludeFromCodeCoverage]` only for code that should not be unit-tested directly:

- model-only DTO/entity assemblies
- generated code
- EF migration/designer files
- application host bootstrapping such as `Program.cs`
- trivial constants or marker types with no behavior

Do not use coverage exclusions to hide missing behavior in services, handlers, validators, repositories, mappers, or composition code.

The AM and DM model projects are excluded at assembly level because they contain request/response DTOs, enums, and EF/domain model classes. Their behavior is covered through validators, mappers, handlers, repositories, and endpoint tests.

Example assembly-level exclusion:

```csharp
using System.Diagnostics.CodeAnalysis;

[assembly: ExcludeFromCodeCoverage]
```

## Adding Tests For A New Module

Mirror the production module shape under `test/ResumeEnhancer.Tests/Modules/<ModuleName>/`.

Recommended folders:

```text
Modules/<ModuleName>/
|-- Application/
|-- ApplicationModel/
|-- DomainModel/
|-- Persistence/
`-- Web/
    `-- Validation/
```

Add project references in `ResumeEnhancer.Tests.csproj` for each new production project:

```xml
<ProjectReference Include="..\..\application\Modules\MyModule\MyModuleSL\MyModuleSL.csproj" />
<ProjectReference Include="..\..\application\Modules\MyModule\MyModulePL\MyModulePL.csproj" />
<ProjectReference Include="..\..\application\Modules\MyModule\MyModuleWeb\MyModuleWeb.csproj" />
```

If a module has internal behavior that should be tested, add `InternalsVisibleTo` to the production project:

```xml
<ItemGroup>
  <InternalsVisibleTo Include="ResumeEnhancer.Tests" />
</ItemGroup>
```

## Test Patterns

Handler test pattern:

```csharp
[Fact]
public async Task CreateItemCommandHandler_ValidCommand_AddsItemAndMapsResponse()
{
    var cancellationToken = TestContext.Current.CancellationToken;
    var repository = Substitute.For<IItemRepository>();
    repository.AddAsync(Arg.Any<Item>(), 42, cancellationToken)
        .Returns(call => Task.FromResult(call.Arg<Item>()));
    var handler = new CreateItemCommandHandler(repository);

    var response = await handler.Handle(
        new CreateItemCommand(new CreateItemRequest { Name = " Item " }, 42),
        cancellationToken);

    response.Name.ShouldBe("Item");
    await repository.Received(1).AddAsync(
        Arg.Is<Item>(item => item != null && item.Name == "Item"),
        42,
        cancellationToken);
}
```

Validator test pattern:

```csharp
[Fact]
public async Task CreateItemRequestValidator_NameMissing_ReturnsValidationError()
{
    var validator = new CreateItemRequestValidator();

    var result = await validator.ValidateAsync(
        new CreateItemRequest { Name = "" },
        TestContext.Current.CancellationToken);

    result.ShouldHaveError("Name");
}
```

Mapper test pattern:

```csharp
[Fact]
public void ItemModelMapper_CreateItem_NormalizesValuesAndBuildsGraph()
{
    var request = new CreateItemRequest
    {
        Name = " Item ",
        Description = " Description "
    };

    var item = ItemModelMapper.CreateItem(request);

    item.Name.ShouldBe("Item");
    item.Description.ShouldBe("Description");
}
```

Repository test pattern with SQLite:

```csharp
[Fact]
public async Task ItemRepository_SearchAsync_FilterCombination_ReturnsExpectedPage()
{
    using var scope = new SqliteAppDbContextScope();
    var cancellationToken = TestContext.Current.CancellationToken;
    var repository = new ItemRepository(scope.UnitOfWork);
    scope.DbContext.Add(new Item { Name = "Alpha", UserId = "user-1" });
    await scope.DbContext.SaveChangesAsync(new TestAudit(1), cancellationToken);
    scope.DbContext.ChangeTracker.Clear();

    var result = await repository.SearchAsync(
        new ItemSearchCriteria
        {
            UserId = " user-1 ",
            PageNumber = 1,
            PageSize = 10
        },
        cancellationToken);

    result.TotalCount.ShouldBe(1);
    result.Items.ShouldHaveSingleItem().Name.ShouldBe("Alpha");
}
```

Endpoint delegate test pattern:

```csharp
[Fact]
public async Task CreateItemAsync_ValidRequest_SendsCommandAndReturnsCreated()
{
    var cancellationToken = TestContext.Current.CancellationToken;
    var mediator = Substitute.For<IMediator>();
    mediator.Send(Arg.Any<ICommand<ItemDetailResponse>>(), cancellationToken)
        .Returns(new ValueTask<ItemDetailResponse>(
            new ItemDetailResponse { Id = 10, Name = "Created" }));
    var httpContext = new DefaultHttpContext();
    httpContext.Request.Headers["X-Audit-UserId"] = "42";

    var result = await InvokeEndpointAsync(
        "CreateItemAsync",
        new CreateItemRequest { Name = "Created" },
        new InlineValidator<CreateItemRequest>(),
        mediator,
        httpContext,
        cancellationToken);

    var snapshot = await result.ExecuteAsync();
    snapshot.StatusCode.ShouldBe(StatusCodes.Status201Created);
}
```

## Junior Developer Notes

- Start with the public behavior. Ask: what should this class promise to callers?
- Write the happy-path test first, then add edge cases.
- For every validation rule, test one valid request and one invalid request.
- For every handler, test repository calls and mapped response.
- For every update mapper, test new, existing, removed, and invalid child IDs.
- For every repository search, test filters, sorting, pagination, empty ids, invalid ids, and date ranges.
- For every delete operation, test allowed, forbidden, missing, duplicate, empty, and invalid ids.
- Keep builders small. Add helper methods only when duplication starts hiding the behavior under test.
- Prefer assertions that explain intent: `ShouldBe`, `ShouldBeNull`, `ShouldHaveSingleItem`, `ShouldThrowAsync`.
- Do not test auto-properties on DTOs. Test the behavior that consumes those DTOs.
- If a test requires a network/database/service, first ask whether a fake, substitute, or in-memory relational provider can prove the same behavior.

## Current Test Libraries

- xUnit.net v3 for test framework and `[Fact]`.
- Shouldly for assertions.
- NSubstitute for test doubles.
- FluentValidation test patterns through validation result assertions.
- Microsoft.EntityFrameworkCore.Sqlite for relational in-memory persistence tests.
- coverlet collector for coverage.

## Coverage Summary

Generated: 2026-08-09 17:03:54 +05:30

Latest coverage run:

- Line coverage: 96.47% (`2546/2639`)
- Branch coverage: 83.06% (`471/567`)
- Test count: 213 passing
- Coverage settings: `test/coverlet.runsettings`
- AM and DM model assemblies: excluded with `[ExcludeFromCodeCoverage]`
