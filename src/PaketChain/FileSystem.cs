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

        public static string LocatePaketFilePath(string rootDir, CancellationToken cancellationToken)
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
