using McMaster.Extensions.CommandLineUtils;

namespace PaketChain
{
    internal class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<RunnerArgs>(args);
    }
}
