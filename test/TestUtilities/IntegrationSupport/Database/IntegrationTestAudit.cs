using Persistence;

namespace ResumeEnhancer.TestUtilities.IntegrationSupport;

public sealed record IntegrationTestAudit(int? UserId) : IAudit;
