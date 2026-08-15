using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.WebSolution.ModulesComposition;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.PL;
using ResumeEnhancer.ResumeModule.PL.Repositories;
using ResumeEnhancer.ResumeModule.SL;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ResumeModule.Web;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.Composition;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddResumeModuleApplication_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();

        var returned = services.AddResumeModuleApplication();

        ReferenceEquals(services, returned).ShouldBeTrue();
    }

    [Fact]
    public void AddResumeModulePersistence_RegistersModelConfigurationSeederAndRepositoryOnce()
    {
        var services = new ServiceCollection();

        services.AddResumeModulePersistence("tenant");
        services.AddResumeModulePersistence("tenant");

        services.Count(descriptor => descriptor.ServiceType == typeof(IAppDbContextModelConfiguration))
            .ShouldBe(1);
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(IAppDbContextSeeder));
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(IResumeRepository)
                                             && descriptor.ImplementationType == typeof(ResumeRepository));
    }

    [Fact]
    public void AddResumeModuleWeb_RegistersValidatorAndMediator()
    {
        var services = new ServiceCollection();

        services.AddResumeModuleWeb();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IValidator<CreateResumeRequest>>().ShouldNotBeNull();
        provider.GetRequiredService<IMediator>().ShouldNotBeNull();
    }

    [Fact]
    public void AddAppDbContext_RegistersUnitOfWorkRepositoryAndModelLoader()
    {
        var services = new ServiceCollection();

        services.AddAppDbContext((_, options) => options.UseSqlite("Data Source=:memory:"));
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        scope.ServiceProvider.GetRequiredService<AppDbContext>().ShouldNotBeNull();
        scope.ServiceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>().ShouldNotBeNull();
        scope.ServiceProvider.GetRequiredService<IUnitOfWorkFactory<AppDbContext>>().ShouldNotBeNull();
        scope.ServiceProvider.GetRequiredService<IAuditEntityRepository<ResumeEnhancer.ResumeModule.DM.Entities.Resume>>()
            .ShouldNotBeNull();
        scope.ServiceProvider.GetRequiredService<IModelLoader<ResumeEnhancer.ResumeModule.DM.Entities.Resume>>()
            .ShouldNotBeNull();
    }

    [Fact]
    public void AddApplicationModules_RegistersResumeModuleServices()
    {
        var services = new ServiceCollection();

        services.AddApplicationModules();

        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(IResumeRepository));
        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(IValidator<CreateResumeRequest>));
    }

    [Fact]
    public void MapApplicationModuleApis_RegistersResumeEndpoints()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.Services.AddApplicationModules();
        var app = builder.Build();

        app.MapApplicationModuleApis();
        var endpointNames = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .OfType<RouteEndpoint>()
            .Select(endpoint => endpoint.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName)
            .Where(name => name is not null)
            .ToArray();

        endpointNames.ShouldBe(
            ["CreateResume", "UpdateResume", "DeleteResume", "DeleteResumes", "GetResume", "SearchResumes", "ResumeExists"],
            ignoreOrder: true);
    }
}



