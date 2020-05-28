// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable UnusedMember.Local

using McMaster.Extensions.CommandLineUtils;
using System.Threading;

namespace PaketChain
{
    internal class RunnerArgs
    {
        [Option("-d|--dir <PATH>", "The path to a root of a repository, defaults to current directory if not provided (Note: <PATH> should be in quotes)", CommandOptionType.SingleValue)]
        public string Directory { get; }

        [Option("-u|--update", "Run a paket update", CommandOptionType.NoValue)]
        public bool Update { get; }

        [Option("-ua|--update-args <ARGS>", "Args to pass to paket update (Note: <ARGS> should be in quotes)", CommandOptionType.SingleValue)]
        public string UpdateArgs { get; }

        [Option("-i|--install", "Run a paket install", CommandOptionType.NoValue)]
        public bool Install { get; }

        [Option("-ia|--install-args <ARGS>", "Args to pass to paket install (Note: <ARGS> should be in quotes)", CommandOptionType.SingleValue)]
        public string InstallArgs { get; }

        [Option("-a|--add <PackageName>", "Run a paket add for package", CommandOptionType.SingleValue)]
        public string AddPackage { get; }

        [Option("-ap|--add-project <ProjectName>", "Projects to add package to. (Requires -a/--add with package)", CommandOptionType.MultipleValue)]
        public string[] AddProjects { get; }

        [Option("-aa|--add-args <ARGS>", "Args to pass to paket add (Note: <ARGS> should be in quotes)", CommandOptionType.SingleValue)]
        public string AddAdditionalArgs { get; }

        [Option("-re|--remove <PackageName>", "Run a paket remove for package", CommandOptionType.SingleValue)]
        public string RemovePackage { get; }

        [Option("-rep|--remove-project <ProjectName>", "Projects to remove package from. (Requires -re/--remove with package) (Note: can be used multiple times for multiple projects)", CommandOptionType.MultipleValue)]
        public string[] RemoveProjects { get; }

        [Option("-rea|--remove-args <ARGS>", "Args to pass to paket remove (Note: <ARGS> should be in quotes)", CommandOptionType.SingleValue)]
        public string RemoveAdditionalArgs { get; }

        [Option("-r|--restore", "Run a paket restore", CommandOptionType.NoValue)]
        public bool Restore { get; }

        [Option("-ra|--restore-args <ARGS>", "Args to pass to paket restore (Note: <ARGS> should be in quotes)", CommandOptionType.SingleValue)]
        public string RestoreArgs { get; }

        [Option("-s|--simplify", "Run a paket simplify", CommandOptionType.NoValue)]
        public bool Simplify { get; }

        [Option("-sa|--simplify-args <ARGS>", "Args to pass to paket simplify (Note: <ARGS> should be in quotes)", CommandOptionType.SingleValue)]
        public string SimplifyArgs { get; }

        [Option("-ri|--reinstall", "Delete the lock file and create from scratch", CommandOptionType.NoValue)]
        public bool Reinstall { get; }

        [Option("-so|--sort", "Sort paket files alphabetically", CommandOptionType.NoValue)]
        public bool Sort { get; }

        [Option("-cc|--clear-cache", "Clear caches before running", CommandOptionType.NoValue)]
        public bool ClearCache { get; }

        [Option("-gc|--git-clean", "Run git clean", CommandOptionType.NoValue)]
        public bool GitClean { get; }

        [Option("-co|--clean-obj", "Clean obj folders to force a full update", CommandOptionType.NoValue)]
        public bool CleanObj { get; }

        [Option("-upt|--update-paket-tool", "update the paket tool", CommandOptionType.NoValue)]
        public bool UpdateTool { get; }

        [Option("-np|--no-prompt", "Never prompt user input", CommandOptionType.NoValue)]
        public bool NoPrompt { get; }

        [Option("-v|--verbose", "Verbose logging", CommandOptionType.NoValue)]
        public bool Verbose { get; }

        private int OnExecute(CancellationToken cancellationToken) => Runner.Start(this, cancellationToken);
    }
}
