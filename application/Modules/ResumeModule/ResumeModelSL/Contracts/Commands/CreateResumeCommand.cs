using Mediator;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.AM.Responses;

namespace ResumeEnhancer.ResumeModule.SL.Contracts;

public sealed record CreateResumeCommand(
    CreateResumeRequest Request,
    int? AuditUserId) : ICommand<ResumeDetailResponse>;

