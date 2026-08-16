using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.BillingModule.SL;
using ResumeEnhancer.BillingModule.SL.Contracts;
using ResumeEnhancer.BillingModule.Web.Validation;

namespace ResumeEnhancer.BillingModule.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddBillingModuleWeb(this IServiceCollection services)
    {
        services.AddBillingModuleApplication();
        services.AddMediator(options =>
        {
            options.Assemblies = [typeof(CreateBillingAccountCommand)];
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        services.AddValidatorsFromAssemblyContaining<CreateBillingAccountRequestValidator>();

        return services;
    }
}
