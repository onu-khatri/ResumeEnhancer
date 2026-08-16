using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.TemplateModule.DM.Entities;
using ResumeEnhancer.TemplateModule.PL.Repositories;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;

namespace ResumeEnhancer.Tests.Unit.Modules.TemplateModule.Persistence;

public sealed class TemplateRepositoryTests
{
    [Fact]
    public async Task TemplateRepository_CoversCrudAndExistsChecks()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var cacheProvider = CreateCacheProvider();
        var repository = new TemplateRepository(scope.UnitOfWork, cacheProvider);
        var category = new TemplateCategory { Code = "MODERN", Description = "Modern templates", DisplayName = "Modern", Guid = Guid.NewGuid() };
        var renderType = await scope.DbContext.Set<TemplateRenderTypeSetup>().SingleAsync(cancellationToken);

        var addedCategory = await repository.AddTemplateCategoryAsync(category, 77, cancellationToken);
        var addedTemplate = await repository.AddTemplateAsync(
            new Template
            {
                Code = "MODERN_HTML",
                Description = "Template",
                DisplayName = "Modern Html",
                TemplateCategoryId = addedCategory.Id,
                RenderTypeId = renderType.Id,
                Body = "<html />",
                Guid = Guid.NewGuid()
            },
            77,
            cancellationToken);

        scope.DbContext.ChangeTracker.Clear();

        (await repository.GetTemplateCategoryAsync(addedCategory.Id, false, cancellationToken))!.Code.ShouldBe("MODERN");
        (await repository.GetTemplateAsync(addedTemplate.Id, false, cancellationToken))!.RenderType.ShouldNotBeNull();
        (await repository.ListTemplateCategoriesAsync(cancellationToken)).ShouldContain(item => item.Id == addedCategory.Id);
        (await repository.ListTemplatesAsync(cancellationToken)).ShouldContain(item => item.Id == addedTemplate.Id);
        (await repository.TemplateCategoryExistsAsync(addedCategory.Id, cancellationToken)).ShouldBeTrue();
        (await repository.TemplateExistsAsync(addedTemplate.Id, cancellationToken)).ShouldBeTrue();

        await repository.DeleteTemplateAsync((await repository.GetTemplateAsync(addedTemplate.Id, true, cancellationToken))!, 88, cancellationToken);
        await repository.DeleteTemplateCategoryAsync((await repository.GetTemplateCategoryAsync(addedCategory.Id, true, cancellationToken))!, 88, cancellationToken);

        (await scope.DbContext.Set<Template>().AnyAsync(item => item.Id == addedTemplate.Id, cancellationToken)).ShouldBeFalse();
        (await scope.DbContext.Set<TemplateCategory>().AnyAsync(item => item.Id == addedCategory.Id, cancellationToken)).ShouldBeFalse();
        await cacheProvider.Received().RemoveAsync("template:setup:categories", cancellationToken);
        await cacheProvider.Received().RemoveAsync("template:setup:render-types", cancellationToken);
    }

    [Fact]
    public async Task TemplateSetupDataRepository_ListsCategoriesAndRenderTypesThroughCacheFactory()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var cacheProvider = Substitute.For<ICacheProvider>();
        cacheProvider
            .GetOrSetAsync<IReadOnlyList<TemplateCategory>>(
                "template:setup:categories",
                Arg.Any<Func<CancellationToken, Task<IReadOnlyList<TemplateCategory>>>>(),
                Arg.Any<CacheEntryOptions?>(),
                cancellationToken)
            .Returns(call => call.Arg<Func<CancellationToken, Task<IReadOnlyList<TemplateCategory>>>>()(cancellationToken));
        cacheProvider
            .GetOrSetAsync<IReadOnlyList<TemplateRenderTypeSetup>>(
                "template:setup:render-types",
                Arg.Any<Func<CancellationToken, Task<IReadOnlyList<TemplateRenderTypeSetup>>>>(),
                Arg.Any<CacheEntryOptions?>(),
                cancellationToken)
            .Returns(call => call.Arg<Func<CancellationToken, Task<IReadOnlyList<TemplateRenderTypeSetup>>>>()(cancellationToken));
        scope.DbContext.Add(ResumeTestData.TemplateCategory(id: 2));
        scope.DbContext.Add(ResumeTestData.TemplateRenderType(id: 2));
        await scope.DbContext.SaveChangesAsync(new TestAudit(1), cancellationToken);

        var repository = new TemplateSetupDataRepository(scope.UnitOfWork, cacheProvider);

        (await repository.ListTemplateCategoriesAsync(cancellationToken)).Count.ShouldBeGreaterThanOrEqualTo(2);
        (await repository.ListTemplateRenderTypesAsync(cancellationToken)).Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    private static ICacheProvider CreateCacheProvider()
    {
        var cacheProvider = Substitute.For<ICacheProvider>();
        cacheProvider.RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        return cacheProvider;
    }
}
