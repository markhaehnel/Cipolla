using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Cipolla.CLI.Utils;
using CliWrap;
using Microsoft.Extensions.Logging;

namespace Cipolla.CLI.Models
{
    public enum InstanceStatus
    {
        Undetermined,
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
        public InstanceStatus Status { get; private set; } = InstanceStatus.Undetermined;

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
                    .WithStandardOutputPipe(verboseLogging ? PipeTarget.ToStream(Console.OpenStandardOutput()) : PipeTarget.Null)
                    .WithStandardErrorPipe(verboseLogging ? PipeTarget.ToStream(Console.OpenStandardError()) : PipeTarget.Null)
                    .ExecuteAsync();
        }

        public async Task<bool> CheckConnectivityAsync()
        {
            var proxy = new WebProxy
            {
                Address = new Uri($"socks5://127.0.0.1:{SocksPort}")
            };

            //proxy.Credentials = new NetworkCredential(); //Used to set Proxy logins. 
            var handler = new HttpClientHandler
            {
                Proxy = proxy
            };
            var httpClient = new HttpClient(handler);

            try
            {
                Logger.LogDebug("{instance} - IP: {data}", Id, await httpClient.GetStringAsync("https://icanhazip.com/"));
                return true;
            }
            catch
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
