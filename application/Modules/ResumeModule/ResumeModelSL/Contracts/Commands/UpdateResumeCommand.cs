using Mediator;
using ResumeModuleAM.Requests;
using ResumeModuleAM.Responses;

namespace ResumeModuleSL.Contracts;

public sealed record UpdateResumeCommand(
    int ResumeId,
    UpdateResumeRequest Request,
    int? AuditUserId,
    string? UserId = null) : ICommand<ResumeDetailResponse>;
