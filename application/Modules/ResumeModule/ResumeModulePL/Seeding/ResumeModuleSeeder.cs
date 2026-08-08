using Persistence;
using ResumeModuleDM.Entities;

namespace ResumeModulePL.Seeding;

public sealed class ResumeModuleSeeder : IAppDbContextSeeder
{
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<ResumeSectionSetup>().SeedSetupDataAsync(
            ResumeSectionSetupSeedData.Create(),
            (existingSection, seed) =>
            {
                var hasChanges = false;

                if (existingSection.SectionType != seed.SectionType)
                {
                    existingSection.SectionType = seed.SectionType;
                    hasChanges = true;
                }

                if (existingSection.DisplayOrder != seed.DisplayOrder)
                {
                    existingSection.DisplayOrder = seed.DisplayOrder;
                    hasChanges = true;
                }

                if (existingSection.IsVisible != seed.IsVisible)
                {
                    existingSection.IsVisible = seed.IsVisible;
                    hasChanges = true;
                }

                return hasChanges;
            },
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
