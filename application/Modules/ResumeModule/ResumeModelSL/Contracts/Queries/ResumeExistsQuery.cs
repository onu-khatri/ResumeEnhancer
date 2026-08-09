using Mediator;

namespace ResumeModuleSL.Contracts;

public sealed record ResumeExistsQuery(
    int ResumeId,
    string? UserId = null) : IQuery<bool>;
