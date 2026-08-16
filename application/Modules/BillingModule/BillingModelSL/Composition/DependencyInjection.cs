using Microsoft.Extensions.DependencyInjection;

namespace ResumeEnhancer.BillingModule.SL;

public static class DependencyInjection
{
    public static IServiceCollection AddBillingModuleApplication(this IServiceCollection services)
    {
        return services;
    }
}
