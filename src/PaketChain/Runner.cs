using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace PaketChain
{
    internal class Runner
    {
        public static int Start(RunnerArgs runnerArgs, CancellationToken cancellationToken)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("");
                Console.WriteLine("----------------------------");
                Console.WriteLine("|                          |");
                Console.WriteLine("|        Paket Chain       |");
                Console.WriteLine("|                          |");
                Console.WriteLine("|    Author: Chris Blyth   |");
                Console.WriteLine("|                          |");
                Console.WriteLine("----------------------------");
                Console.WriteLine("");
                Console.WriteLine($"Starting at {DateTime.UtcNow:u}");
                Console.WriteLine("");
                Console.ResetColor();
                return Run(runnerArgs, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine("-----------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An Error Occured:");
                Console.WriteLine("");
                if (runnerArgs.Verbose)
                {
                    Console.WriteLine(e);
                }
                else
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine("");
                Console.ResetColor();
                Console.WriteLine("-----------------------------------------------------");
                return -1;
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Done at {DateTime.UtcNow:u}");
                if (!runnerArgs.NoPrompt)
                {
                    Console.WriteLine("Press Enter To Close...");
                    Console.ReadLine();
                }
                Console.ResetColor();
            }
        }

        private static int Run(RunnerArgs runnerArgs, CancellationToken cancellationToken)
        {
            var rootDir = string.IsNullOrWhiteSpace(runnerArgs.Directory) ? Environment.CurrentDirectory : runnerArgs.Directory;
            ValidatePaths(rootDir);
            var paketPath = LocatePaketFilePath(rootDir, cancellationToken);

            Console.WriteLine($"Running against: {rootDir}");
            Console.WriteLine("-----------------------------------------------------");
            if (cancellationToken.IsCancellationRequested) return -2;

            if (runnerArgs.Sort)
            {
                Sorter.SortReferences(rootDir);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
                Sorter.SortDependencies(rootDir);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

            if (runnerArgs.ClearCache)
            {
                ConsoleHelper.RunPaketCommand(rootDir, paketPath, "clear-cache", "--clear-local", cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

            if (runnerArgs.CleanObj)
            {
                Cleaner.CleanObjFiles(rootDir);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

            if (runnerArgs.Reinstall)
            {
                if (runnerArgs.Update)
                {
                    Console.WriteLine("Skipping Update as reinstall install newest versions");
                }

                Console.WriteLine("Deleting paket.lock file");
                File.Delete(Path.Combine(rootDir, "paket.lock"));
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }
            else if (runnerArgs.Update)
            {
                ConsoleHelper.RunPaketCommand(rootDir, paketPath, "update", runnerArgs.UpdateArgs, cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

            if (runnerArgs.Simplify)
            {
                ConsoleHelper.RunPaketCommand(rootDir, paketPath, "simplify", runnerArgs.SimplifyArgs, cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

            if (runnerArgs.Install)
            {
                ConsoleHelper.RunPaketCommand(rootDir, paketPath, "install", runnerArgs.InstallArgs, cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

            return 0;
        }

        private static void ValidatePaths(string rootDir)
        {
            if (!Directory.Exists(rootDir))
            {
                throw new DirectoryNotFoundException($"Unable to locate directory {rootDir}");
            }
        }

        private static string LocatePaketFilePath(string rootDir, CancellationToken cancellationToken)
        {
            if (Directory.Exists(Path.Combine(rootDir, ".paket")) && File.Exists(Path.Combine(rootDir, ".paket", "paket.exe")))
            {
                return Path.Combine(rootDir, ".paket", "paket.exe");
            }

            var (_, output) = ConsoleHelper.RunDotNetCommand(rootDir, "tool list", cancellationToken);

            if (output.Any(x => x.StartsWith("paket", StringComparison.CurrentCultureIgnoreCase)))
            {
                return "dotnet";
            }

            throw new FileNotFoundException("Unable to locate paket");
        }
    }
}