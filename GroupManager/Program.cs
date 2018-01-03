using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GroupManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog((context, logger) =>
                {
                    string cnnstr = context.Configuration["ConnectionStrings:DefaultConnection"];

                    logger.MinimumLevel.Error()
                        .Enrich.FromLogContext()
                        .WriteTo.MSSqlServer(
                            connectionString: cnnstr,
                            tableName: "Errors",
                            autoCreateSqlTable: true);

                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        logger.WriteTo.RollingFile("error-log.txt");
                    }

                })
                .Build();
    }
}
