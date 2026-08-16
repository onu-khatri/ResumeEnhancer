using Mediator;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.AM.Responses;

namespace ResumeEnhancer.ResumeModule.SL.Contracts;

public sealed record UpdateResumeCommand(
    int ResumeId,
    UpdateResumeRequest Request,
    int? AuditUserId,
    int? UserId = null) : ICommand<ResumeDetailResponse>;

