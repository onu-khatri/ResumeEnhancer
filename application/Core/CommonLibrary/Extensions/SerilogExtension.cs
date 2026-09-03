using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace ResumeEnhancer.Core.CommonLibrary.Extensions;

public static class SerilogExtension
{
    public static void ConfigureSerilog(this IHostBuilder host, IConfiguration configurations)
    {
        var sink_Options = new MSSqlServerSinkOptions()
        {
            TableName = "Logs",
            AutoCreateSqlTable = true,
            SchemaName = "ResumeEnhancer",
        };

        var LogConnectionString = configurations["LogConnectionString"];

        host.UseSerilog((context, config) =>
        {
            if (context.HostingEnvironment.IsDevelopment())
            {
                config.WriteTo.Console();
                config.WriteTo.File("Logs/log.txt",
                    shared: false,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30);
            }
            else if (!string.IsNullOrWhiteSpace(LogConnectionString))
            {
                config.WriteTo.MSSqlServer(connectionString: LogConnectionString, sinkOptions: sink_Options);
            }
        });
    }
}
