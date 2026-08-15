using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.ResumeModule.PL.Configurations;

public sealed class ResumeSectionSetupConfiguration : IEntityTypeConfiguration<ResumeSectionSetup>
{
    public void Configure(EntityTypeBuilder<ResumeSectionSetup> builder)
    {
        builder.HasKey(sectionSetup => sectionSetup.Id);

        builder.Property(sectionSetup => sectionSetup.SectionType).HasConversion<int>();

        builder.HasIndex(sectionSetup => sectionSetup.SectionType).IsUnique();
        builder.HasIndex(sectionSetup => sectionSetup.DisplayOrder).IsUnique();
    }
}

