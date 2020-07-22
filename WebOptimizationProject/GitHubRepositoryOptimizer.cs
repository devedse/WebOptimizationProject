using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using DeveImageOptimizer.State.StoringProcessedDirectories;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebOptimizationProject.Configuration;
using WebOptimizationProject.Helpers;
using WebOptimizationProject.Helpers.Git;
using WebOptimizationProject.ImageOptimization;

namespace WebOptimizationProject
{
    public class GitHubRepositoryOptimizer
    {
        public static async Task<IEnumerable<string>> ObtainRepositoriesForOwner(string repositoryOwner)
        {
            var config = ConfigHelper.GetConfig();
            var gitOctoKitHandler = new GitOctoKitHandler(config);

            var pubrepos = await gitOctoKitHandler.GitHubClient.Repository.GetAllForUser(repositoryOwner);
            var orderedPubRepos = pubrepos.OrderByDescending(t => t.StargazersCount);

            var orderedPubReposNames = orderedPubRepos.Select(t => t.Name);
            return orderedPubReposNames;
        }

        public static async Task GoOptimize(string repositoryOwner, string repositoryName, string branchName = null)
        {
            Console.WriteLine($"{repositoryOwner}/{repositoryName} is being optimized...");
            Console.WriteLine();

            var config = ConfigHelper.GetConfig();
            var gitOctoKitHandler = new GitOctoKitHandler(config);

            string dirOfClonedRepos = config.ClonedRepositoriesDirectoryName;
            if (!Path.IsPathRooted(dirOfClonedRepos))
            {
                dirOfClonedRepos = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, config.ClonedRepositoriesDirectoryName);
            }

            Directory.CreateDirectory(dirOfClonedRepos);
            Directory.SetCurrentDirectory(dirOfClonedRepos);
            var git = new GitCommandLineHandler(config);

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

            //Fetch everything in my repository
            await git.RunHubCommand("fetch --all");

            //Go to master
            await git.RunHubCommand($"checkout {config.GithubUserName}/{branchName}");
            await git.RunHubCommand($"merge --strategy-option=theirs origin/{branchName}");
            await git.RunHubCommand($"push {config.GithubUserName} HEAD:{branchName}");

            var wasAbleToAddTrackedBranch = await git.RunHubCommand($"checkout --track -b {Constants.FeatureName} {config.GithubUserName}/{Constants.FeatureName}");

            if (wasAbleToAddTrackedBranch == 0)
            {
                await git.RunHubCommand($"merge --strategy-option=theirs {config.GithubUserName}/{branchName}");
                await git.RunHubCommand($"push {config.GithubUserName} {Constants.FeatureName} -u");
            }
            else
            {
                var createdNewBranch = await git.RunHubCommand($"checkout -b {Constants.FeatureName}");
                if (createdNewBranch == 0)
                {
                }
                else
                {
                    await git.RunHubCommand($"checkout {Constants.FeatureName}");
                    await git.RunHubCommand($"merge --strategy-option=theirs {config.GithubUserName}/{branchName}");
                }
                await git.RunHubCommand($"push {config.GithubUserName} {Constants.FeatureName} -u");
            }

            var optimizedFileResults = await GoOptimize(clonedRepo, config);
            //var optimizedFileResults = await GoOptimizeStub(clonedRepo, config);

            await git.RunHubCommand("add .");

            var descriptionForCommit = await TemplatesHandler.GetDescriptionForCommit();
            await git.Commit("Wop optimized this repository", descriptionForCommit);
            await git.RunHubCommand($"push");

            var descriptionForPullRequest = await TemplatesHandler.GetDescriptionForPullRequest();

            //Only create pull request if there were actually any successful optimizations
            if (optimizedFileResults.Any(t => t.OptimizationResult == OptimizationResult.Success) && optimizedFileResults.Sum(t => t.OriginalSize) > optimizedFileResults.Sum(t => t.OptimizedSize))
            {
                var pullRequestState = await git.PullRequest("The Web Optimization Project has optimized your repository!", descriptionForPullRequest);
                Console.WriteLine("Pullrequeststate: " + pullRequestState);

                PullRequest obtainedPullRequest = null;
                for (int i = 0; i < 3; i++)
                {
                    obtainedPullRequest = await gitOctoKitHandler.GetPullRequest(repositoryOwner, repositoryName);
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

                await gitOctoKitHandler.GitHubClient.PullRequest.Update(repositoryOwner, repositoryName, obtainedPullRequest.Number, pullRequestUpdate);
            }

            Console.WriteLine();
            Console.WriteLine($"{repositoryOwner}/{repositoryName} is optimized :)");
            Console.WriteLine();
        }

        private static async Task<IEnumerable<OptimizableFile>> GoOptimize(string dir, Config config)
        {
            var c = new DeveImageOptimizerConfiguration()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            var wopProgressReporter = new WopProgressReporter();
            var fileRememberer = new FileProcessedStateRememberer(false);
            var dirRememberer = new DirProcessedStateRememberer(false);

            var fileProcessor = new DeveImageOptimizerProcessor(c, wopProgressReporter, fileRememberer, dirRememberer);
            var optimizedFileResults = await fileProcessor.ProcessDirectory(dir);

            return optimizedFileResults;
        }
    }
}