using McMaster.Extensions.CommandLineUtils;
using System.Threading;

namespace PaketChain
{
    internal class RunnerArgs
    {
        [Option("-d|--dir <PATH>", "The path to a root of a repository, defaults to current directory if not provided (Note: <PATH> should be in quotes)", CommandOptionType.SingleValue)]
        public string Directory { get; }

        [Option("-upt|--update-paket-tool", "update the paket tool", CommandOptionType.NoValue)]
        public bool UpdateTool { get; }

        [Option("-upc|--update-paket-chain", "update the paket chain tool", CommandOptionType.NoValue)]
        public bool UpdateSelf { get; }

        [Option("-u|--update", "Include a paket update", CommandOptionType.NoValue)]
        public bool Update { get; }

        [Option("-ua|--update-args <ARGS>", "Args to pass to paket update (Note: <ARGS> should be in quotes)", CommandOptionType.SingleValue)]
        public string UpdateArgs { get; }

        [Option("-co|--clean-obj", "Clean obj folders to force a full update", CommandOptionType.NoValue)]
        public bool CleanObj { get; }

        [Option("-i|--install", "Include a paket install", CommandOptionType.NoValue)]
        public bool Install { get; }

        [Option("-ia|--install-args <ARGS>", "Args to pass to paket install (Note: <ARGS> should be in quotes)", CommandOptionType.SingleValue)]
        public string InstallArgs { get; }

        [Option("-r|--reinstall", "Delete the lock file and create from scratch", CommandOptionType.NoValue)]
        public bool Reinstall { get; }

        [Option("-s|--simplify", "Include a paket simplify", CommandOptionType.NoValue)]
        public bool Simplify { get; }

        [Option("-sa|--simplify-args <ARGS>", "Args to pass to paket simplify (Note: <ARGS> should be in quotes)", CommandOptionType.SingleValue)]
        public string SimplifyArgs { get; }

        [Option("-so|--sort", "Sort paket files alphabetically", CommandOptionType.NoValue)]
        public bool Sort { get; }

        [Option("-cc|--clear-cache", "Clear caches before running", CommandOptionType.NoValue)]
        public bool ClearCache { get; }

        [Option("-np|--no-prompt", "Never prompt user input", CommandOptionType.NoValue)]
        public bool NoPrompt { get; }

        [Option("-v|--verbose", "Verbose logging", CommandOptionType.NoValue)]
        public bool Verbose { get; }

        private int OnExecute(CancellationToken cancellationToken) => Runner.Start(this, cancellationToken);
    }
}
