using System;
using System.Collections.Generic;
using System.Text;
using AuthModuleDM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthModulePL.Configurations;

public sealed class AuthModuleUserConfigurations : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> builder)
    {
        //builder.HasKey(x => x.Id);
        builder.HasMany(user => user.UserRoles)
               .WithOne(UserRoles => UserRoles.User)
               .HasForeignKey(UserRoles => UserRoles.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(user => user.UserClaims)
               .WithOne(UserClaims => UserClaims.User)
               .HasForeignKey(UserClaims => UserClaims.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
