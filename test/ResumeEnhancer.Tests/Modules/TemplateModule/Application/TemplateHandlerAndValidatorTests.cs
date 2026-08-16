using NSubstitute;
using Shouldly;
using ResumeEnhancer.TemplateModule.AM.Requests;
using ResumeEnhancer.TemplateModule.DM.Entities;
using ResumeEnhancer.TemplateModule.SL.Abstractions.Persistence;
using ResumeEnhancer.TemplateModule.SL.Handlers;
using ResumeEnhancer.TemplateModule.Web.Validation;

namespace ResumeEnhancer.Tests.Unit.Modules.TemplateModule.Application;

public sealed class TemplateHandlerAndValidatorTests
{
    [Fact]
    public async Task TemplateCategoryHandlers_CoverCrudFlows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<ITemplateRepository>();
        repository.GetTemplateCategoryAsync(1, true, cancellationToken)
            .Returns(new TemplateCategory { Id = 1, Code = "MODERN", Description = "Desc", DisplayName = "Modern" });
        repository.GetTemplateCategoryAsync(99, true, cancellationToken).Returns((TemplateCategory?)null);
        repository.GetTemplateCategoryAsync(1, false, cancellationToken)
            .Returns(new TemplateCategory { Id = 1, Code = "MODERN", Description = "Desc", DisplayName = "Modern" });
        repository.ListTemplateCategoriesAsync(cancellationToken).Returns(
            new[] { new TemplateCategory { Id = 1, Code = "MODERN", DisplayName = "Modern" } });

        var created = await new CreateTemplateCategoryCommandHandler(repository).Handle(
            new(new CreateTemplateCategoryRequest { Code = " MODERN ", Description = " Desc ", DisplayName = " Modern ", IsDeactivated = true }, 3),
            cancellationToken);
        var updated = await new UpdateTemplateCategoryCommandHandler(repository).Handle(
            new(1, new UpdateTemplateCategoryRequest { Code = " CLASSIC ", Description = " Updated ", DisplayName = " Classic ", IsDeactivated = true, ObsoleteFlag = true }, 4),
            cancellationToken);
        var updateMissing = await new UpdateTemplateCategoryCommandHandler(repository).Handle(
            new(99, new UpdateTemplateCategoryRequest { Code = "X", Description = "Y", DisplayName = "Z" }, 4),
            cancellationToken);
        var deleted = await new DeleteTemplateCategoryCommandHandler(repository).Handle(new(1, 5), cancellationToken);
        var deletedMissing = await new DeleteTemplateCategoryCommandHandler(repository).Handle(new(99, 5), cancellationToken);
        var detail = await new GetTemplateCategoryQueryHandler(repository).Handle(new(1), cancellationToken);
        var items = await new ListTemplateCategoriesQueryHandler(repository).Handle(new(), cancellationToken);

        created.Code.ShouldBe("MODERN");
        updated!.Code.ShouldBe("CLASSIC");
        updated.ObsoleteFlag.ShouldBeTrue();
        updateMissing.ShouldBeNull();
        deleted.ShouldBeTrue();
        deletedMissing.ShouldBeFalse();
        detail!.DisplayName.ShouldBe("Modern");
        items.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task TemplateHandlers_CoverCrudFlowsAndRenderTypeGuards()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<ITemplateRepository>();
        var setupRepository = Substitute.For<ITemplateSetupDataRepository>();
        var renderType = new TemplateRenderTypeSetup { Id = 2, Code = "HTML", DisplayName = "HTML" };
        repository.TemplateCategoryExistsAsync(1, cancellationToken).Returns(true);
        repository.TemplateCategoryExistsAsync(999, cancellationToken).Returns(false);
        repository.GetTemplateAsync(5, true, cancellationToken)
            .Returns(new Template { Id = 5, Code = "TMP", Description = "Desc", DisplayName = "Template", TemplateCategoryId = 1, RenderTypeId = 2, RenderType = renderType, Body = "<div />" });
        repository.GetTemplateAsync(9, true, cancellationToken).Returns((Template?)null);
        repository.GetTemplateAsync(5, false, cancellationToken)
            .Returns(new Template { Id = 5, Code = "TMP", Description = "Desc", DisplayName = "Template", TemplateCategoryId = 1, RenderTypeId = 2, RenderType = renderType, Body = "<div />" });
        repository.ListTemplatesAsync(cancellationToken).Returns(
            new[] { new Template { Id = 5, Code = "TMP", DisplayName = "Template", TemplateCategoryId = 1, RenderType = renderType } });
        setupRepository.ListTemplateRenderTypesAsync(cancellationToken).Returns(new[] { renderType });

        var created = await new CreateTemplateCommandHandler(repository, setupRepository).Handle(
            new(new CreateTemplateRequest
            {
                Code = " TMP ",
                Description = " Desc ",
                DisplayName = " Template ",
                TemplateCategoryId = 1,
                RenderTypeCode = "html",
                Body = " <body> ",
                PreviewImageUrl = "  "
            }, 3),
            cancellationToken);
        var invalidCategory = await new CreateTemplateCommandHandler(repository, setupRepository).Handle(
            new(new CreateTemplateRequest { TemplateCategoryId = 999, RenderTypeCode = "HTML" }, 3),
            cancellationToken);
        var updated = await new UpdateTemplateCommandHandler(repository, setupRepository).Handle(
            new(5, new UpdateTemplateRequest
            {
                Code = " TMP-2 ",
                Description = " Updated ",
                DisplayName = " Template 2 ",
                TemplateCategoryId = 1,
                RenderTypeCode = "HTML",
                Body = " <main> ",
                PreviewImageUrl = " image ",
                IsDeactivated = true,
                ObsoleteFlag = true
            }, 4),
            cancellationToken);
        var updateMissing = await new UpdateTemplateCommandHandler(repository, setupRepository).Handle(
            new(9, new UpdateTemplateRequest { TemplateCategoryId = 1, RenderTypeCode = "HTML" }, 4),
            cancellationToken);
        var deleted = await new DeleteTemplateCommandHandler(repository).Handle(new(5, 4), cancellationToken);
        var deletedMissing = await new DeleteTemplateCommandHandler(repository).Handle(new(9, 4), cancellationToken);
        var detail = await new GetTemplateQueryHandler(repository).Handle(new(5), cancellationToken);
        var items = await new ListTemplatesQueryHandler(repository).Handle(new(), cancellationToken);

        created!.Code.ShouldBe("TMP");
        created.PreviewImageUrl.ShouldBeNull();
        invalidCategory.ShouldBeNull();
        updated!.Code.ShouldBe("TMP-2");
        updated.RenderTypeCode.ShouldBe("HTML");
        updateMissing.ShouldBeNull();
        deleted.ShouldBeTrue();
        deletedMissing.ShouldBeFalse();
        detail!.Id.ShouldBe(5);
        items.ShouldHaveSingleItem();
    }

    [Fact]
    public void TemplateValidators_RejectInvalidRequests()
    {
        new CreateTemplateCategoryRequestValidator()
            .Validate(new CreateTemplateCategoryRequest { Code = "", Description = "", DisplayName = "" })
            .Errors.Count.ShouldBeGreaterThan(0);
        new UpdateTemplateCategoryRequestValidator()
            .Validate(new UpdateTemplateCategoryRequest { Code = "", Description = "", DisplayName = "" })
            .Errors.Count.ShouldBeGreaterThan(0);
        new CreateTemplateRequestValidator()
            .Validate(new CreateTemplateRequest { Code = "", Description = "", DisplayName = "", TemplateCategoryId = 0, Body = "", RenderTypeCode = "", PreviewImageUrl = new string('x', 501) })
            .Errors.Count.ShouldBeGreaterThan(0);
        new UpdateTemplateRequestValidator()
            .Validate(new UpdateTemplateRequest { Code = "", Description = "", DisplayName = "", TemplateCategoryId = 0, Body = "", RenderTypeCode = "", PreviewImageUrl = new string('x', 501) })
            .Errors.Count.ShouldBeGreaterThan(0);
    }
}
