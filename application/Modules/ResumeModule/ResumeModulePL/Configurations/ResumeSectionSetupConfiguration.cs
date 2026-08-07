using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeModuleDM.Entities;
using ResumeModulePL.Seeding;

namespace ResumeModulePL.Configurations;

public sealed class ResumeSectionSetupConfiguration : IEntityTypeConfiguration<ResumeSectionSetup>
{
    public void Configure(EntityTypeBuilder<ResumeSectionSetup> builder)
    {
        builder.ToTable("ResumeSectionSetups", "resume");

        builder.HasKey(sectionSetup => sectionSetup.Id);

        builder.Property(sectionSetup => sectionSetup.SectionType).HasConversion<int>();
        builder.Property(sectionSetup => sectionSetup.SectionTitle).HasMaxLength(100).IsRequired();

        builder.HasIndex(sectionSetup => sectionSetup.SectionType).IsUnique();
        builder.HasIndex(sectionSetup => sectionSetup.DisplayOrder).IsUnique();

        builder.HasData(ResumeSectionSetupSeedData.Create());
    }
}
