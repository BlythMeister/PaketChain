using System;
using System.IO;
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
            FileSystem.ValidatePaths(rootDir);
            Console.WriteLine($"Running against: {rootDir}");
            Console.WriteLine("-----------------------------------------------------");
            if (cancellationToken.IsCancellationRequested) return -2;

            Console.WriteLine("Running tool restore");
            ConsoleHelper.RunDotNetCommand(rootDir, "tool restore", cancellationToken);
            Console.WriteLine("-----------------------------------------------------");
            if (cancellationToken.IsCancellationRequested) return -2;

            var (paketPath, toolType) = FileSystem.LocatePaketFilePath(rootDir, cancellationToken);

            if (runnerArgs.UpdateTool)
            {
                switch (toolType)
                {
                    case PaketType.LocalTool:
                        ConsoleHelper.RunDotNetCommand(rootDir, "tool update paket", cancellationToken);
                        break;

                    case PaketType.GlobalTool:
                        ConsoleHelper.RunDotNetCommand(rootDir, "tool update paket --global", cancellationToken);
                        break;

                    default:
                        Console.WriteLine("Cannot update local exe using paket chain");
                        break;
                }
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

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
                ConsoleHelper.RunPaketCommand(rootDir, paketPath, toolType, "clear-cache", "--clear-local", cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

            if (runnerArgs.CleanObj)
            {
                FileSystem.CleanObjFiles(rootDir);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

            if (runnerArgs.Reinstall)
            {
                Console.WriteLine("Deleting paket.lock file");
                File.Delete(Path.Combine(rootDir, "paket.lock"));
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

            if (runnerArgs.Update)
            {
                if (runnerArgs.Reinstall)
                {
                    Console.WriteLine("Skipping Update as reinstall install newest versions");
                }

                ConsoleHelper.RunPaketCommand(rootDir, paketPath, toolType, "update", runnerArgs.UpdateArgs, cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

            if (runnerArgs.Simplify)
            {
                ConsoleHelper.RunPaketCommand(rootDir, paketPath, toolType, "simplify", runnerArgs.SimplifyArgs, cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

            if (runnerArgs.Install || runnerArgs.Reinstall || runnerArgs.Simplify)
            {
                ConsoleHelper.RunPaketCommand(rootDir, paketPath, toolType, "install", runnerArgs.InstallArgs, cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

            return 0;
        }
    }
}
