using Persistence;

namespace ResumeEnhancer.Tests.TestInfrastructure;

internal sealed record TestAudit(int? UserId) : IAudit;
