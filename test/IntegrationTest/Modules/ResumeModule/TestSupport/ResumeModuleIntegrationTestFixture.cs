using ResumeEnhancer.TestUtilities.IntegrationSupport;

namespace ResumeEnhancer.IntegrationTests.Modules.ResumeModule;

public sealed class ResumeModuleIntegrationTestFixture : IDisposable
{
    public ResumeModuleIntegrationTestFixture()
    {
        Utilities = IntegrationTestUtilitiesBuilder.Get<global::Program>()
            .WithInMemoryDbContext()
            .WithFakeAuthentication()
            .WithMockedCacheProvider()
            .Build();
    }

    internal IntegrationTestUtilities<global::Program> Utilities { get; }

    internal ISetupper CreateSetupper() => Utilities.CreateSetupper();

    public void Dispose()
    {
        Utilities.Dispose();
    }
}
