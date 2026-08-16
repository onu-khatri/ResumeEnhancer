using Mediator;

namespace ResumeEnhancer.ResumeModule.SL.Contracts;

public sealed record ResumeExistsQuery(
    int ResumeId,
    int? UserId = null) : IQuery<bool>;

