using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PaketChain
{
    internal class Sorter
    {
        public static void SortReferences(string rootDir)
        {
            foreach (var file in Directory.GetFiles(rootDir, "paket.references", SearchOption.AllDirectories))
            {
                Console.WriteLine($"Sorting reference file: {file}");
                var content = File.ReadAllLines(file);

                var newContent = new List<string>();
                var nugetBlock = new List<string>();
                var onNuget = false;

                foreach (var line in content)
                {
                    if (line.Trim().StartsWith("group"))
                    {
                        onNuget = false;
                    }
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        onNuget = true;
                    }

                    if (onNuget && !string.IsNullOrWhiteSpace(line))
                    {
                        nugetBlock.Add(line);
                    }

                    if (!onNuget)
                    {
                        if (nugetBlock.Any())
                        {
                            newContent.AddRange(nugetBlock.OrderBy(x => x));
                            nugetBlock.Clear();
                            newContent.Add("");
                        }

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            newContent.Add(line);
                        }
                    }
                }

                if (nugetBlock.Any())
                {
                    newContent.AddRange(nugetBlock.OrderBy(x => x));
                    newContent.Add("");
                }

                if (newContent.Count == 0)
                {
                    newContent.Add("");
                }

                File.WriteAllLines(file, newContent);
            }
        }

        public static void SortDependencies(string rootDir)
        {
            foreach (var file in Directory.GetFiles(rootDir, "paket.dependencies", SearchOption.AllDirectories))
            {
                Console.WriteLine($"Sorting dependencies file: {file}");
                var content = File.ReadAllLines(file);

                var newContent = new List<string>();
                var nugetBlock = new List<string>();
                var onNuget = false;

                foreach (var line in content)
                {
                    if (line.Trim().StartsWith("nuget") || line.Trim().StartsWith("clitool"))
                    {
                        onNuget = true;
                    }
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        onNuget = false;
                    }

                    if (onNuget && !string.IsNullOrWhiteSpace(line))
                    {
                        nugetBlock.Add(line);
                    }

                    if (!onNuget)
                    {
                        if (nugetBlock.Any())
                        {
                            newContent.Add("");
                            newContent.AddRange(nugetBlock.Where(x => x.Trim().StartsWith("nuget")).OrderBy(x => x));
                            newContent.AddRange(nugetBlock.Where(x => x.Trim().StartsWith("clitool")).OrderBy(x => x));
                            newContent.Add("");
                            nugetBlock.Clear();
                        }

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            newContent.Add(line);
                        }
                    }
                }

                if (nugetBlock.Any())
                {
                    newContent.Add("");
                    newContent.AddRange(nugetBlock.Where(x => x.Trim().StartsWith("nuget")).OrderBy(x => x));
                    newContent.AddRange(nugetBlock.Where(x => x.Trim().StartsWith("clitool")).OrderBy(x => x));
                    newContent.Add("");
                }

                if (newContent.Count == 0)
                {
                    newContent.Add("");
                }

                File.WriteAllLines(file, newContent);
            }
        }
    }
}