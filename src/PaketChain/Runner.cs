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
            var paketPath = FileSystem.LocatePaketFilePath(rootDir, cancellationToken);

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
                FileSystem.CleanObjFiles(rootDir);
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
    }
}
