using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.BillingModule.PL;
using ResumeEnhancer.BillingModule.PL.Integrations;
using ResumeEnhancer.BillingModule.PL.Repositories;
using ResumeEnhancer.BillingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.BillingModule.SL.Integrations;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.Modules.BillingModule.Persistence;

public sealed class BillingPersistenceCompositionTests
{
    [Fact]
    public void AddBillingModulePersistence_RegistersOwnedServicesAndConfigurationOnce()
    {
        var services = new ServiceCollection();

        services.AddBillingModulePersistence("tenant");
        services.AddBillingModulePersistence("tenant");

        services.Count(descriptor => descriptor.ServiceType == typeof(IAppDbContextModelConfiguration))
            .ShouldBe(1);
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(IBillingRepository)
                                             && descriptor.ImplementationType == typeof(BillingRepository));
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(IBillingSetupDataRepository)
                                             && descriptor.ImplementationType == typeof(BillingSetupDataRepository));
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(IBillingRegistrationService)
                                             && descriptor.ImplementationType == typeof(BillingRegistrationService));
    }

    [Fact]
    public void BillingModelConfiguration_EnforcesBillingMappingContracts()
    {
        using var scope = new SqliteAppDbContextScope();
        var account = scope.DbContext.Model.FindEntityType(typeof(BillingAccount))!;
        var plan = scope.DbContext.Model.FindEntityType(typeof(BillingPlan))!;
        var subscription = scope.DbContext.Model.FindEntityType(typeof(BillingSubscription))!;

        account.GetTableName().ShouldBe("B_BillingAccount");
        account.FindProperty(nameof(BillingAccount.AccountNumber))!.IsNullable.ShouldBeFalse();
        account.FindIndex(new[] { account.FindProperty(nameof(BillingAccount.AccountNumber))! })!.IsUnique.ShouldBeTrue();
        plan.FindProperty(nameof(BillingPlan.AccessProfileId))!.IsNullable.ShouldBeFalse();
        subscription.GetForeignKeys().ShouldContain(foreignKey =>
            foreignKey.Properties.Select(property => property.Name).SequenceEqual(new[] { nameof(BillingSubscription.BillingPlanId) }));
    }
}
