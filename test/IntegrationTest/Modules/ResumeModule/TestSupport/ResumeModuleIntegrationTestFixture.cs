using ResumeEnhancer.Tests.Integration.TestSupport;
using ResumeEnhancer.TestUtilities.IntegrationSupport;

namespace ResumeEnhancer.Tests.Integration.Modules.ResumeModule;

[CollectionDefinition("Sequential_ResumeModule", DisableParallelization = true)]
public sealed class ResumeModuleCollection : ICollectionFixture<ResumeModuleIntegrationTestFixture>;

public sealed class ResumeModuleIntegrationTestFixture
{
    public ResumeModuleIntegrationTestFixture()
    {
        Utilities = IntegrationTestAssemblyFixture.Utilities;
    }

    internal IntegrationTestUtilities<global::Program> Utilities { get; }

    internal ISetupper CreateSetupper() => Utilities.CreateSetupper();
}
