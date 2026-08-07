using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeModuleDM.Entities;

namespace ResumeModulePL.Configurations;

public sealed class SocialMediaLinkConfiguration : IEntityTypeConfiguration<SocialMediaLink>
{
    public void Configure(EntityTypeBuilder<SocialMediaLink> builder)
    {
        builder.ToTable("SocialMediaLinks", "resume");

        builder.HasKey(socialMediaLink => socialMediaLink.Id);

        builder.Property(socialMediaLink => socialMediaLink.Platform).HasMaxLength(100).IsRequired();
        builder.Property(socialMediaLink => socialMediaLink.Url).HasMaxLength(500).IsRequired();
        builder.Property(socialMediaLink => socialMediaLink.DisplayName).HasMaxLength(100);

        builder.HasOne(socialMediaLink => socialMediaLink.PersonalInformation)
            .WithMany(personalInformation => personalInformation.SocialMediaLinks)
            .HasForeignKey(socialMediaLink => socialMediaLink.PersonalInformationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
