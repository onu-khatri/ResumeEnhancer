using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.AuthModule.SL;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.Web.Outbox;

namespace ResumeEnhancer.AuthModule.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthModuleWeb(this IServiceCollection services)
    {
        services.AddAuthModuleApplication();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        services.TryAddScoped<IAuthSideEffectHandler, LoggingAuthSideEffectHandler>();
        services.AddHostedService<AuthOutboxDispatcher>();
        return services;
    }
}
