using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Discovery.Client;

namespace bootcamp_webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            host.EnsureMigrationOfContext<ProductContext>();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.AddConfigServer(GetLoggerFactory());
                    webBuilder.AddServiceDiscovery();
                    webBuilder.UseStartup<Startup>();
                });

        public static ILoggerFactory GetLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace)
                    .AddConsole()
                    .AddDebug();
            });
            return serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
        }
    }
}
