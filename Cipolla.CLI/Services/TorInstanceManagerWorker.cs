using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private readonly List<TorInstance> _instances = new();

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
                _logger.LogDebug("Removing unhealthy instances..");
                _instances.RemoveAll(x => x.Status == InstanceStatus.Unhealthy);

                _logger.LogDebug("Checking for missing instances..");
                _instances.AddRange(CreateMissingTorInstances());

                _logger.LogDebug("Triggering instance self-checks..");
                _instances.ForEach(x => Task.Run(x.CheckConnectivityAsyncCheckStatusAsync));

                await Task.Delay(TimeSpan.FromSeconds(_options.CheckInterval), cancellationToken);
            }

            return;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cleaning up..");
            base.StopAsync(cancellationToken);

            Directory.Delete(_options.DataDirectory, true);
            return Task.CompletedTask;
        }

        private IEnumerable<TorInstance> CreateMissingTorInstances()
        {
            var possiblePorts = Enumerable.Range(_options.StartingSocksPort, _options.NumberOfInstances).Select(i => (ushort)i);
            var usedPorts = _instances.Select(x => x.SocksPort);
            var freePorts = possiblePorts.Where(x => !usedPorts.Contains(x));
            return freePorts.Select(port => new TorInstance(port, (ushort)(port + 100), _options.DataDirectory, _options.VerboseLogging, _logger));
        }
    }
}
