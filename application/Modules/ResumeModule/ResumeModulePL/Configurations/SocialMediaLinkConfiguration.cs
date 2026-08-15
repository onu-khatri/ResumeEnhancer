using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.ResumeModule.PL.Configurations;

public sealed class SocialMediaLinkConfiguration : IEntityTypeConfiguration<SocialMediaLink>
{
    public void Configure(EntityTypeBuilder<SocialMediaLink> builder)
    {
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

