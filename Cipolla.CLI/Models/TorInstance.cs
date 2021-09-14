using System;
using System.IO;
using System.Threading.Tasks;
using Cipolla.CLI.Utils;
using Microsoft.Extensions.Logging;

namespace Cipolla.CLI.Models
{
    public class TorInstance : IDisposable
    {
        public Guid Id { get; init; }
        public ushort SocksPort { get; init; }
        public ushort ControlPort { get; init; }
        public string InstanceDataPath { get; init; }
        public Task Task { get; init; }

        public TorInstance(ushort socksPort, ushort controlPort, string dataDirectory, ILogger logger)
        {
            Id = Guid.NewGuid();
            SocksPort = socksPort;
            ControlPort = controlPort;
            InstanceDataPath = Path.Combine(dataDirectory, Id.ToString());

            FileSystem.EnsureDirectoryExists(InstanceDataPath);

            var torConfigFileName = $"torrc_{Id}";
            var torConfigPath = Path.Combine(InstanceDataPath, torConfigFileName);
            var torConfigContent = TorConfigTemplate.GetTorConfig(SocksPort, ControlPort, InstanceDataPath);

            File.WriteAllText(torConfigPath, torConfigContent);

            Task = ProcessAsyncHelper.StartProcessAsync("tor", $"-f {torConfigFileName}", InstanceDataPath, logger);
        }

        public void Dispose()
        {
            Task.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
