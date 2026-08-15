using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.Tests.Unit.TestInfrastructure;

internal sealed record TestAudit(int? UserId) : IAudit;


