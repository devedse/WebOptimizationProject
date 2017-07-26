using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebOptimizationProject.Configuration;
using WebOptimizationProject.Helpers;

namespace WebOptimizationProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("For this tool to work you need to have both GIT and HUB installed.");

            //Gogo("devedse", "ImageTest").Wait();
            Gogo("desjoerd", "sdfg-aspnetcore").Wait();



        }

        public static async Task Gogo(string repositoryOwner, string repositoryName)
        {
            var config = ConfigHelper.GetConfig();

            var dirOfClonedRepos = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, config.ClonedRepositoriesDirectoryName);
            Directory.CreateDirectory(dirOfClonedRepos);
            Directory.SetCurrentDirectory(dirOfClonedRepos);
            var git = new GitHandler(config);

            var clonedRepo = Path.Combine(Directory.GetCurrentDirectory(), repositoryName);

            if (true)
            {
                clonedRepo = await git.GitClone(repositoryOwner, repositoryName);
                var worked = await GoOptimize(clonedRepo, config);
            }
            string featureName = "WebOptimizationProject";





            Directory.SetCurrentDirectory(clonedRepo);
            await git.RunHubCommand($"checkout -b {featureName}");
            await git.RunHubCommand("add .");
            await git.RunHubCommand("commit -m \"Wop losslessly compressed your images.\"");

            if (string.Equals(repositoryOwner, config.GithubUserName, StringComparison.OrdinalIgnoreCase))
            {
                //This is a repository from me, so we don't want to fork it.
                await git.RunHubCommand($"push origin HEAD:WebOptimizationProject");
                await git.RunHubCommand("pull-request -m \"The Web Optimization Project compressed all your images!\"");

            }
            else
            {
                await git.RunHubCommand("fork");
                await git.RunHubCommand($"push origin HEAD:WebOptimizationProject");
                await git.RunHubCommand("pull-request -m \"The Web Optimization Project compressed all your images!\"");
            }

            //await git.RunGitCommand("push --set-upstream origin WebOptimizationProject");
            //await git.RunGitCommand($"request-pull master https://github.com/devedse/ImageTest.git {featureName}");

            //Git add .
            //Git commit -m "hoi"
            //git 
        }

        private static async Task<bool> GoOptimize(string dir, Config config)
        {
            var processingState = new FilesProcessingState();

            var fileOptimizer = new FileOptimizerProcessor(config.FileOptimizerFullExePath, FolderHelperMethods.TempDirectoryForTests.Value);
            var fileProcessor = new FileProcessor(fileOptimizer, processingState);
            await fileProcessor.ProcessDirectory(dir);

            return processingState.FailedFiles.Any();
        }
    }
}