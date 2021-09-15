using System;
using System.IO;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace Cipolla.CLI.Models
{
    public class CliOptions
    {
        [Option('n', "num-instances", Default = 10, HelpText = "Number of Tor instances to launch")]
        public int NumberOfInstances { get; init; }

        [Option('p', "socks-port", Default = (ushort)9250, HelpText = "Starting port number for socks port")]
        public ushort StartingSocksPort { get; init; }

        [Option('d', "data-directory", HelpText = "Starting port number for control port")]
        public string DataDirectory { get; init; } = Path.Combine(Path.GetTempPath(), "Cipolla");

        [Option('i', "interval", Default = 30, HelpText = "Instance check interval in seconds")]
        public int CheckInterval { get; init; }

        [Option('v', "verbose", Default = false, HelpText = "Makes logging output more verbose")]
        public bool VerboseLogging { get; init; }
    }

    public static class CliOptionsExtensions
    {
        public static void PrintOptions(this CliOptions options, ILogger logger)
        {
            foreach (var prop in options.GetType().GetProperties())
            {
                logger.LogInformation("{0}: {1}", prop.Name, prop.GetValue(options, null));
            }
        }
    }
}
