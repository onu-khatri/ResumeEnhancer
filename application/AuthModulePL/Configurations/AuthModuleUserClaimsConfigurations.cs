using System;
using System.Collections.Generic;
using System.Text;
using AuthModuleDM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthModulePL.Configurations;


public sealed class AuthModuleUserClaimConfigurations
    : IEntityTypeConfiguration<UserClaims>
{
    public void Configure(EntityTypeBuilder<UserClaims> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClaimType)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.ClaimValue)
               .IsRequired()
               .HasMaxLength(500);

        builder.HasOne(UserClaims => UserClaims.User)
               .WithMany(Users => Users.UserClaims)
               .HasForeignKey(UserClaims => UserClaims.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
