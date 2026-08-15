// Service-Layer (SL) Composition Template (ResumeEnhancer)
//
// In this repository the SL layer is a Mediator + Mapster slice, not a classic
// "service" class. This template shows the three pieces a new feature slice
// needs in <ModuleName>ModuleSL: the contract, the handler, and its mapping.
//
//   - Contract  : <ModuleName>ModuleSL/Contracts/<Commands|Queries>/<Name>.cs
//   - Handler   : <ModuleName>ModuleSL/Handlers/<Commands|Queries>/<Name>Handler.cs
//   - Mapping   : <ModuleName>ModuleSL/Mapping/ResumeModelMapper.<Area>.cs (partial class)

using Mapster;
using Mediator;
using <ModuleName>ModuleAM.Requests;
using <ModuleName>ModuleAM.Responses;
using <ModuleName>ModuleDM.Entities;
using <ModuleName>ModuleSL.Abstractions.Persistence;
using <ModuleName>ModuleSL.Contracts;

namespace <ModuleName>ModuleSL.Handlers;

// 1. Contract — a positional record with the request plus routing metadata.
public sealed record CreateResumeCommand(
    CreateResumeRequest Request,
    int? AuditUserId) : ICommand<ResumeDetailResponse>;

// 2. Handler — one business job, constructor-injected dependencies, ValueTask return.
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

// 3. Mapping — a static partial class holding a single TypeAdapterConfig.
internal static partial class ResumeModelMapper
{
    private static readonly TypeAdapterConfig MapsterConfig = CreateMapsterConfig();

    private static TypeAdapterConfig CreateMapsterConfig()
    {
        var config = new TypeAdapterConfig();

        config.NewConfig<CreateResumeRequest, Resume>()
            .Map(dest => dest.Title, src => NormalizeRequired(src.Title))
            .Map(dest => dest.Summary, src => NormalizeOptional(src.Summary))
            .Ignore(dest => dest.PersonalInformation)
            .Ignore(dest => dest.Education)
            .Ignore(dest => dest.Certifications);

        return config;
    }

    public static Resume CreateResume(CreateResumeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var resume = request.Adapt<Resume>(MapsterConfig);

        // Build the child graph explicitly; never rely on Mapster for navigation properties.
        AddCreatedChildren(resume.Education, request.Education, item => CreateEducation(item, resume));

        return resume;
    }

    private static Education CreateEducation(EducationRequest request, Resume resume)
    {
        var education = new Education { Resume = resume };
        // ApplyEducation(education, request);
        return education;
    }

    private static void AddCreatedChildren<TRequest, TEntity>(
        ICollection<TEntity> target,
        IReadOnlyCollection<TRequest> source,
        Func<TRequest, TEntity> create)
    {
        foreach (var item in source)
        {
            target.Add(create(item));
        }
    }

    private static string NormalizeRequired(string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
