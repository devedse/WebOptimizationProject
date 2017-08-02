using DeveImageOptimizer;
using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            //Gogo("desjoerd", "sdfg-aspnetcore").Wait();
            Gogo("desjoerd", "test-image-optimization").Wait();

            //Testje().Wait();

        }

        public static async Task Testje()
        {
            Directory.SetCurrentDirectory(@"C:\XGit\WebOptimizationProject\WebOptimizationProject\bin\Debug\netcoreapp1.1\ClonedRepos\sdfg-aspnetcore");

            var config = ConfigHelper.GetConfig();
            var git = new GitHandler(config);


            File.WriteAllText("Testje.txt", $"{DateTime.Now}: This is just a test file. Don't accept my pull request, it's just for testing.");


            var files = new List<OptimizedFileResult>() { new OptimizedFileResult("test.png", true, 150, 30, new List<string>()) };

            var commitDesc = await TemplatesHandler.GetDescriptionForCommit();

            await git.RunHubCommand("add .");
            await git.RunHubCommand("status");
            await git.Commit("Wop optimized this repository", commitDesc);
            await git.RunHubCommand("push thefork");

            var desc = await TemplatesHandler.GetDescriptionForPullRequest(files);

            await git.PullRequest("The Web Optimization Project has optimized your repository!", desc);
        }

        private static async Task<IEnumerable<OptimizedFileResult>> GoOptimizeStub(string dir, Config config)
        {
            var testjeFile = Path.Combine(dir, "Testje.txt");
            File.WriteAllText(testjeFile, $"{DateTime.Now}: This is just a test file. Don't accept my pull request, it's just for testing.");

            var lijstje = new List<OptimizedFileResult>();

            lijstje.Add(new OptimizedFileResult(testjeFile, true, 1000000, 900000, new List<string>()));

            await Task.Delay(10);

            return lijstje;
        }

        public static async Task Gogo(string repositoryOwner, string repositoryName)
        {
            var config = ConfigHelper.GetConfig();

            var dirOfClonedRepos = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, config.ClonedRepositoriesDirectoryName);
            Directory.CreateDirectory(dirOfClonedRepos);
            Directory.SetCurrentDirectory(dirOfClonedRepos);
            var git = new GitHandler(config);

            //var clonedRepo = Path.Combine(Directory.GetCurrentDirectory(), repositoryName);

            var clonedRepo = await git.GitClone(dirOfClonedRepos, repositoryOwner, repositoryName);
            Directory.SetCurrentDirectory(clonedRepo);

            string featureName = "WebOptimizationProject";


            await git.RunHubCommand("fork");

            await git.RunHubCommand($"remote set-url {config.GithubUserName} https://github.com/{config.GithubUserName}/{repositoryName}.git");
            //await git.RunHubCommand($"remote add thefork https://github.com/{config.GithubUserName}/{repositoryName}.git");

            //Fetch everything in my repository
            await git.RunHubCommand("fetch --all");

            //Go to master
            await git.RunHubCommand($"checkout {config.GithubUserName}/master");
            await git.RunHubCommand($"merge --strategy-option=theirs origin/master");
            await git.RunHubCommand($"push {config.GithubUserName} {config.GithubUserName}/master");

            //var createdBranch = await git.RunHubCommand($"branch {featureName}");

            var branchAlreadyExists = await git.RunHubCommand($"checkout --track -b {featureName} {config.GithubUserName}/{featureName}");

            if (branchAlreadyExists == 0)
            {
                //await git.RunHubCommand($"checkout {config.GithubUserName}/WebOptimizationProject");
                await git.RunHubCommand($"merge --strategy-option=theirs {config.GithubUserName}/master");
                await git.RunHubCommand($"push {config.GithubUserName} {featureName} -u");
            }
            else
            {
                await git.RunHubCommand($"checkout -b {featureName}");
                await git.RunHubCommand($"push {config.GithubUserName} {featureName} -u");
            }

            var optimizedFileResults = await GoOptimize(clonedRepo, config);
            //var optimizedFileResults = await GoOptimizeStub(clonedRepo, config);

            await git.RunHubCommand("add .");

            var descriptionForCommit = await TemplatesHandler.GetDescriptionForCommit();
            await git.Commit("Wop optimized this repository", descriptionForCommit);
            await git.RunHubCommand($"push");


            var descriptionForPullRequest = await TemplatesHandler.GetDescriptionForPullRequest(optimizedFileResults);

            var pullRequestState = await git.PullRequest("The Web Optimization Project has optimized your repository!", descriptionForPullRequest);

            Console.WriteLine("Pullrequeststate: " + pullRequestState);

            if (pullRequestState == 1)
            {
                //Do an update of the pull request instead.
            }

            //if (string.Equals(repositoryOwner, config.GithubUserName, StringComparison.OrdinalIgnoreCase))
            //{
            //    //This is a repository from me, so we don't want to fork it.
            //    await git.RunHubCommand($"push origin HEAD:{featureName}");
            //    await git.PullRequest("The Web Optimization Project has optimized your repository!", descriptionForPullRequest);
            //}
            //else
            //{



            //}

            //await git.RunGitCommand("push --set-upstream origin WebOptimizationProject");
            //await git.RunGitCommand($"request-pull master https://github.com/devedse/ImageTest.git {featureName}");

            //Git add .
            //Git commit -m "hoi"
            //git 
        }

        private static async Task<IEnumerable<OptimizedFileResult>> GoOptimize(string dir, Config config)
        {
            var fileOptimizer = new FileOptimizerProcessor(config.FileOptimizerFullExePath, FolderHelperMethods.TempDirectory.Value);
            var fileProcessor = new FileProcessor(fileOptimizer, null);
            var optimizedFileResults = await fileProcessor.ProcessDirectory(dir);

            return optimizedFileResults;
        }
    }
}