using DeveImageOptimizer;
using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using Octokit;
using Octokit.Internal;
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
        private const string FeatureName = "WebOptimizationProject";

        static void Main(string[] args)
        {
            Console.WriteLine("For this tool to work you need to have both GIT and HUB installed.");

            //Testje2().Wait();

            //Gogo("devedse", "ImageTest").Wait();
            //Gogo("desjoerd", "sdfg-aspnetcore").Wait();
            //Gogo("desjoerd", "test-image-optimization").Wait();
            //Gogo("facebook", "react").GetAwaiter().GetResult();
            //Gogo("shoheiyokoyama", "Assets").Wait();
            //Gogo("antonfirsov", "Imagesharp.Tests.Images").Wait();
            Gogo("docker", "kitematic").Wait();

            Console.WriteLine("Application finished, press any key to continue...");
            Console.ReadKey();

            //Testje().Wait();

        }

        private static GitHubClient CreateGitHubClient(Config config)
        {
            if (string.IsNullOrWhiteSpace(config.GitHubToken))
            {
                throw new InvalidOperationException("Github token is null or empty, please modify config.json");
            }
            //var credentials = new InMemoryCredentialStore(new Credentials(config.GitHubToken));
            var githubclient = new GitHubClient(new ProductHeaderValue(FeatureName));

            var cred = new Credentials(config.GitHubToken);
            githubclient.Credentials = cred;
            return githubclient;
        }

        public static async Task Testje2()
        {
            var config = ConfigHelper.GetConfig();

            //Directory.SetCurrentDirectory(@"C:\XWOP\test-image-optimization");
            //var git = new GitHandler(config);

            //await git.PullRequest("HOI", "DOEI");


            var github = CreateGitHubClient(config);
            var pr = await GetPullRequest(github, "desjoerd", "test-image-optimization", config);

            var pullRequestUpdate = new PullRequestUpdate()
            {
                Body = "hoihoi"
            };

            var res = await github.PullRequest.Update("desjoerd", "test-image-optimization", 6, pullRequestUpdate);

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

        public static async Task<PullRequest> GetPullRequest(GitHubClient github, string repositoryOwner, string repositoryName, Config config)
        {
            Console.WriteLine($"Getting pullrequest with RepoOwner: {repositoryOwner} RepoName: {repositoryName}");

            //var allPullRequests = await github.Search.SearchIssues(new SearchIssuesRequest($"state%3Aopen+author%3A{config.GithubUserName}+type%3Apr"));

            var totalIssueList = new List<Issue>();

            int page = 1;

            while (true)
            {
                var searchIssuesRequest = new SearchIssuesRequest()
                {
                    Page = page,
                    PerPage = 100,
                    Author = config.GithubUserName,
                    Type = IssueTypeQualifier.PullRequest,
                    State = ItemState.Open,
                    Head = FeatureName,
                    Repos = new RepositoryCollection() { $"{repositoryOwner}/{repositoryName}" }
                };

                bool shouldStop = false;

                //Retry 3 times
                for (int i = 0; i < 3; i++)
                {
                    var pullRequestsThisPage = await github.Search.SearchIssues(searchIssuesRequest);
                    if (pullRequestsThisPage.IncompleteResults == false)
                    {
                        totalIssueList.AddRange(pullRequestsThisPage.Items);
                        if (pullRequestsThisPage.Items.Count < searchIssuesRequest.PerPage)
                        {
                            shouldStop = true;
                        }
                        break;
                    }
                    else
                    {
                        if (i == 2)
                        {
                            Console.WriteLine("Request to git didn't complete fully, Pull Request might have not been found");
                            totalIssueList.AddRange(pullRequestsThisPage.Items);
                        }
                    }
                }

                if (shouldStop)
                {
                    break;
                }
            }

            var allPullRequests = new List<PullRequest>();

            foreach (var issue in totalIssueList)
            {
                var pr = await github.PullRequest.Get(repositoryOwner, repositoryName, issue.Number);
                allPullRequests.Add(pr);
            }

            var thePullRequest = allPullRequests.Where(t => t.Head.Ref == FeatureName).ToList();
            if (thePullRequest.Count == 0)
            {
                Console.WriteLine("Couldn't find the created pull request");
                return null;
            }
            else if (thePullRequest.Count > 1)
            {
                throw new InvalidOperationException($"There are multiple pull requests on the same branch with the same name. This is impossible: {string.Join(Environment.NewLine, thePullRequest.Select(t => t.HtmlUrl))}");
            }
            else
            {
                return thePullRequest.Single();
            }

            //if (allPullRequests.TotalCount > 1)
            //{
            //    throw new InvalidOperationException($"Found more than one pull request, this is strange: {string.Join(Environment.NewLine, allPullRequests.Items.Select(t => t.HtmlUrl))}");
            //}
            //if (allPullRequests.TotalCount == 0)
            //{
            //    return null;
            //}

            //var thisIssue = allPullRequests.Items.Single();
            //var foundPullRequest = await github.PullRequest.Get(repositoryOwner, repositoryName, thisIssue.Number);
            //return foundPullRequest;
            //var prForThisRepo = allPullRequests.i
        }

        public static async Task Testje()
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

        private static async Task<IEnumerable<OptimizedFileResult>> GoOptimizeStub(string dir, Config config)
        {
            var testjeFile = Path.Combine(dir, "Testje.txt");
            File.WriteAllText(testjeFile, $"{DateTime.Now}: This is just a test file. Don't accept my pull request, it's just for testing.");

            var lijstje = new List<OptimizedFileResult>();

            lijstje.Add(new OptimizedFileResult(testjeFile, true, 1000000, 900000, TimeSpan.FromSeconds(9.5), new List<string>()));

            await Task.Delay(10);

            return lijstje;
        }

        public static async Task Gogo(string repositoryOwner, string repositoryName, string branchName = null)
        {
            var config = ConfigHelper.GetConfig();
            var githubClient = CreateGitHubClient(config);

            string dirOfClonedRepos = config.ClonedRepositoriesDirectoryName;
            if (!Path.IsPathRooted(dirOfClonedRepos))
            {
                dirOfClonedRepos = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, config.ClonedRepositoriesDirectoryName);
            }

            Directory.CreateDirectory(dirOfClonedRepos);
            Directory.SetCurrentDirectory(dirOfClonedRepos);
            var git = new GitHandler(config);

            //var clonedRepo = Path.Combine(Directory.GetCurrentDirectory(), repositoryName);

            var clonedRepo = await git.GitClone(dirOfClonedRepos, repositoryOwner, repositoryName);
            Directory.SetCurrentDirectory(clonedRepo);



            if (branchName == null)
            {
                branchName = await git.GetHeadBranch();
                if (branchName == null)
                {
                    throw new Exception("ERROR, couldn't determine branchname");
                }
            }

            await git.RunHubCommand("fork");

            await git.RunHubCommand($"remote set-url {config.GithubUserName} https://github.com/{config.GithubUserName}/{repositoryName}.git");
            //await git.RunHubCommand($"remote add thefork https://github.com/{config.GithubUserName}/{repositoryName}.git");

            //Fetch everything in my repository
            await git.RunHubCommand("fetch --all");

            //Go to master
            await git.RunHubCommand($"checkout {config.GithubUserName}/{branchName}");
            await git.RunHubCommand($"merge --strategy-option=theirs origin/{branchName}");
            await git.RunHubCommand($"push {config.GithubUserName} HEAD:{branchName}");

            //var createdBranch = await git.RunHubCommand($"branch {featureName}");

            var wasAbleToAddTrackedBranch = await git.RunHubCommand($"checkout --track -b {FeatureName} {config.GithubUserName}/{FeatureName}");

            if (wasAbleToAddTrackedBranch == 0)
            {
                //await git.RunHubCommand($"checkout {config.GithubUserName}/WebOptimizationProject");
                await git.RunHubCommand($"merge --strategy-option=theirs {config.GithubUserName}/{branchName}");
                await git.RunHubCommand($"push {config.GithubUserName} {FeatureName} -u");
            }
            else
            {
                var createdNewBranch = await git.RunHubCommand($"checkout -b {FeatureName}");
                if (createdNewBranch == 0)
                {
                }
                else
                {
                    await git.RunHubCommand($"checkout {FeatureName}");
                    await git.RunHubCommand($"merge --strategy-option=theirs {config.GithubUserName}/{branchName}");
                }
                await git.RunHubCommand($"push {config.GithubUserName} {FeatureName} -u");
            }

            var optimizedFileResults = await GoOptimize(clonedRepo, config);
            //var optimizedFileResults = await GoOptimizeStub(clonedRepo, config);

            await git.RunHubCommand("add .");

            var descriptionForCommit = await TemplatesHandler.GetDescriptionForCommit();
            await git.Commit("Wop optimized this repository", descriptionForCommit);
            await git.RunHubCommand($"push");

            var descriptionForPullRequest = await TemplatesHandler.GetDescriptionForPullRequest();

            //Only create pull request if there were actually any successful optimizations
            if (optimizedFileResults.Any(t => t.Successful) && optimizedFileResults.Sum(t => t.OriginalSize) > optimizedFileResults.Sum(t => t.OptimizedSize))
            {
                var pullRequestState = await git.PullRequest("The Web Optimization Project has optimized your repository!", descriptionForPullRequest);
                Console.WriteLine("Pullrequeststate: " + pullRequestState);

                PullRequest obtainedPullRequest = null;
                for (int i = 0; i < 3; i++)
                {
                    obtainedPullRequest = await GetPullRequest(githubClient, repositoryOwner, repositoryName, config);
                    if (obtainedPullRequest != null)
                    {
                        break;
                    }
                    Console.WriteLine("Couldn't find Pull Request. Waiting and retrying...");
                    await Task.Delay(10000);
                }

                Console.WriteLine($"Found pull request: {obtainedPullRequest.HtmlUrl}");

                var commitIdentifier = "### Commit ";
                var splittedBody = obtainedPullRequest.Body.Split('\n');
                var commitCount = splittedBody.Count(t => t.StartsWith(commitIdentifier));

                var descriptionForCommitInPr = await TemplatesHandler.GetCommitDescriptionForPullRequest(clonedRepo, branchName, optimizedFileResults, commitCount + 1);

                var bodySb = new StringBuilder(obtainedPullRequest.Body);
                bodySb.AppendLine();
                bodySb.AppendLine();
                bodySb.AppendLine(descriptionForCommitInPr);

                var pullRequestUpdate = new PullRequestUpdate()
                {
                    Body = bodySb.ToString()
                };

                await githubClient.PullRequest.Update(repositoryOwner, repositoryName, obtainedPullRequest.Number, pullRequestUpdate);
            }
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