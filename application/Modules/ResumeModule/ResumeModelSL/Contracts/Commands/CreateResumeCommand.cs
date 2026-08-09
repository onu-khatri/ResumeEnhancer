using Mediator;
using ResumeModuleAM.Requests;
using ResumeModuleAM.Responses;

namespace ResumeModuleSL.Contracts;

public sealed record CreateResumeCommand(
    CreateResumeRequest Request,
    int? AuditUserId) : ICommand<ResumeDetailResponse>;
