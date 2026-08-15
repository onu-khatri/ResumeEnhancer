using Mediator;

namespace ResumeEnhancer.ResumeModule.SL.Contracts;

public sealed record ResumeExistsQuery(
    int ResumeId,
    string? UserId = null) : IQuery<bool>;

