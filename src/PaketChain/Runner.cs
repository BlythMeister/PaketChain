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
                if (runnerArgs.PromptClose && !Debugger.IsAttached)
                {
                    Console.WriteLine("Press Enter To Close...");
                    Console.ReadLine();
                }
            }
        }

        private static int Run(RunnerArgs runnerArgs, CancellationToken cancellationToken)
        {
            var (rootDir, paketInfo, additionalArgs) = GetRunInfomation(runnerArgs, cancellationToken);
            if (cancellationToken.IsCancellationRequested) return -2;

            RestoreLocalTool(cancellationToken, rootDir, paketInfo, additionalArgs);
            if (cancellationToken.IsCancellationRequested) return -2;

            UpdatePaketTool(runnerArgs, cancellationToken, rootDir, paketInfo, additionalArgs);
            if (cancellationToken.IsCancellationRequested) return -2;

            CacheClean(runnerArgs, cancellationToken, rootDir, paketInfo, additionalArgs);
            if (cancellationToken.IsCancellationRequested) return -2;

            GitClean(runnerArgs, cancellationToken, rootDir);
            if (cancellationToken.IsCancellationRequested) return -2;

            ObjClean(runnerArgs, rootDir);
            if (cancellationToken.IsCancellationRequested) return -2;

            PaketAdd(runnerArgs, cancellationToken, rootDir, paketInfo, additionalArgs);
            if (cancellationToken.IsCancellationRequested) return -2;

            PaketRemove(runnerArgs, cancellationToken, rootDir, paketInfo, additionalArgs);
            if (cancellationToken.IsCancellationRequested) return -2;

            SortPaketFiles(runnerArgs, rootDir);
            if (cancellationToken.IsCancellationRequested) return -2;

            Reinstall(runnerArgs, rootDir);
            if (cancellationToken.IsCancellationRequested) return -2;

            PaketUpdate(runnerArgs, cancellationToken, rootDir, paketInfo, additionalArgs);
            if (cancellationToken.IsCancellationRequested) return -2;

            PaketSimplify(runnerArgs, cancellationToken, rootDir, paketInfo, additionalArgs);
            if (cancellationToken.IsCancellationRequested) return -2;

            PaketInstall(runnerArgs, cancellationToken, rootDir, paketInfo, additionalArgs);
            if (cancellationToken.IsCancellationRequested) return -2;

            PaketRestore(runnerArgs, cancellationToken, rootDir, paketInfo, additionalArgs);
            if (cancellationToken.IsCancellationRequested) return -2;

            return 0;
        }

        private static (string rootDir, PaketInfo paketInfo, AdditionalArgs additionalArgs) GetRunInfomation(RunnerArgs runnerArgs, CancellationToken cancellationToken)
        {
            var rootDir = string.IsNullOrWhiteSpace(runnerArgs.Directory) ? Environment.CurrentDirectory : runnerArgs.Directory;
            FileSystem.ValidatePaths(rootDir);
            Console.WriteLine($"Running against: {rootDir}");
            var paketInfo = FileSystem.LocatePaketFilePath(rootDir, cancellationToken);
            Console.WriteLine($"Will run paket using: {paketInfo.PaketPath}");
            Console.WriteLine($"paket mode: {paketInfo.ToolType}");
            var additionalArgs = new AdditionalArgs(runnerArgs);
            Console.WriteLine("-----------------------------------------------------");
            return (rootDir, paketInfo, additionalArgs);
        }

        private static void RestoreLocalTool(CancellationToken cancellationToken, string rootDir, PaketInfo paketInfo, AdditionalArgs additionalArgs)
        {
            if (paketInfo.ToolType == PaketType.LocalTool)
            {
                Console.WriteLine("Running tool restore");
                ConsoleHelper.RunDotNetToolCommand(rootDir, $"restore {additionalArgs.DotnetVerboseArgs}", cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
            }
        }

        private static void UpdatePaketTool(RunnerArgs runnerArgs, CancellationToken cancellationToken, string rootDir, PaketInfo paketInfo, AdditionalArgs additionalArgs)
        {
            if (runnerArgs.UpdateTool)
            {
                switch (paketInfo.ToolType)
                {
                    case PaketType.LocalTool:
                        ConsoleHelper.RunDotNetToolCommand(rootDir, $"update paket {additionalArgs.DotnetVerboseArgs}", cancellationToken);
                        break;

                    case PaketType.GlobalTool:
                        ConsoleHelper.RunDotNetToolCommand(rootDir, $"update paket --global {additionalArgs.DotnetVerboseArgs}", cancellationToken);
                        break;

                    default:
                        Console.WriteLine("Cannot update local exe using paket chain");
                        break;
                }

                Console.WriteLine("-----------------------------------------------------");
            }
        }

        private static void PaketAdd(RunnerArgs runnerArgs, CancellationToken cancellationToken, string rootDir, PaketInfo paketInfo, AdditionalArgs additionalArgs)
        {
            if (!string.IsNullOrWhiteSpace(runnerArgs.AddPackageInteractive))
            {
                ConsoleHelper.RunPaketCommand(rootDir, paketInfo.PaketPath, paketInfo.ToolType, "add", $"{runnerArgs.AddPackage} --interactive {additionalArgs.PaketRedirectArgs} {additionalArgs.PaketForceArgs} {additionalArgs.PaketVerboseArgs} {runnerArgs.AddAdditionalArgs}", cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
            }
            else if (!string.IsNullOrWhiteSpace(runnerArgs.AddPackage))
            {
                if (runnerArgs.AddProjects != null && runnerArgs.AddProjects.Any(x => !string.IsNullOrWhiteSpace(x)))
                {
                    foreach (var addProject in runnerArgs.AddProjects.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        ConsoleHelper.RunPaketCommand(rootDir, paketInfo.PaketPath, paketInfo.ToolType, "add", $"{runnerArgs.AddPackage} --project {addProject} {additionalArgs.PaketRedirectArgs} {additionalArgs.PaketForceArgs} {additionalArgs.PaketVerboseArgs} {runnerArgs.AddAdditionalArgs}", cancellationToken);
                        Console.WriteLine("-----------------------------------------------------");
                    }
                }
                else
                {
                    ConsoleHelper.RunPaketCommand(rootDir, paketInfo.PaketPath, paketInfo.ToolType, "add", $"{runnerArgs.AddPackage} {additionalArgs.PaketRedirectArgs} {additionalArgs.PaketForceArgs} {additionalArgs.PaketVerboseArgs} {runnerArgs.AddAdditionalArgs}", cancellationToken);
                    Console.WriteLine("-----------------------------------------------------");
                }
            }
        }

        private static void PaketRemove(RunnerArgs runnerArgs, CancellationToken cancellationToken, string rootDir, PaketInfo paketInfo, AdditionalArgs additionalArgs)
        {
            if (!string.IsNullOrWhiteSpace(runnerArgs.RemovePackageInteractive))
            {
                ConsoleHelper.RunPaketCommand(rootDir, paketInfo.PaketPath, paketInfo.ToolType, "remove", $"{runnerArgs.RemovePackage} --interactive {additionalArgs.PaketForceArgs} {additionalArgs.PaketVerboseArgs} {runnerArgs.RemoveAdditionalArgs}", cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
            }
            else if (!string.IsNullOrWhiteSpace(runnerArgs.RemovePackage))
            {
                if (runnerArgs.RemoveProjects != null && runnerArgs.RemoveProjects.Any(x => !string.IsNullOrWhiteSpace(x)))
                {
                    foreach (var removeProject in runnerArgs.RemoveProjects.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        ConsoleHelper.RunPaketCommand(rootDir, paketInfo.PaketPath, paketInfo.ToolType, "remove", $"{runnerArgs.RemovePackage} --project {removeProject} {additionalArgs.PaketForceArgs} {additionalArgs.PaketVerboseArgs} {runnerArgs.RemoveAdditionalArgs}", cancellationToken);
                        Console.WriteLine("-----------------------------------------------------");
                    }
                }
                else
                {
                    ConsoleHelper.RunPaketCommand(rootDir, paketInfo.PaketPath, paketInfo.ToolType, "remove", $"{runnerArgs.RemovePackage} {additionalArgs.PaketForceArgs} {additionalArgs.PaketVerboseArgs} {runnerArgs.RemoveAdditionalArgs}", cancellationToken);
                    Console.WriteLine("-----------------------------------------------------");
                }
            }
        }

        private static void SortPaketFiles(RunnerArgs runnerArgs, string rootDir)
        {
            if (runnerArgs.Sort)
            {
                Sorter.SortReferences(rootDir);
                Console.WriteLine("-----------------------------------------------------");
                Sorter.SortDependencies(rootDir);
                Console.WriteLine("-----------------------------------------------------");
            }
        }

        private static void CacheClean(RunnerArgs runnerArgs, CancellationToken cancellationToken, string rootDir, PaketInfo paketInfo, AdditionalArgs additionalArgs)
        {
            if (runnerArgs.ClearCache)
            {
                ConsoleHelper.RunPaketCommand(rootDir, paketInfo.PaketPath, paketInfo.ToolType, "clear-cache", $"--clear-local {additionalArgs.PaketVerboseArgs}", cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
            }
        }

        private static void GitClean(RunnerArgs runnerArgs, CancellationToken cancellationToken, string rootDir)
        {
            if (runnerArgs.GitClean)
            {
                ConsoleHelper.RunGitCommand(rootDir, "clean", "-dfx", cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
            }
        }

        private static void ObjClean(RunnerArgs runnerArgs, string rootDir)
        {
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
                }
            }
        }

        private static void Reinstall(RunnerArgs runnerArgs, string rootDir)
        {
            if (runnerArgs.Reinstall)
            {
                Console.WriteLine("Deleting paket.lock file");
                File.Delete(Path.Combine(rootDir, "paket.lock"));
                Console.WriteLine("-----------------------------------------------------");
            }
        }

        private static void PaketUpdate(RunnerArgs runnerArgs, CancellationToken cancellationToken, string rootDir, PaketInfo paketInfo, AdditionalArgs additionalArgs)
        {
            if ((!string.IsNullOrWhiteSpace(runnerArgs.UpdatePackage) || runnerArgs.Update) && runnerArgs.Reinstall)
            {
                Console.WriteLine("Skipping Update as reinstall install newest versions");
                Console.WriteLine("-----------------------------------------------------");
                return;
            }

            if (!string.IsNullOrWhiteSpace(runnerArgs.UpdatePackage))
            {
                ConsoleHelper.RunPaketCommand(rootDir, paketInfo.PaketPath, paketInfo.ToolType, "update", $"{runnerArgs.UpdatePackage} {additionalArgs.PaketRedirectArgs} {additionalArgs.PaketForceArgs} {additionalArgs.PaketVerboseArgs} {runnerArgs.UpdateArgs}", cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
            }
            else if (runnerArgs.Update)
            {
                ConsoleHelper.RunPaketCommand(rootDir, paketInfo.PaketPath, paketInfo.ToolType, "update", $"{additionalArgs.PaketRedirectArgs} {additionalArgs.PaketForceArgs} {additionalArgs.PaketVerboseArgs} {runnerArgs.UpdateArgs}", cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
            }
        }

        private static void PaketSimplify(RunnerArgs runnerArgs, CancellationToken cancellationToken, string rootDir, PaketInfo paketInfo, AdditionalArgs additionalArgs)
        {
            if (runnerArgs.SimplifyInteractive || runnerArgs.Simplify)
            {
                if (runnerArgs.Reinstall || (string.IsNullOrWhiteSpace(runnerArgs.UpdatePackage) && !runnerArgs.Update))
                {
                    Console.WriteLine("Need to install before we can simplify");
                    Console.WriteLine("-----------------------------------------------------");
                    PaketInstall(runnerArgs, cancellationToken, rootDir, paketInfo, additionalArgs);
                }

                if (runnerArgs.SimplifyInteractive)
                {
                    ConsoleHelper.RunPaketCommand(rootDir, paketInfo.PaketPath, paketInfo.ToolType, "simplify", $"--interactive {additionalArgs.PaketVerboseArgs} {runnerArgs.SimplifyArgs}", cancellationToken);
                    Console.WriteLine("-----------------------------------------------------");
                }
                else if (runnerArgs.Simplify)
                {
                    ConsoleHelper.RunPaketCommand(rootDir, paketInfo.PaketPath, paketInfo.ToolType, "simplify", $"{additionalArgs.PaketVerboseArgs} {runnerArgs.SimplifyArgs}", cancellationToken);
                    Console.WriteLine("-----------------------------------------------------");
                }
            }
        }

        private static void PaketInstall(RunnerArgs runnerArgs, CancellationToken cancellationToken, string rootDir, PaketInfo paketInfo, AdditionalArgs additionalArgs)
        {
            if (runnerArgs.Install || runnerArgs.Reinstall || runnerArgs.Simplify || runnerArgs.SimplifyInteractive)
            {
                ConsoleHelper.RunPaketCommand(rootDir, paketInfo.PaketPath, paketInfo.ToolType, "install", $"{additionalArgs.PaketRedirectArgs} {additionalArgs.PaketForceArgs} {additionalArgs.PaketVerboseArgs} {runnerArgs.InstallArgs}", cancellationToken);
                Console.WriteLine("-----------------------------------------------------");
            }
        }

        private static void PaketRestore(RunnerArgs runnerArgs, CancellationToken cancellationToken, string rootDir, PaketInfo paketInfo, AdditionalArgs additionalArgs)
        {
            if (runnerArgs.Restore)
            {
                if (runnerArgs.Install || runnerArgs.Reinstall || runnerArgs.Simplify || runnerArgs.SimplifyInteractive)
                {
                    Console.WriteLine("Skipping Restore as already installed");
                    Console.WriteLine("-----------------------------------------------------");
                }
                else
                {
                    ConsoleHelper.RunPaketCommand(rootDir, paketInfo.PaketPath, paketInfo.ToolType, "restore", $"{additionalArgs.PaketForceArgs} {additionalArgs.PaketVerboseArgs} {runnerArgs.RestoreArgs}", cancellationToken);
                    Console.WriteLine("-----------------------------------------------------");
                }
            }
        }
    }
}
