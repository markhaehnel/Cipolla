using System.Threading.Tasks;
using Cipolla.CLI.Models;
using Cipolla.CLI.Services;
using Cipolla.CLI.Utils;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cipolla.CLI
{
    class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<TorInstanceManagerService>();
                    services.AddSingleton<TorInstanceManagerWorker>();
                    Parser.Default.ParseArguments<CliOptions>(args).WithParsed(options => services.AddSingleton(options));
                });
        }
    }
}
