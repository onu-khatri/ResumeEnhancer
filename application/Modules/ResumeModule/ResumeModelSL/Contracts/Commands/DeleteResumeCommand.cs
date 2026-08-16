using Mediator;
using ResumeEnhancer.ResumeModule.AM.Responses;

namespace ResumeEnhancer.ResumeModule.SL.Contracts;

public sealed record DeleteResumeCommand(
    int ResumeId,
    int? AuditUserId,
    int? UserId = null) : ICommand<ResumeDeleteResponse>;

