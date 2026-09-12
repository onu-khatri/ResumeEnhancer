using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.AuthModule.PL.Repositories;
using ResumeEnhancer.AuthModule.PL.Seeding;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.AuthModule.PL;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthModulePersistence(
        this IServiceCollection services,
        string? rootEntitySchema = null
    )
    {
        if (
            !services.Any(x =>
                x.ServiceType == typeof(IAppDbContextModelConfiguration)
                && x.ImplementationInstance is AuthModuleDbContextModelConfiguration
            )
        )
            services.AddSingleton<IAppDbContextModelConfiguration>(
                new AuthModuleDbContextModelConfiguration(rootEntitySchema)
            );
        services.TryAddEnumerable(
            ServiceDescriptor.Scoped<IAppDbContextSeeder, AuthModuleSeeder>()
        );
        services.TryAddScoped<IAuthRepository, AuthRepository>();
        services.TryAddSingleton<IPasswordHasher, AuthPasswordHasher>();
        services.TryAddSingleton<ITokenService, AuthTokenService>();
        services.TryAddSingleton<IRegistrationThrottle, InMemoryRegistrationThrottle>();
        return services;
    }
}
