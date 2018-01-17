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

            string owner = "vuejs";
            var repos = await GitHubRepositoryOptimizer.ObtainRepositoriesForOwner(owner);
            foreach (var repo in repos)
            {
                await GitHubRepositoryOptimizer.GoOptimize(owner, repo);
            }

            //await GitHubRepositoryOptimizer.GoOptimize("vuejs-templates", "webpack");
            //await GitHubRepositoryOptimizer.GoOptimize("vuejs-templates", "simple");
            //await GitHubRepositoryOptimizer.GoOptimize("vuejs-templates", "pwa");
            //await GitHubRepositoryOptimizer.GoOptimize("vuejs-templates", "browserify");
            //await GitHubRepositoryOptimizer.GoOptimize("vuejs-templates", "webpack-simple");
            //await GitHubRepositoryOptimizer.GoOptimize("vuejs-templates", "browserify-simple");

            Console.WriteLine("Application finished, press any key to continue...");
            Console.ReadKey();
        }
    }
}
