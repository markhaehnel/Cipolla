using System;
using System.Threading.Tasks;
using Cipolla.CLI.Models;
using Cipolla.CLI.Services;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cipolla.CLI
{
    class Program
    {
        public async static Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<CliOptions>(args).WithParsedAsync(async options =>
            {
                if (options.VerboseLogging) Environment.SetEnvironmentVariable("LOGGING__LOGLEVEL__DEFAULT", "DEBUG");
                var host = CreateHostBuilder(args, options).Build();
                await host.RunAsync();
            });
        }

        public static IHostBuilder CreateHostBuilder(string[] args, CliOptions options)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<TorInstanceManagerService>();
                    services.AddSingleton<TorInstanceManagerWorker>();
                    services.AddSingleton(options);
                });
        }
    }
}
