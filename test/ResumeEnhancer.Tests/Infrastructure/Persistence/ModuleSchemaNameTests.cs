using Shouldly;
using Persistence;

namespace ResumeEnhancer.Tests.Infrastructure.Persistence;

public sealed class ModuleSchemaNameTests
{
    [Fact]
    public void FromModule_NameHasOuterWhitespace_ReturnsTrimmedName()
    {
        var schema = ModuleSchemaName.FromModule(" resume ");

        schema.ShouldBe("resume");
    }

    [Fact]
    public void FromRootAndSupportingModule_ValidNames_JoinsWithUnderscore()
    {
        var schema = ModuleSchemaName.FromRootAndSupportingModule("core", "resume");

        schema.ShouldBe("core_resume");
    }

    [Fact]
    public void FromModule_NameIsWhitespace_ThrowsArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() => ModuleSchemaName.FromModule(" "));

        exception.ParamName.ShouldBe("moduleSchema");
    }

    [Fact]
    public void FromModule_NameStartsWithNumber_ThrowsArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() => ModuleSchemaName.FromModule("1resume"));

        exception.Message.ShouldContain("start with a letter or underscore");
    }

    [Fact]
    public void FromModule_NameContainsHyphen_ThrowsArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() => ModuleSchemaName.FromModule("resume-module"));

        exception.Message.ShouldContain("only letters, numbers, and underscores");
    }
}
