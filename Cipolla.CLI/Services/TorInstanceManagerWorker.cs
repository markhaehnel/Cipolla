using System;
using System.Threading;
using System.Threading.Tasks;
using Cipolla.CLI.Models;
using Cipolla.CLI.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cipolla.CLI.Services
{
    public class TorInstanceManagerWorker : BackgroundService
    {
        private readonly ILogger<TorInstanceManagerWorker> _logger;
        private readonly CliOptions _options;

        public TorInstanceManagerWorker(ILogger<TorInstanceManagerWorker> logger, CliOptions options)
        {
            _logger = logger;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            FileSystem.EnsureDirectoryExists(_options.DataDirectory);

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Looping..");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }

            return;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cleaning up..");
            base.StopAsync(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
