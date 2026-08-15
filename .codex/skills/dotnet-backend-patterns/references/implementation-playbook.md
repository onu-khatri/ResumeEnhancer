# ResumeEnhancer Backend Implementation Playbook

This playbook describes the house style for backend work in this repository. It mirrors the real `<ModuleName>` implementation so new code stays consistent with the existing flow.

## Stack

- .NET 10, `Microsoft.NET.Sdk.Web` host (`WebSolution.Server`)
- **Mediator** (`Mediator.SourceGenerator`, `Mediator.Abstractions`) — not MediatR
- **Mapster** for object mapping
- **FluentValidation** for request validation
- **EF Core** + SQL Server with a shared `AppDbContext` and per-module schema
- **Scalar** for OpenAPI documentation

## Solution shape

```
application/
├── Core/
│   ├── CommonLibrary/          # exceptions, shared extensions
│   ├── DomainLibrary/          # domain base types (AuditEntity, BusinessEntity, SetupData)
│   └── WebLibrary/             # endpoint executor + HTTP helpers
├── Infrastructure/
│   ├── Caching/                # ICacheProvider / ICacheStrategy (InMemory, Redis, MemCache)
│   ├── Migration/              # EF design-time factory + migrations
│   └── Persistence/            # AppDbContext, UnitOfWork, repositories, querying, seeding
├── Modules/
│   └── <ModuleName>/
│       ├── <ModuleName>ModuleAM/     # request/response contracts
│       ├── <ModuleName>ModuleDM/     # domain entities + enums
│       ├── <ModuleName>ModuleSL/      # contracts, handlers, mapping, persistence abstractions
│       ├── <ModuleName>ModulePL/     # EF configurations, repositories, context, seeding
│       └── <ModuleName>ModuleWeb/    # Minimal APIs + validation
└── WebSolution/
    ├── ModulesComposition/     # host-facing module composition
    └── WebSolution.Server/     # entry point
```

### Dependency direction

- `<ModuleName>ModuleAM` depends on nothing (pure contracts).
- `<ModuleName>ModuleDM` depends only on `DomainLibrary`.
- `<ModuleName>ModuleSL` depends on `AM` + `DM` + Mapster + Mediator abstractions.
- `<ModuleName>ModulePL` depends on `Persistence` + `DM` + `SL` (for the persistence abstractions it implements).
- `<ModuleName>ModuleWeb` depends on `WebLibrary` + `AM` + `SL` (compile-time only, `PrivateAssets="all"`).
- `ModulesComposition` wires `PL` + `Web`; the host wires `ModulesComposition`, `Persistence`, and `Caching`.

## Request flow

`endpoint -> validator -> mediator.Send(command/query) -> handler -> mapper -> repository -> unit of work -> EF Core`

Each layer owns one concern and never skips a boundary.

## 1. Contracts (<ModuleName>ModuleAM)

Contracts are plain, serialization-safe DTOs with `[Required]`/`[MaxLength]`/`[Range]` annotations. Requests live under `Requests/<Area>`, responses under `Responses/<Area>`.

```csharp
namespace <ModuleName>ModuleAM.Requests;

public sealed class CreateResumeRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public List<EducationRequest> Education { get; set; } = [];
    // ... remaining sections
}
```

## 2. Minimal API endpoints (<ModuleName>ModuleWeb)

Endpoints are static extension classes grouped under `MiniApis/Commands` and `MiniApis/Queries`. The public surface is one `Map<ModuleName>ModuleApis` extension that maps a route group and delegates to command/query endpoints.

```csharp
public static class ResumeMinimalApis
{
    public static IEndpointRouteBuilder Map<ModuleName>ModuleApis(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/resumes").WithTags("Resumes");
        group.Map<ModuleName>CommandEndpoints();
        group.Map<ModuleName>QueryEndpoints();
        return endpoints;
    }
}
```

Each endpoint validates, then delegates to `Mediator` via `ApiEndpointExecutor.ValidateOrExecute`, which returns `Results.ValidationProblem` when validation fails and centralizes exception-to-status mapping (`KeyNotFoundException` -> 404, `UnauthorizedAccessException` -> 403, `ArgumentException`/`InvalidOperationException` -> 400).

```csharp
internal static partial class ResumeCommandEndpoints
{
    private static Task<IResult> CreateResumeAsync(
        CreateResumeRequest? request,
        IValidator<CreateResumeRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (request is null)
            return Task.FromResult(Results.ValidationProblem(ResumeEndpointValidation.BodyRequired()));

        return ValidateAndCreateAsync();

        async Task<IResult> ValidateAndCreateAsync()
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(
                validationResult.ToDictionary(),
                async () =>
                {
                    var response = await mediator.Send(
                        new CreateResumeCommand(request, ResumeEndpointHeaders.ReadAuditUserId(httpContext)),
                        cancellationToken);
                    return Results.Created($"/api/resumes/{response.Id}", response);
                });
        }
    }
}
```

## 3. Validation (<ModuleName>ModuleWeb/Validation)

Validators are one `AbstractValidator<TRequest>` per request, composed with `SetValidator` for nested objects and `RuleForEach` for collections. Reusable rule helpers live under `Validation/Shared`.

```csharp
public sealed class CreateResumeRequestValidator : AbstractValidator<CreateResumeRequest>
{
    public CreateResumeRequestValidator()
    {
        RuleFor(r => r.Title).NotEmpty().MaximumLength(200);
        RuleFor(r => r.UserId).NotEmpty().MaximumLength(450);
        When(r => r.PersonalInformation is not null, () =>
            RuleFor(r => r.PersonalInformation!).SetValidator(new PersonalInformationRequestValidator(isCreate: true)));
        RuleForEach(r => r.Education).SetValidator(new EducationRequestValidator(isCreate: true));
    }
}
```

## 4. Mediator contracts and handlers (<ModuleName>ModuleSL)

Contracts are positional records implementing `ICommand<TResponse>` or `IQuery<TResponse>`. Handlers implement `ICommandHandler<T, TResponse>` / `IQueryHandler<T, TResponse>` and return `ValueTask<TResponse>`.

```csharp
// Contracts/Commands/CreateResumeCommand.cs
public sealed record CreateResumeCommand(
    CreateResumeRequest Request,
    int? AuditUserId) : ICommand<ResumeDetailResponse>;

// Handlers/Commands/CreateResumeCommandHandler.cs
public sealed class CreateResumeCommandHandler
    : ICommandHandler<CreateResumeCommand, ResumeDetailResponse>
{
    private readonly IResumeRepository _resumeRepository;

    public CreateResumeCommandHandler(IResumeRepository resumeRepository) =>
        _resumeRepository = resumeRepository;

    public async ValueTask<ResumeDetailResponse> Handle(
        CreateResumeCommand request,
        CancellationToken cancellationToken = default)
    {
        var resume = ResumeModelMapper.CreateResume(request.Request);
        var savedResume = await _resumeRepository.AddAsync(resume, request.AuditUserId, cancellationToken);
        return ResumeModelMapper.MapDetail(savedResume);
    }
}
```

Register Mediator once in the Web layer, scoping it to the SL assembly:

```csharp
services.AddMediator(options =>
{
    options.Assemblies = [typeof(CreateResumeCommand)];
    options.ServiceLifetime = ServiceLifetime.Scoped;
});
```

## 5. Mapping (<ModuleName>ModuleSL/Mapping)

Mapster is configured in a `static partial class ResumeModelMapper` split across focused files (`Create`, `Update`, `Responses`, `Search`, `Mapster`). A single `TypeAdapterConfig` holds the mappings; navigation/relational properties are always `.Ignore()`d and set explicitly in the `Create`/`Update` methods.

```csharp
config.NewConfig<CreateResumeRequest, Resume>()
    .Map(dest => dest.Title, src => NormalizeRequired(src.Title))
    .Ignore(dest => dest.PersonalInformation)
    .Ignore(dest => dest.Education);

var resume = request.Adapt<Resume>(MapsterConfig);
```

Keep normalization (`NormalizeRequired`/`NormalizeOptional`) and child graph construction explicit rather than letting Mapster silently decide.

## 6. Repository (SL abstraction + PL implementation)

The abstraction lives in `<ModuleName>ModuleSL/Abstractions/Persistence`; the EF implementation lives in `<ModuleName>ModulePL/Repositories`. The implementation wraps `IUnitOfWork<AppDbContext>` and uses `GetRepo<Resume>()` rather than injecting a `DbSet` directly.

```csharp
public interface IResumeRepository
{
    Task<Resume> AddAsync(Resume resume, int? auditUserId, CancellationToken cancellationToken = default);
    Task<Resume?> GetAsync(int resumeId, string? userId = null, bool track = false, CancellationToken cancellationToken = default);
    Task<ResumeSearchResult> SearchAsync(ResumeSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<ResumeDeleteResult> DeleteAsync(IReadOnlyList<int> resumeIds, int? auditUserId, string? userId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int resumeId, string? userId = null, CancellationToken cancellationToken = default);
}

public sealed class ResumeRepository : IResumeRepository
{
    private readonly IUnitOfWork<AppDbContext> _unitOfWork;

    public ResumeRepository(IUnitOfWork<AppDbContext> unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Resume> AddAsync(Resume resume, int? auditUserId, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(resume);
        await _unitOfWork.GetRepo<Resume>().AddAsync(resume, ct);
        await _unitOfWork.SaveAsync(new RepositoryAudit(auditUserId), ct);
        return resume;
    }
}
```

## 7. Composition and DI

Each module exposes a `DependencyInjection` static class. `ModulesComposition` is the only place that wires modules together, and the host calls that composition root.

```csharp
// ModulesComposition/DependencyInjection.cs
public static IServiceCollection AddApplicationModules(this IServiceCollection services)
{
    services.Add<ModuleName>ModulePersistence();
    services.Add<ModuleName>ModuleWeb();
    return services;
}
```

```csharp
// WebSolution.Server/Program.cs
builder.Services.AddApplicationCaching(builder.Configuration);
builder.Services.AddAppDbContext((_, options) => options.UseSqlServer(GetConnectionString(builder)));
builder.Services.AddApplicationModules();
```

## 8. Testing

Unit tests target `ResumeEnhancer.Tests`; integration tests target `ResumeEnhancer.IntegrationTests`. Modules expose internals via `InternalsVisibleTo("ResumeEnhancer.Tests")`.

## Definition of Done

- `dotnet build application\ResumeEnhancerApp.slnx` passes.
- Unit tests pass: `dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.csproj --no-restore`.
- Integration tests pass when contracts/persistence change: `dotnet test test\IntegrationTest\ResumeEnhancer.IntegrationTests.csproj --no-restore`.