using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.ResumeModule.PL.Seeding;

public sealed class ResumeModuleSeeder : IAppDbContextSeeder
{
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<ResumeSectionSetup>().SeedSetupDataAsync(
            ResumeSectionSetupSeedData.Create(),
            (existingSection, seed) =>
            {
                var hasChanges = false;

                if (existingSection.Order != seed.Order)
                {
                    existingSection.Order = seed.Order;
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

