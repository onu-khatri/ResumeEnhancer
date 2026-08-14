using System;
using System.Collections.Generic;
using System.Text;
using AuthModuleDM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthModulePL.Configurations;
public sealed class AuthModuleRoleClaimConfigurations
    : IEntityTypeConfiguration<RoleClaims>
{
    public void Configure(EntityTypeBuilder<RoleClaims> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(RoleClaims => RoleClaims.ClaimType)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(RoleClaims => RoleClaims.ClaimValue)
               .IsRequired()
               .HasMaxLength(500);

        builder.HasOne(RoleClaims => RoleClaims.Role)
               .WithMany(Roles => Roles.RoleClaims)
               .HasForeignKey(RoleClaims => RoleClaims.RoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
