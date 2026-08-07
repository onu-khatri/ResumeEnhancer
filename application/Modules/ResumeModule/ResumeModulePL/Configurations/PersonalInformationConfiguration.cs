using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeModuleDM.Entities;

namespace ResumeModulePL.Configurations;

public sealed class PersonalInformationConfiguration : IEntityTypeConfiguration<PersonalInformation>
{
    public void Configure(EntityTypeBuilder<PersonalInformation> builder)
    {
        builder.ToTable("PersonalInformation", "resume");

        builder.HasKey(personalInformation => personalInformation.Id);

        builder.Property(personalInformation => personalInformation.Email).HasMaxLength(256);
        builder.Property(personalInformation => personalInformation.PhoneNumber).HasMaxLength(30);

        builder.HasOne(personalInformation => personalInformation.Resume)
            .WithOne(resume => resume.PersonalInformation)
            .HasForeignKey<PersonalInformation>(personalInformation => personalInformation.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(personalInformation => personalInformation.ResumeId).IsUnique();
    }
}
