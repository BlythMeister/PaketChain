using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace PaketChain
{
    internal class Runner
    {
        public static int Start(RunnerArgs runnerArgs, CancellationToken cancellationToken)
        {
            try
            {
                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var version = FileVersionInfo.GetVersionInfo(assemblyLocation).ProductVersion;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("");
                Console.WriteLine("----------------------------");
                Console.WriteLine("|                          |");
                Console.WriteLine("|        Paket Chain       |");
                Console.WriteLine($"|{version.PadLeft(10 + version.Length).PadRight(26)}|");
                Console.WriteLine("|                          |");
                Console.WriteLine("|    Author: Chris Blyth   |");
                Console.WriteLine("|                          |");
                Console.WriteLine("----------------------------");
                Console.WriteLine("");
                Console.WriteLine($"Starting at {DateTime.UtcNow:u}");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;
                return Run(runnerArgs, cancellationToken);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.White;
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
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("-----------------------------------------------------");
                return -1;
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Done at {DateTime.UtcNow:u}");
                if (!runnerArgs.NoPrompt && !Debugger.IsAttached)
                {
                    Console.WriteLine("Press Enter To Close...");
                    Console.ReadLine();
                }
            }
        }

        private static int Run(RunnerArgs runnerArgs, CancellationToken cancellationToken)
        {
            var rootDir = string.IsNullOrWhiteSpace(runnerArgs.Directory) ? Environment.CurrentDirectory : runnerArgs.Directory;
            FileSystem.ValidatePaths(rootDir);
            Console.WriteLine($"Running against: {rootDir}");
            Console.WriteLine("-----------------------------------------------------");
            if (cancellationToken.IsCancellationRequested) return -2;

            var (paketPath, toolType) = FileSystem.LocatePaketFilePath(rootDir, cancellationToken);

            if (toolType == PaketType.LocalTool)
            {
                Console.WriteLine("Running tool restore");
                ConsoleHelper.RunDotNetCommand(rootDir, "tool restore", cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

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

            if (!string.IsNullOrWhiteSpace(runnerArgs.AddPackage))
            {
                if (runnerArgs.AddProjects != null && runnerArgs.AddProjects.Any(x => !string.IsNullOrWhiteSpace(x)))
                {
                    foreach (var addProject in runnerArgs.AddProjects.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        ConsoleHelper.RunPaketCommand(rootDir, paketPath, toolType, "add", $"{runnerArgs.AddPackage} --project {addProject} {runnerArgs.AddAdditionalArgs}", cancellationToken);
                        Console.WriteLine("-----------------------------------------------------");
                        if (cancellationToken.IsCancellationRequested) return -2;
                    }
                }
                else
                {
                    ConsoleHelper.RunPaketCommand(rootDir, paketPath, toolType, "add", $"{runnerArgs.AddPackage} {runnerArgs.AddAdditionalArgs}", cancellationToken);
                    Console.WriteLine("-----------------------------------------------------");
                    if (cancellationToken.IsCancellationRequested) return -2;
                }
            }

            if (!string.IsNullOrWhiteSpace(runnerArgs.RemovePackage))
            {
                if (runnerArgs.RemoveProjects != null && runnerArgs.RemoveProjects.Any(x => !string.IsNullOrWhiteSpace(x)))
                {
                    foreach (var removeProject in runnerArgs.RemoveProjects.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        ConsoleHelper.RunPaketCommand(rootDir, paketPath, toolType, "remove", $"{runnerArgs.RemovePackage} --project {removeProject} {runnerArgs.RemoveAdditionalArgs}", cancellationToken);
                        Console.WriteLine("-----------------------------------------------------");
                        if (cancellationToken.IsCancellationRequested) return -2;
                    }
                }
                else
                {
                    ConsoleHelper.RunPaketCommand(rootDir, paketPath, toolType, "remove", $"{runnerArgs.RemovePackage} {runnerArgs.RemoveAdditionalArgs}", cancellationToken);
                    Console.WriteLine("-----------------------------------------------------");
                    if (cancellationToken.IsCancellationRequested) return -2;
                }
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

            if (runnerArgs.GitClean)
            {
                ConsoleHelper.RunGitCommand(rootDir, "clean", "-dfx", cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
                if (cancellationToken.IsCancellationRequested) return -2;
            }

            if (runnerArgs.CleanObj)
            {
                if (runnerArgs.GitClean)
                {
                    Console.WriteLine("Skipping Clean Objects as Git clean run");
                    Console.WriteLine("-----------------------------------------------------");
                }
                else
                {
                    FileSystem.CleanObjFiles(rootDir);
                    Console.WriteLine("-----------------------------------------------------");
                    if (cancellationToken.IsCancellationRequested) return -2;
                }
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
                    Console.WriteLine("-----------------------------------------------------");
                }
                else
                {
                    ConsoleHelper.RunPaketCommand(rootDir, paketPath, toolType, "update", runnerArgs.UpdateArgs, cancellationToken);
                    Console.WriteLine("-----------------------------------------------------");
                    if (cancellationToken.IsCancellationRequested) return -2;
                }
            }

            if (runnerArgs.Simplify)
            {
                if (runnerArgs.Reinstall)
                {
                    ConsoleHelper.RunPaketCommand(rootDir, paketPath, toolType, "install", runnerArgs.InstallArgs, cancellationToken);
                    Console.WriteLine("-----------------------------------------------------");
                    if (cancellationToken.IsCancellationRequested) return -2;
                }

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

            if (runnerArgs.Restore)
            {
                if (runnerArgs.Install || runnerArgs.Reinstall || runnerArgs.Simplify)
                {
                    Console.WriteLine("Skipping Restore as already installed");
                    Console.WriteLine("-----------------------------------------------------");
                }
                else
                {
                    ConsoleHelper.RunPaketCommand(rootDir, paketPath, toolType, "restore", runnerArgs.RestoreArgs, cancellationToken);
                    Console.WriteLine("-----------------------------------------------------");
                    if (cancellationToken.IsCancellationRequested) return -2;
                }
            }

            return 0;
        }
    }
}
