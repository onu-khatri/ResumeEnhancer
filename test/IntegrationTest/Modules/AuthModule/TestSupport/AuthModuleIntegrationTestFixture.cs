using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.AuthModule.PL;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.Tests.Integration.TestSupport;
using ResumeEnhancer.TestUtilities.IntegrationSupport;

namespace ResumeEnhancer.Tests.Integration.Modules.AuthModule.TestSupport;

[CollectionDefinition("Sequential_AuthModule", DisableParallelization = true)]
public sealed class AuthModuleCollection : ICollectionFixture<AuthModuleIntegrationTestFixture> { }

public sealed class AuthModuleIntegrationTestFixture
{
    private static readonly IntegrationTestUtilities<global::Program> SharedUtilities =
        IntegrationTestAssemblyFixture.Utilities;

    public AuthModuleIntegrationTestFixture()
    {
        Utilities = SharedUtilities;
    }

    internal IntegrationTestUtilities<global::Program> Utilities { get; }

    internal ISetupper CreateSetupper() => Utilities.CreateSetupper();

    internal async Task ResetAndSeedAsync(CancellationToken cancellationToken)
    {
        Utilities.ResetDatabase();
        (
            Utilities.Services.GetRequiredService<IRegistrationThrottle>()
            as InMemoryRegistrationThrottle
        )?.Reset();
        await Utilities.Services.SeedAppDbContextAsync(cancellationToken);
    }
}
