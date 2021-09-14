using System;
using System.IO;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace Cipolla.CLI.Models
{
    public class CliOptions
    {
        [Option('n', "num-instances", Default = 10, HelpText = "Number of Tor instances to launch")]
        public int NumberOfInstances { get; set; }

        [Option('p', "socks-port", Default = (ushort)9250, HelpText = "Starting port number for socks port")]
        public ushort StartingSocksPort { get; set; }

        [Option('d', "data-directory", HelpText = "Starting port number for control port")]
        public string DataDirectory { get; set; } = Path.Combine(Path.GetTempPath(), "Cipolla");
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
