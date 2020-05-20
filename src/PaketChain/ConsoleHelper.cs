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

            return Run(processInfo, cancellationToken, true);
        }

        public static void RunDotNetCommand(string rootDir, string arguments, CancellationToken cancellationToken)
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

            Run(processInfo, cancellationToken);
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

            Run(processInfo, cancellationToken);
        }

        public static void RunGitCommand(string rootDir, string command, string args, CancellationToken cancellationToken)
        {
            var processInfo = new ProcessStartInfo
            {
                WorkingDirectory = rootDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                FileName = "git",
                Arguments = $"{command} {args ?? string.Empty}".Trim()
            };

            Run(processInfo, cancellationToken);
        }

        private static IReadOnlyCollection<string> Run(ProcessStartInfo processInfo, CancellationToken cancellationToken, bool silent = false)
        {
            var process = new Process
            {
                StartInfo = processInfo,
                EnableRaisingEvents = true
            };

            var output = new List<string>();

            void ProcessDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
            {
                if (!silent)
                {
                    Console.WriteLine(dataReceivedEventArgs.Data);
                }
                output.Add(dataReceivedEventArgs.Data);
            }

            process.OutputDataReceived += (sender, args) =>
            {
                if (!silent)
                {
                    Console.WriteLine(args.Data);
                }
                output.Add(args.Data);
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!silent)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(args.Data);
                    Console.ResetColor();
                }
                output.Add(args.Data);
            };

            cancellationToken.Register(() => process.Kill(true));

            Console.WriteLine($"Running: {processInfo.FileName} {processInfo.Arguments}");
            Console.WriteLine("");
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.CancelOutputRead();
            process.CancelErrorRead();

            if (process.ExitCode != 0)
            {
                throw new Exception($"'{processInfo.FileName} {processInfo.Arguments}' exit code: {process.ExitCode}");
            }

            return output;
        }
    }
}
