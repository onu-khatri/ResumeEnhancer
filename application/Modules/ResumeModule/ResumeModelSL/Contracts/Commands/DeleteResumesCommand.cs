using Mediator;
using ResumeEnhancer.ResumeModule.AM.Responses;

namespace ResumeEnhancer.ResumeModule.SL.Contracts;

public sealed record DeleteResumesCommand(
    IReadOnlyList<int> ResumeIds,
    int? AuditUserId,
    string? UserId = null) : ICommand<ResumeDeleteResponse>;

