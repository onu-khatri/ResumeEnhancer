using Shouldly;

namespace ResumeEnhancer.Tests.Infrastructure.Migration;

public sealed class MigrationCommandLineTests
{
    [Fact]
    public void Parse_HelpArgument_SetsShowHelpWithoutActions()
    {
        var options = MigrationCommandLine.Parse(["--help"]);

        options.ShowHelp.ShouldBeTrue();
        options.HasActions.ShouldBeFalse();
        options.DescribeActions().ShouldBe("none");
    }

    [Fact]
    public void Parse_AllActionsAndConnection_SetsOptions()
    {
        var options = MigrationCommandLine.Parse(
            ["--create=AddResume", "--apply", "--seeding", "--connection", "Server=test"]);

        options.CreateMigration.ShouldBeTrue();
        options.ApplyMigrations.ShouldBeTrue();
        options.SeedData.ShouldBeTrue();
        options.MigrationName.ShouldBe("AddResume");
        options.ConnectionString.ShouldBe("Server=test");
        options.DescribeActions().ShouldBe("create migration, apply migrations, seed data");
    }

    [Fact]
    public void Parse_CreateWithFollowingOption_LeavesMigrationNameNull()
    {
        var options = MigrationCommandLine.Parse(["-c", "-a"]);

        options.CreateMigration.ShouldBeTrue();
        options.ApplyMigrations.ShouldBeTrue();
        options.MigrationName.ShouldBeNull();
    }

    [Fact]
    public void Parse_NameOptionWithoutValue_ThrowsInvalidOperationException()
    {
        var exception = Should.Throw<InvalidOperationException>(
            () => MigrationCommandLine.Parse(["--name"]));

        exception.Message.ShouldContain("A value is required");
    }

    [Fact]
    public void Parse_UnknownArgument_ThrowsInvalidOperationException()
    {
        var exception = Should.Throw<InvalidOperationException>(
            () => MigrationCommandLine.Parse(["--wat"]));

        exception.Message.ShouldContain("Unknown argument");
    }
}
