using DeveImageOptimizer.FileProcessing;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using WebOptimizationProject.Configuration;

namespace WebOptimizationProject.Runner
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true, true);

            var configuration = builder.Build();

            Console.WriteLine("For this tool to work you need to have both GIT and HUB installed.");

            var wopConfig = new WopConfig();
            configuration.Bind(wopConfig);

            var deveImageOptimizerConfiguration = new DeveImageOptimizerConfiguration()
            {
                ExecuteImageOptimizationParallel = true,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            var githubRepositoryOptimizer = new GitHubRepositoryOptimizer(wopConfig, deveImageOptimizerConfiguration, new WopProgressReporter());
            //await githubRepositoryOptimizer.GoOptimize("WebOptimizationProject", "TestRepo1");
            await githubRepositoryOptimizer.GoOptimize(282345207L);

            //string owner = "vuejs";
            //var repos = await GitHubRepositoryOptimizer.ObtainRepositoriesForOwner(owner);
            //foreach (var repo in repos)
            //{
            //    await GitHubRepositoryOptimizer.GoOptimize(owner, repo);
            //}

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
