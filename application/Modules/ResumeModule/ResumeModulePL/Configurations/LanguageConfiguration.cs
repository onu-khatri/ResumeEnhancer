using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.ResumeModule.PL.Configurations;

public sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasKey(language => language.Id);

        builder.Property(language => language.LanguageName).HasMaxLength(100).IsRequired();
        builder.Property(language => language.ProficiencyLevel).HasMaxLength(100);
        builder.Property(language => language.Description).HasMaxLength(500);

        builder.HasOne(language => language.PersonalInformation)
            .WithMany(personalInformation => personalInformation.Languages)
            .HasForeignKey(language => language.PersonalInformationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

