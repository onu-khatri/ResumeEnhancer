using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeModuleDM.Entities;

namespace ResumeModulePL.Configurations;

public sealed class AwardConfiguration : IEntityTypeConfiguration<Award>
{
    public void Configure(EntityTypeBuilder<Award> builder)
    {
        builder.ToTable("Awards", "resume");

        builder.HasKey(award => award.Id);

        builder.Property(award => award.AwardName).HasMaxLength(200).IsRequired();
        builder.Property(award => award.IssuingOrganization).HasMaxLength(200);
        builder.Property(award => award.Description).HasMaxLength(1000);

        builder.HasOne(award => award.PersonalInformation)
            .WithMany(personalInformation => personalInformation.Awards)
            .HasForeignKey(award => award.PersonalInformationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
