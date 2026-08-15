using Microsoft.EntityFrameworkCore;
using Shouldly;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.Infrastructure.Migration;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.Tests.Unit.Infrastructure.Migration;

public sealed class AppDbContextDesignTimeFactoryTests
{
    [Fact]
    public void CreateDbContext_ConnectionArgumentProvided_CreatesSqlServerContextWithResumeModel()
    {
        var factory = new AppDbContextDesignTimeFactory();

        using var dbContext = factory.CreateDbContext(
            ["--connection", "Server=(local);Database=ResumeEnhancerTests;Trusted_Connection=True;TrustServerCertificate=True"]);

        dbContext.ShouldBeOfType<AppDbContext>();
        dbContext.Database.ProviderName.ShouldBe("Microsoft.EntityFrameworkCore.SqlServer");
        dbContext.Model.FindEntityType(typeof(Resume)).ShouldNotBeNull();
    }

    [Fact]
    public void MigrationAssembly_AssemblyName_ReturnsMigrationAssemblyName()
    {
        MigrationAssembly.AssemblyName.ShouldBe("ResumeEnhancer.Infrastructure.Migration");
    }
}



