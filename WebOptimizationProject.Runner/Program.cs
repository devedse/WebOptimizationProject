using System;
using System.Threading.Tasks;

namespace WebOptimizationProject.Runner
{
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            Console.WriteLine("For this tool to work you need to have both GIT and HUB installed.");

            await GitHubRepositoryOptimizer.GoOptimize("Clowting", "OOPDraw");

            Console.WriteLine("Application finished, press any key to continue...");
            Console.ReadKey();
        }
    }
}
