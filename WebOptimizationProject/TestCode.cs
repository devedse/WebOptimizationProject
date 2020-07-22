using DeveImageOptimizer.State;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WebOptimizationProject.Configuration;
using WebOptimizationProject.Helpers.Git;

namespace WebOptimizationProject
{
    internal class TestCode
    {
        public static void Testje()
        {
            //Directory.SetCurrentDirectory(@"C:\XGit\WebOptimizationProject\WebOptimizationProject\bin\Debug\netcoreapp1.1\ClonedRepos\sdfg-aspnetcore");

            //var config = ConfigHelper.GetConfig();
            //var git = new GitHandler(config);


            //File.WriteAllText("Testje.txt", $"{DateTime.Now}: This is just a test file. Don't accept my pull request, it's just for testing.");


            //var files = new List<OptimizedFileResult>() { new OptimizedFileResult("test.png", true, 150, 30, TimeSpan.FromSeconds(5.6), new List<string>()) };

            //var commitDesc = await TemplatesHandler.GetDescriptionForCommit();

            //await git.RunHubCommand("add .");
            //await git.RunHubCommand("status");
            //await git.Commit("Wop optimized this repository", commitDesc);
            //await git.RunHubCommand("push thefork");

            //var desc = await TemplatesHandler.GetDescriptionForPullRequest(files);

            //await git.PullRequest("The Web Optimization Project has optimized your repository!", desc);
        }

        //private static async Task<IEnumerable<OptimizableFile>> GoOptimizeStub(string dir, Config config)
        //{
        //    var testjeFile = Path.Combine(dir, "Testje.txt");
        //    File.WriteAllText(testjeFile, $"{DateTime.Now}: This is just a test file. Don't accept my pull request, it's just for testing.");

        //    var lijstje = new List<OptimizableFile>();

        //    lijstje.Add(new OptimizableFile(testjeFile, testjeFile, true, false, 1000000, 900000, TimeSpan.FromSeconds(9.5), new List<string>()));

        //    await Task.Delay(10);

        //    return lijstje;
        //}

        public static async Task Testje2()
        {
            var config = ConfigHelper.GetConfig();
            var gitOctoKitHandler = new GitOctoKitHandler(config);
            //Directory.SetCurrentDirectory(@"C:\XWOP\test-image-optimization");
            //var git = new GitHandler(config);

            //await git.PullRequest("HOI", "DOEI");


            var pr = await gitOctoKitHandler.GetPullRequest("desjoerd", "test-image-optimization");

            var pullRequestUpdate = new PullRequestUpdate()
            {
                Body = "hoihoi"
            };

            var res = await gitOctoKitHandler.GitHubClient.PullRequest.Update("desjoerd", "test-image-optimization", 6, pullRequestUpdate);

            //var pr = github.PullRequest;


            //var repo = await github.Repository.Get("antonfirsov", "Imagesharp.Tests.Images");

            //var fff = await github.Search.SearchIssues(new SearchIssuesRequest("state%3Aopen+author%3Adevedse+type%3Apr"));

            //var apiOptions = new ApiOptions()
            //{

            //};
            //var aaa = new PullRequestRequest() { State = ItemStateFilter.Open };

            //var prrr = await github.PullRequest.GetAllForRepository("antonfirsov", "Imagesharp.Tests.Images", aaa);
            //var user = await github.User.Get("devedse");
        }
    }
}
