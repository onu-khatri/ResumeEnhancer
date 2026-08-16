using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeEnhancer.TemplateModule.DM.Entities;

namespace ResumeEnhancer.TemplateModule.PL.Configurations;

public sealed class TemplateCategoryConfiguration : IEntityTypeConfiguration<TemplateCategory>
{
    public void Configure(EntityTypeBuilder<TemplateCategory> builder)
    {
        builder.Property(entity => entity.Code).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Description).HasMaxLength(1000).IsRequired();
        builder.Property(entity => entity.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.Order).IsRequired();
        builder.Property(entity => entity.IsDeactivated).HasDefaultValue(false);

        builder.HasIndex(entity => entity.Code).IsUnique();
    }
}

public sealed class TemplateRenderTypeSetupConfiguration : IEntityTypeConfiguration<TemplateRenderTypeSetup>
{
    public void Configure(EntityTypeBuilder<TemplateRenderTypeSetup> builder)
    {
        builder.Property(entity => entity.Code).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Description).HasMaxLength(1000).IsRequired();
        builder.Property(entity => entity.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.Order).IsRequired();

        builder.HasIndex(entity => entity.Code).IsUnique();
    }
}

public sealed class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.Property(entity => entity.Code).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Description).HasMaxLength(1000).IsRequired();
        builder.Property(entity => entity.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.RenderTypeId).IsRequired();
        builder.Property(entity => entity.Body).HasMaxLength(20000).IsRequired();
        builder.Property(entity => entity.PreviewImageUrl).HasMaxLength(500);
        builder.Property(entity => entity.IsDeactivated).HasDefaultValue(false);

        builder.HasIndex(entity => entity.Code).IsUnique();

        builder.HasOne(entity => entity.TemplateCategory)
            .WithMany(category => category.Templates)
            .HasForeignKey(entity => entity.TemplateCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(entity => entity.RenderType)
            .WithMany(renderType => renderType.Templates)
            .HasForeignKey(entity => entity.RenderTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
