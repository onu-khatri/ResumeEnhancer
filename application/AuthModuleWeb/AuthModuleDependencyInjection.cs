using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AuthModuleWeb
{
    public static class AuthModuleDependencyInjection
    {
        public static IServiceCollection AddAuthModuleWebDI(this IServiceCollection services)
        {
            // Add AuthModuleWeb services here
            services.AddControllers().AddApplicationPart(typeof(AuthModuleDependencyInjection).Assembly); // add-controllers from the current assembly

            services.AddValidatorsFromAssembly(typeof(AuthModuleDependencyInjection).Assembly); // add validators from the current assembly
            return services;
        }
    }
}
