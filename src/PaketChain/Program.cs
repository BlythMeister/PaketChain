using McMaster.Extensions.CommandLineUtils;
using System;
using System.Diagnostics;

namespace PaketChain
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                Console.ResetColor();
                Console.Clear();
                return CommandLineApplication.Execute<RunnerArgs>(args);
            }
            finally
            {
                if (Debugger.IsAttached)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("");
                    Console.WriteLine("-----------------------------------------------------");
                    Console.WriteLine("Press Enter To Close Debugger");
                    Console.ReadLine();
                }

                Console.ResetColor();
            }
        }
    }
}
