using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.AuthModule.SL;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.Web.Outbox;
using Microsoft.Extensions.Configuration;
using ResumeEnhancer.AuthModule.SL.Options;
using ResumeEnhancer.AuthModule.Web.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ResumeEnhancer.AuthModule.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthModuleWeb(this IServiceCollection services, IConfiguration? configuration = null)
    {
        var securitySection = configuration?.GetSection(AuthSecurityOptions.SectionName);
        var options = securitySection?.Get<AuthSecurityOptions>() ?? new AuthSecurityOptions();
        var delayChildren = securitySection?
            .GetSection(nameof(AuthSecurityOptions.ProgressiveLoginDelays))
            .GetChildren()
            .OrderBy(child => child.Key, StringComparer.Ordinal)
            .ToArray();
        if (delayChildren is { Length: > 0 })
        {
            options.ProgressiveLoginDelays = delayChildren
                .Select(child => TimeSpan.Parse(child.Value ?? throw new InvalidOperationException(
                    $"Auth:Security:ProgressiveLoginDelays:{child.Key} is required."),
                    System.Globalization.CultureInfo.InvariantCulture))
                .ToArray();
        }
        AuthSecurityOptionsValidator.Validate(options, requirePersistedKeyRing: configuration is not null);
        if (configuration is not null)
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(options.DataProtectionKeyRingPath))
                .SetApplicationName(options.DataProtectionApplicationName);
        }
        services.AddAuthModuleApplication(options, requirePersistedKeyRing: configuration is not null);
        services.AddAuthentication("Bearer").AddScheme<AuthenticationSchemeOptions, AuthTokenAuthenticationHandler>("Bearer", _ => { });
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        services.TryAddScoped<IAuthSideEffectHandler, LoggingAuthSideEffectHandler>();
        services.TryAddScoped<IAuthChallengeDeliveryProtector, AuthChallengeDeliveryProtector>();
        services.AddHostedService<AuthOutboxDispatcher>();
        return services;
    }
}
