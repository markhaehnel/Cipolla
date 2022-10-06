using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cipolla.CLI.Utils;
using CliWrap;
using Microsoft.Extensions.Logging;

namespace Cipolla.CLI.Models
{
    public enum InstanceStatus
    {
        Starting,
        Started,
        Healthy,
        Unhealthy
    }

    public class TorInstance : IDisposable
    {

        public Guid Id { get; init; }
        public ushort SocksPort { get; init; }
        public ushort ControlPort { get; init; }
        public string InstanceDataPath { get; init; }
        public CommandTask<CommandResult> Process { get; init; }
        public InstanceStatus Status { get; private set; } = InstanceStatus.Starting;

        private ILogger Logger { get; init; }

        public TorInstance(ushort socksPort, ushort controlPort, string dataDirectory, bool verboseLogging, ILogger logger)
        {
            Id = Guid.NewGuid();
            SocksPort = socksPort;
            ControlPort = controlPort;
            InstanceDataPath = Path.Combine(dataDirectory, Id.ToString());
            Logger = logger;

            FileSystem.EnsureDirectoryExists(InstanceDataPath);

            var torConfigFileName = $"torrc_{Id}";
            var torConfigPath = Path.Combine(InstanceDataPath, torConfigFileName);
            var torConfigContent = TorConfigTemplate.GetTorConfig(SocksPort, ControlPort, InstanceDataPath);

            File.WriteAllText(torConfigPath, torConfigContent);

            Process = Cli.Wrap("tor")
                    .WithArguments($"-f {torConfigFileName}")
                    .WithWorkingDirectory(InstanceDataPath)
                    // .WithStandardOutputPipe(verboseLogging ? PipeTarget.ToStream(Console.OpenStandardOutput()) : PipeTarget.Null)
                    // .WithStandardErrorPipe(verboseLogging ? PipeTarget.ToStream(Console.OpenStandardError()) : PipeTarget.Null)
                    .ExecuteAsync();

            Task.Run(() => WaitForStartup());
        }

        private Task WaitForStartup()
        {
            return Task.Run(() =>
            {
                // TODO: add real ready check
                Thread.Sleep(TimeSpan.FromSeconds(30));
                Status = InstanceStatus.Healthy;
            });
        }

        public async Task CheckConnectivityAsyncCheckStatusAsync()
        {
            var goodStatuses = new List<TaskStatus>() { TaskStatus.Running, TaskStatus.WaitingForActivation };

            if (!goodStatuses.Contains(Process.Task.Status))
            {
                Status = InstanceStatus.Unhealthy;
                return;
            }

            switch (Status)
            {
                case InstanceStatus.Started:
                case InstanceStatus.Healthy:
                    var isHealthy = await CheckConnectivityAsync();
                    Status = isHealthy ? InstanceStatus.Healthy : InstanceStatus.Unhealthy;
                    return;
                default:
                    break;
            }
        }

        private async Task<bool> CheckConnectivityAsync()
        {
            var proxy = new WebProxy
            {
                Address = new Uri($"socks5://localhost:{SocksPort}")
            };

            //proxy.Credentials = new NetworkCredential(); //Used to set Proxy logins. 
            var handler = new HttpClientHandler
            {
                Proxy = proxy
            };
            var httpClient = new HttpClient(handler);

            try
            {
                await httpClient.GetStringAsync("https://icanhazip.com/");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Dispose()
        {
            Process.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
