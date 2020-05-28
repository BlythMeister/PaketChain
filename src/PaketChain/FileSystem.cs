using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace PaketChain
{
    internal class FileSystem
    {
        public static void CleanObjFiles(string rootDir)
        {
            foreach (var paketProps in new DirectoryInfo(rootDir).GetFiles("*.paket.props", SearchOption.AllDirectories))
            {
                if (paketProps.Directory != null)
                {
                    Console.WriteLine($"Cleaning files in {paketProps.Directory.FullName}");
                    foreach (var file in paketProps.Directory?.GetFiles())
                    {
                        file.Delete();
                    }
                }
            }
        }

        public static void ValidatePaths(string rootDir)
        {
            if (!Directory.Exists(rootDir))
            {
                throw new DirectoryNotFoundException($"Unable to locate directory {rootDir}");
            }
        }

        public static PaketInfo LocatePaketFilePath(string rootDir, CancellationToken cancellationToken)
        {
            if (Directory.Exists(Path.Combine(rootDir, ".paket")) && File.Exists(Path.Combine(rootDir, ".paket", "paket.exe")))
            {
                return new PaketInfo(Path.Combine(rootDir, ".paket", "paket.exe"), PaketType.Exe);
            }

            var output = ConsoleHelper.RunDotNetToolCommandWithOutput(rootDir, "list", cancellationToken);

            if (output != null && output.Where(x => x != null).Any(x => x.StartsWith("paket", StringComparison.CurrentCultureIgnoreCase)))
            {
                return new PaketInfo("dotnet", PaketType.LocalTool);
            }

            var outputGlobal = ConsoleHelper.RunDotNetToolCommandWithOutput(rootDir, "list --global", cancellationToken);

            if (outputGlobal != null && outputGlobal.Where(x => x != null).Any(x => x.StartsWith("paket", StringComparison.CurrentCultureIgnoreCase)))
            {
                return new PaketInfo("paket", PaketType.GlobalTool);
            }

            throw new FileNotFoundException("Unable to locate paket");
        }
    }
}
