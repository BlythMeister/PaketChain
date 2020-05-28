using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PaketChain
{
    internal static class ConsoleHelper
    {
        public static IReadOnlyCollection<string> RunDotNetCommandWithOutput(string rootDir, string arguments, CancellationToken cancellationToken)
        {
            return RunReturnOutput(rootDir, "dotnet", arguments, cancellationToken);
        }

        public static void RunDotNetCommand(string rootDir, string arguments, CancellationToken cancellationToken)
        {
            Run(rootDir, "dotnet", arguments, cancellationToken);
        }

        public static void RunPaketCommand(string rootDir, string paketExePath, PaketType paketType, string command, string args, CancellationToken cancellationToken)
        {
            var arguments = paketType switch
            {
                PaketType.Exe => $"{command} {args ?? string.Empty}".Trim(),
                PaketType.GlobalTool => $"{command} {args ?? string.Empty}".Trim(),
                PaketType.LocalTool => $"paket {command} {args ?? string.Empty}".Trim(),
                _ => throw new ArgumentOutOfRangeException(nameof(paketType), paketType, null)
            };

            Run(rootDir, paketExePath, arguments, cancellationToken);
        }

        public static void RunGitCommand(string rootDir, string command, string args, CancellationToken cancellationToken)
        {
            Run(rootDir, "git", $"{command} {args ?? string.Empty}".Trim(), cancellationToken);
        }

        private static void Run(string rootDir, string fileName, string arguments, CancellationToken cancellationToken)
        {
            var processInfo = new ProcessStartInfo
            {
                WorkingDirectory = rootDir,
                FileName = fileName,
                Arguments = arguments
            };

            var process = new Process
            {
                StartInfo = processInfo
            };

            cancellationToken.Register(() => process.Kill(true));

            Console.WriteLine($"Running: {fileName} {arguments}");
            Console.WriteLine("");
            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"'{fileName} {arguments}' exit code: {process.ExitCode}");
            }
        }

        private static IReadOnlyCollection<string> RunReturnOutput(string rootDir, string fileName, string arguments, CancellationToken cancellationToken)
        {
            var output = new List<string>();

            var processInfo = new ProcessStartInfo
            {
                WorkingDirectory = rootDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                FileName = fileName,
                Arguments = arguments
            };

            var process = new Process
            {
                StartInfo = processInfo,
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (sender, args) =>
            {
                output.Add(args.Data);
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                output.Add(args.Data);
            };

            cancellationToken.Register(() => process.Kill(true));

            Console.WriteLine($"Running: {fileName} {arguments}");
            Console.WriteLine("");
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.CancelOutputRead();
            process.CancelErrorRead();

            if (process.ExitCode != 0)
            {
                throw new Exception($"'{fileName} {arguments}' exit code: {process.ExitCode}");
            }

            return output;
        }
    }
}
