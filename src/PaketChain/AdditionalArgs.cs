namespace PaketChain
{
    internal class AdditionalArgs
    {
        public string PaketRedirectArgs { get; }
        public string PaketForceArgs { get; }
        public string PaketVerboseArgs { get; }
        public string DotnetVerboseArgs { get; }

        public AdditionalArgs(RunnerArgs runnerArgs)
        {
            PaketRedirectArgs = runnerArgs.Redirects ? "--redirects --create-new-binding-files --clean-redirects" : "--create-new-binding-files --clean-redirects";
            PaketForceArgs = runnerArgs.Force ? "--force" : "";
            PaketVerboseArgs = runnerArgs.Verbose ? "--verbose" : "";
            DotnetVerboseArgs = runnerArgs.Verbose ? "--verbosity d" : "";
        }
    }
}
