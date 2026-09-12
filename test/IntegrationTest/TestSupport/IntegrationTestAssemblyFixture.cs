using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.TestUtilities.IntegrationSupport;

namespace ResumeEnhancer.Tests.Integration.TestSupport;

internal static class IntegrationTestAssemblyFixture
{
    internal static IntegrationTestUtilities<global::Program> CreateUtilities() =>
        IntegrationTestUtilitiesBuilder
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
