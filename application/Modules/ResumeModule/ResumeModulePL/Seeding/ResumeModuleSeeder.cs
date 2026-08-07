using Microsoft.EntityFrameworkCore;
using Persistence;
using ResumeModuleDM.Entities;

namespace ResumeModulePL.Seeding;

public sealed class ResumeModuleSeeder : IAppDbContextSeeder
{
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var sectionSetups = dbContext.Set<ResumeSectionSetup>();

        foreach (var seed in ResumeSectionSetupSeedData.Create())
        {
            var existingSection = await sectionSetups.FindAsync([seed.Id], cancellationToken);

            if (existingSection is null)
            {
                sectionSetups.Add(seed);
                continue;
            }

            existingSection.SectionType = seed.SectionType;
            existingSection.SectionTitle = seed.SectionTitle;
            existingSection.DisplayOrder = seed.DisplayOrder;
            existingSection.IsVisible = seed.IsVisible;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
