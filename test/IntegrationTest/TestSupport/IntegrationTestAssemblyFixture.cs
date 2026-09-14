using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.AuthModule.PL;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.TestUtilities.IntegrationSupport;

namespace ResumeEnhancer.Tests.Integration.TestSupport;

[CollectionDefinition("Sequential_Integration", DisableParallelization = true)]
public sealed class IntegrationTestCollection : ICollectionFixture<IntegrationTestAssemblyFixture>
{
}

public sealed class IntegrationTestAssemblyFixture : IDisposable
{
    public IntegrationTestAssemblyFixture()
    {
        Utilities = IntegrationTestUtilitiesBuilder
            .Get<global::Program>()
            .WithInMemoryDbContext()
            .WithFakeAuthentication()
            .WithMockedCacheProvider()
            .WithConfigureServices(services =>
            {
                services.RemoveAll<IConfiguration>();
                services.AddSingleton<IConfiguration>(
                    new ConfigurationBuilder()
                        .AddInMemoryCollection(
                            new Dictionary<string, string?>
                            {
                                ["Auth:SigningKey"] = "integration-test-signing-key-0123456789",
                            }
                        )
                        .Build()
                );
            })
            .Build();
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
        Utilities.ClearAuthentication();
        await Utilities.Services.SeedAppDbContextAsync(cancellationToken);
    }

    public void Dispose() => Utilities.Dispose();
}
