using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cipolla.CLI.Models;
using Cipolla.CLI.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cipolla.CLI.Services
{
    public class TorInstanceManagerService : IHostedService
    {
        private readonly ILogger<TorInstanceManagerService> _logger;
        private readonly CliOptions _options;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly TorInstanceManagerWorker _worker;

        public TorInstanceManagerService(ILogger<TorInstanceManagerService> logger, CliOptions options, IHostApplicationLifetime applicationLifetime, TorInstanceManagerWorker worker)
        {
            _logger = logger;
            _options = options;
            _applicationLifetime = applicationLifetime;
            _worker = worker;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Manager..");

            _logger.LogInformation(string.Join("\n", _options.GetType().GetProperties().Select(x => $"{x.Name}: {x.GetValue(_options, null)}")));

            _worker.StartAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _worker.StopAsync(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
