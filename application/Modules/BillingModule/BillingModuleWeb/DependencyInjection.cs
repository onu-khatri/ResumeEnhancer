using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.BillingModule.SL;
using ResumeEnhancer.BillingModule.Web.Validation;

namespace ResumeEnhancer.BillingModule.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddBillingModuleWeb(this IServiceCollection services)
    {
        services.AddBillingModuleApplication();
        services.AddValidatorsFromAssemblyContaining<CreateBillingAccountRequestValidator>();

        return services;
    }

    public static Type[] GetBillingModuleMediatorAssemblies() =>
    [
        typeof(BillingModuleWebAssembly),
        typeof(BillingModuleSLAssembly)
    ];
}
