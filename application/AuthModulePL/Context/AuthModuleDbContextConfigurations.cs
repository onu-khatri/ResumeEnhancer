using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace AuthModulePL.Context
{
    public sealed class AuthModuleDbContextConfigurations : IAppDbContextModelConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthModuleDbContextConfigurations).Assembly);
        }
    }
}
