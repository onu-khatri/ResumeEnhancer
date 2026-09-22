using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

namespace ResumeEnhancer.WebSolution.ModulesComposition.Authorization;

public sealed class AuthorizationDependencyStartupValidator(IServiceScopeFactory scopeFactory)
{
    public void Validate()
    {
        using var scope = scopeFactory.CreateScope();
        _ = scope.ServiceProvider.GetRequiredService<IProfilingAuthorizationService>();
        _ = scope.ServiceProvider.GetRequiredService<IEntitlementResolver>();
    }
}
