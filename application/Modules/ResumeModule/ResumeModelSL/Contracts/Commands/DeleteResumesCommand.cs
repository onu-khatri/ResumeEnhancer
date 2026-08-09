using Mediator;
using ResumeModuleAM.Responses;

namespace ResumeModuleSL.Contracts;

public sealed record DeleteResumesCommand(
    IReadOnlyList<int> ResumeIds,
    int? AuditUserId,
    string? UserId = null) : ICommand<ResumeDeleteResponse>;
