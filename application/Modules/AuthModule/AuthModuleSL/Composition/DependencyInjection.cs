using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Services;

namespace ResumeEnhancer.AuthModule.SL;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthModuleApplication(this IServiceCollection services)
    {
        services.TryAddScoped<IRegistrationService, RegistrationService>();
        services.TryAddScoped<IEntitlementResolver, SubscriptionEntitlementResolver>();
        return services;
    }
}
