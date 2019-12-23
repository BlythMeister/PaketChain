using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PaketChain
{
    internal static class ConsoleHelper
    {
        public static (int exitCode, List<string> consoleOutput) RunDotNetCommand(string rootDir, string arguments, CancellationToken cancellationToken)
        {
            var processInfo = new ProcessStartInfo
            {
                WorkingDirectory = rootDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                FileName = "dotnet",
                Arguments = arguments
            };

            var process = new Process
            {
                StartInfo = processInfo,
                EnableRaisingEvents = true
            };

            var output = new List<string>();

            process.OutputDataReceived += (sender, eventArgs) => { output.Add(eventArgs.Data); };
            process.ErrorDataReceived += (sender, eventArgs) => { output.Add(eventArgs.Data); };

            cancellationToken.Register(() => process.Kill(true));

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.CancelOutputRead();

            return (process.ExitCode, output);
        }

        public static void RunPaketCommand(string rootDir, string paketExePath, PaketType paketType, string command, string args, CancellationToken cancellationToken)
        {
            var arguments = string.Empty;

            switch (paketType)
            {
                case PaketType.Exe:
                case PaketType.GlobalTool:
                    arguments = $"{command} {args ?? string.Empty}".Trim();
                    break;

                case PaketType.LocalTool:
                    arguments = $"paket {command} {args ?? string.Empty}".Trim();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(paketType), paketType, null);
            }

            var processInfo = new ProcessStartInfo
            {
                WorkingDirectory = rootDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                FileName = paketExePath,
                Arguments = arguments
            };

            var process = new Process
            {
                StartInfo = processInfo,
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (sender, eventArgs) => { Console.WriteLine(eventArgs.Data); };
            process.ErrorDataReceived += (sender, eventArgs) => { Console.WriteLine(eventArgs.Data); };

            cancellationToken.Register(() => process.Kill(true));

            Console.WriteLine($"Running: {processInfo.FileName} {processInfo.Arguments}");
            Console.WriteLine("");
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.CancelOutputRead();

            if (process.ExitCode < 0)
            {
                throw new Exception($"Paket exit code: {process.ExitCode}");
            }
        }
    }
}
