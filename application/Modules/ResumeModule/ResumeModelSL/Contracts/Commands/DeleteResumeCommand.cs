using Mediator;
using ResumeModuleAM.Responses;

namespace ResumeModuleSL.Contracts;

public sealed record DeleteResumeCommand(
    int ResumeId,
    int? AuditUserId,
    string? UserId = null) : ICommand<ResumeDeleteResponse>;
