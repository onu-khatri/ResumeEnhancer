using ResumeEnhancer.ProfilingModule.DM.Entities;

namespace ResumeEnhancer.Tests.Unit.Modules.AuthModule;

public sealed class EntitlementIdentifierTests
{
    [Fact]
    public void Stable_capability_identifiers_are_public_and_distinct()
    {
        var identifiers = new[]
        {
            "TemplateView",
            "ResumePublish",
            "PdfExport",
            "ResumeCreate",
            "PremiumTemplates",
        };
        Assert.Equal(5, identifiers.Distinct(StringComparer.Ordinal).Count());
        Assert.Contains("ResumeCreate", identifiers);
        Assert.Contains("PremiumTemplates", identifiers);
    }

    [Fact]
    public void Role_persists_the_stable_capability_identifier()
    {
        var role = new Role { Code = "ResumeCreate", Capability = "ResumeCreate" };
        Assert.Equal("ResumeCreate", role.Capability);
    }
}
