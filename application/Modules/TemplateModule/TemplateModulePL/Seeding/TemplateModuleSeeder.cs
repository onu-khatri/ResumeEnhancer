using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.TemplateModule.DM.Entities;
using ResumeEnhancer.TemplateModule.DM.Enums;

namespace ResumeEnhancer.TemplateModule.PL.Seeding;

public sealed class TemplateModuleSeeder : IAppDbContextSeeder
{
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<TemplateRenderTypeSetup>().SeedSetupDataAsync(
            CreateRenderTypes(),
            (existing, seed) =>
            {
                var hasChanges = false;

                if (existing.DisplayName != seed.DisplayName)
                {
                    existing.DisplayName = seed.DisplayName;
                    hasChanges = true;
                }

                if (existing.Order != seed.Order)
                {
                    existing.Order = seed.Order;
                    hasChanges = true;
                }

                return hasChanges;
            },
            cancellationToken);

        var renderTypeIdsByCode = await dbContext.Set<TemplateRenderTypeSetup>()
            .AsNoTracking()
            .ToDictionaryAsync(item => item.Code, item => item.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        if (!await dbContext.Set<TemplateCategory>().AnyAsync(cancellationToken))
        {
            TemplateCategory[] categories =
            [
                new TemplateCategory
                {
                    Code = "PROFESSIONAL",
                    Description = "Professional resume and profile templates",
                    DisplayName = "Professional",
                    Order = 1,
                    Guid = Guid.NewGuid(),
                    IsDeactivated = false
                },
                new TemplateCategory
                {
                    Code = "MODERN",
                    Description = "Modern presentation-oriented templates",
                    DisplayName = "Modern",
                    Order = 2,
                    Guid = Guid.NewGuid(),
                    IsDeactivated = false
                }
            ];

            dbContext.Set<TemplateCategory>().AddRange(categories);
            await dbContext.SaveChangesAsync(cancellationToken);

            dbContext.Set<Template>().AddRange(
                new Template
                {
                    Code = "PROFESSIONAL_PDF",
                    Description = "Professional PDF export template",
                    DisplayName = "Professional PDF",
                    TemplateCategoryId = categories[0].Id,
                    RenderTypeId = renderTypeIdsByCode[nameof(TemplateRenderType.Pdf)],
                    Body = "<pdf-template />",
                    Guid = Guid.NewGuid(),
                    IsDeactivated = false
                },
                new Template
                {
                    Code = "MODERN_WORD",
                    Description = "Modern Word export template",
                    DisplayName = "Modern Word",
                    TemplateCategoryId = categories[1].Id,
                    RenderTypeId = renderTypeIdsByCode[nameof(TemplateRenderType.Word)],
                    Body = "<word-template />",
                    Guid = Guid.NewGuid(),
                    IsDeactivated = false
                },
                new Template
                {
                    Code = "PUBLIC_PROFILE_HTML",
                    Description = "Dynamic HTML online profile template",
                    DisplayName = "Public Profile HTML",
                    TemplateCategoryId = categories[1].Id,
                    RenderTypeId = renderTypeIdsByCode[nameof(TemplateRenderType.DynamicHtmlProfile)],
                    Body = "<html-profile-template />",
                    Guid = Guid.NewGuid(),
                    IsDeactivated = false
                });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static TemplateRenderTypeSetup[] CreateRenderTypes() =>
    [
        new TemplateRenderTypeSetup
        {
            Code = nameof(TemplateRenderType.Pdf),
            Description = "PDF template render type",
            DisplayName = "PDF",
            Order = 1,
            Guid = Guid.Parse("33333333-3333-3333-3333-333333333001"),
            ObsoleteFlag = false
        },
        new TemplateRenderTypeSetup
        {
            Code = nameof(TemplateRenderType.Word),
            Description = "Word template render type",
            DisplayName = "Word",
            Order = 2,
            Guid = Guid.Parse("33333333-3333-3333-3333-333333333002"),
            ObsoleteFlag = false
        },
        new TemplateRenderTypeSetup
        {
            Code = nameof(TemplateRenderType.DynamicHtmlProfile),
            Description = "Dynamic HTML profile template render type",
            DisplayName = "Dynamic HTML Profile",
            Order = 3,
            Guid = Guid.Parse("33333333-3333-3333-3333-333333333003"),
            ObsoleteFlag = false
        }
    ];
}
