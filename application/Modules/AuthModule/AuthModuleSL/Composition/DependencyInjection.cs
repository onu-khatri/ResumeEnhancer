using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Services;
using ResumeEnhancer.AuthModule.SL.Options;

namespace ResumeEnhancer.AuthModule.SL;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthModuleApplication(this IServiceCollection services, AuthSecurityOptions? options = null, bool requirePersistedKeyRing = false)
    {
        options ??= new AuthSecurityOptions();
        AuthSecurityOptionsValidator.Validate(options, requirePersistedKeyRing);
        services.TryAddSingleton<TimeProvider>(_ => TimeProvider.System);
        services.TryAddSingleton(options);
        services.AddSingleton<IPasswordHasher, AuthPasswordHasher>();
        services.TryAddScoped<IAuthSigningKeyProvider, AuthSigningKeyProvider>();
        services.TryAddScoped<AuthSigningKeyStartupValidator>();
        services.AddScoped<ITokenService>(serviceProvider => new AuthTokenService(
            serviceProvider.GetRequiredService<AuthSecurityOptions>(),
            serviceProvider.GetRequiredService<IAuthSigningKeyProvider>(),
            serviceProvider.GetRequiredService<TimeProvider>()));
        services.TryAddSingleton<IAuthAuditFailureSignal, AuthAuditFailureSignal>();
        services.TryAddScoped<IAuthAuditRecorder, AuthAuditRecorder>();
        services.TryAddScoped<IRegistrationService, RegistrationService>();
        services.TryAddScoped<IAuthLifecycleService, AuthLifecycleService>();
        services.TryAddScoped<IAuthChallengeFactory, AuthChallengeFactory>();
        services.TryAddScoped<IAuthCurrentStateService, AuthCurrentStateService>();
        services.TryAddScoped<IEntitlementResolver, SubscriptionEntitlementResolver>();
        services.TryAddScoped<IAuthSecurityStateService, AuthSecurityStateService>();
        services.TryAddSingleton<IProgressiveLoginDelayPolicy, ProgressiveLoginDelayPolicy>();
        return services;
    }
}
