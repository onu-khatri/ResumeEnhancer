using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.Tests.Integration.TestSupport;
using ResumeEnhancer.TestUtilities.IntegrationSupport;

namespace ResumeEnhancer.Tests.Integration.Modules.BillingModule.TestSupport;

[CollectionDefinition("Sequential_BillingModule", DisableParallelization = true)]
public sealed class BillingModuleCollection : ICollectionFixture<BillingModuleIntegrationTestFixture>;

public sealed class BillingModuleIntegrationTestFixture : IDisposable
{
    public BillingModuleIntegrationTestFixture() => Utilities = IntegrationTestAssemblyFixture.CreateUtilities();

    internal IntegrationTestUtilities<global::Program> Utilities { get; }

    internal ISetupper CreateSetupper() => Utilities.CreateSetupper();

    internal async Task ResetAndSeedAsync(CancellationToken cancellationToken)
    {
        Utilities.ResetDatabase();
        Utilities.ClearAuthentication();
        await Utilities.Services.SeedAppDbContextAsync(cancellationToken);
    }

    public void Dispose() => Utilities.Dispose();
}
