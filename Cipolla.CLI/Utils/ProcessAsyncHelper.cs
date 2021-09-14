using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Cipolla.CLI.Utils
{
    public static class ProcessAsyncHelper
    {
        public static Task StartProcessAsync(string command, string arguments, string workingDir, ILogger logger)
        {
            using var process = new Process();

            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = workingDir;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data)) logger.LogInformation(e.Data);
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data)) logger.LogError(e.Data);
            };

            process.Exited += (sender, e) =>
            {
                logger.LogInformation("Exited {0} (Code: {1})", process.Id, process.ExitCode);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return process.WaitForExitAsync();
        }
    }
}
