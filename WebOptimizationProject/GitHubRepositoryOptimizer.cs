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

namespace WebOptimizationProject
{
    public class GitHubRepositoryOptimizer
    {
        private readonly WopConfig _wopConfig;
        private readonly DeveImageOptimizerConfiguration _deveImageOptimizerConfiguration;
        private readonly IProgressReporter _progressReporter;
        private readonly GitOctoKitHandler _gitOctoKitHandler;
        private readonly GitCommandLineHandler _git;

        public GitHubRepositoryOptimizer(WopConfig wopConfig, DeveImageOptimizerConfiguration deveImageOptimizerConfiguration, IProgressReporter progressReporter)
        {
            _wopConfig = wopConfig;
            _deveImageOptimizerConfiguration = deveImageOptimizerConfiguration;
            _progressReporter = progressReporter;
            _gitOctoKitHandler = new GitOctoKitHandler(_wopConfig);
            _git = new GitCommandLineHandler(_wopConfig);
        }

        public async Task<IEnumerable<string>> ObtainRepositoriesForOwner(string repositoryOwner)
        {
            var pubrepos = await _gitOctoKitHandler.GitHubClient.Repository.GetAllForUser(repositoryOwner);
            var orderedPubRepos = pubrepos.OrderByDescending(t => t.StargazersCount);

            var orderedPubReposNames = orderedPubRepos.Select(t => t.Name);
            return orderedPubReposNames;
        }

        public async Task GoOptimize(bool cleanupAfterwards, long repositoryId, string branchName = null)
        {
            var repositoryInfo = await _gitOctoKitHandler.GitHubClient.Repository.Get(repositoryId);
            await GoOptimize(cleanupAfterwards, repositoryInfo, branchName);
        }

        public async Task GoOptimize(bool cleanupAfterwards, string repositoryOwner, string repositoryName, string branchName = null)
        {
            var repositoryInfo = await _gitOctoKitHandler.GitHubClient.Repository.Get(repositoryOwner, repositoryName);
            await GoOptimize(cleanupAfterwards, repositoryInfo, branchName);
        }

        public async Task GoOptimize(bool cleanupAfterwards, Repository repository, string branchName = null)
        {
            var repositoryOwner = repository.Owner.Login;
            var repositoryName = repository.Name;

            var myUser = await _gitOctoKitHandler.GitHubClient.User.Current();
            await _git.ConfigureGitCredentials();

            Console.WriteLine($"{repositoryOwner}/{repositoryName} is being optimized...");
            Console.WriteLine();

            string dirOfClonedRepos = _wopConfig.ClonedRepositoriesDirectoryName;
            if (!Path.IsPathRooted(dirOfClonedRepos))
            {
                dirOfClonedRepos = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, _wopConfig.ClonedRepositoriesDirectoryName);
            }

            Directory.CreateDirectory(dirOfClonedRepos);
            Directory.SetCurrentDirectory(dirOfClonedRepos);

            var clonedRepo = await _git.GitClone(dirOfClonedRepos, repositoryOwner, repositoryName);
            Directory.SetCurrentDirectory(clonedRepo);

            var repositoryInfo = await _gitOctoKitHandler.GitHubClient.Repository.Get(repositoryOwner, repositoryName);

            if (branchName == null)
            {
                branchName = repositoryInfo.DefaultBranch;
                if (branchName == null)
                {
                    throw new Exception("ERROR, couldn't determine branchname");
                }
            }

            await _git.RunHubCommand("fork");

            await _git.RunHubCommand($"remote set-url {_wopConfig.GitHubUserName} https://github.com/{_wopConfig.GitHubUserName}/{repositoryName}.git");

            //Fetch everything in my repository
            await _git.RunHubCommand("fetch --all");

            //Go to master
            await _git.RunHubCommand($"checkout {_wopConfig.GitHubUserName}/{branchName}");
            await _git.RunHubCommand($"merge --strategy-option=theirs origin/{branchName}");
            await _git.RunHubCommand($"push {_wopConfig.GitHubUserName} HEAD:{branchName}");

            var wasAbleToAddTrackedBranch = await _git.RunHubCommand($"checkout --track -b {Constants.FeatureName} {_wopConfig.GitHubUserName}/{Constants.FeatureName}");

            if (wasAbleToAddTrackedBranch.ExitCode == 0)
            {
                await _git.RunHubCommand($"merge --strategy-option=theirs {_wopConfig.GitHubUserName}/{branchName}");
                await _git.RunHubCommand($"push {_wopConfig.GitHubUserName} {Constants.FeatureName} -u");
            }
            else
            {
                var createdNewBranch = await _git.RunHubCommand($"checkout -b {Constants.FeatureName}");
                if (createdNewBranch.ExitCode == 0)
                {
                }
                else
                {
                    await _git.RunHubCommand($"checkout {Constants.FeatureName}");
                    await _git.RunHubCommand($"merge --strategy-option=theirs {_wopConfig.GitHubUserName}/{branchName}");
                }
                await _git.RunHubCommand($"push {_wopConfig.GitHubUserName} {Constants.FeatureName} -u");
            }

            var optimizedFileResults = await GoOptimize(clonedRepo, _wopConfig);
            //var optimizedFileResults = await GoOptimizeStub(clonedRepo, config);

            await _git.RunHubCommand("add .");

            var descriptionForCommit = TemplatesHandler.GetDescriptionForCommit();
            await _git.Commit("Wop optimized this repository", descriptionForCommit);
            await _git.RunHubCommand($"push");

            var descriptionForPullRequest = TemplatesHandler.GetDescriptionForPullRequest();

            //Only create pull request if there were actually any successful optimizations
            if (optimizedFileResults.Any(t => t.OptimizationResult == OptimizationResult.Success && t.OriginalSize > t.OptimizedSize))
            {
                PullRequest obtainedPullRequest = await _gitOctoKitHandler.GetPullRequest(repositoryOwner, repositoryName);

                if (obtainedPullRequest == null)
                {
                    var pr = new NewPullRequest("The Web Optimization Project has optimized your repository!", $"{_wopConfig.GitHubUserName}:{Constants.FeatureName}", branchName)
                    {
                        Body = descriptionForPullRequest
                    };
                    obtainedPullRequest = await _gitOctoKitHandler.GitHubClient.PullRequest.Create(repositoryOwner, repositoryName, pr);
                }
                Console.WriteLine($"Using PR: {obtainedPullRequest.Url}");

                var descriptionForCommitInPr = TemplatesHandler.GetCommitDescriptionForPullRequest(clonedRepo, branchName, optimizedFileResults, DateTime.UtcNow.ToString());
                Console.WriteLine($"Creating comment on pr with length {descriptionForCommitInPr.Length}...");
                var createdComment = await _gitOctoKitHandler.GitHubClient.Issue.Comment.Create(repositoryOwner, repositoryName, obtainedPullRequest.Number, descriptionForCommitInPr);
                Console.WriteLine($"Comment created: {createdComment.Url}");
            }

            Console.WriteLine();
            Console.WriteLine($"{repositoryOwner}/{repositoryName} is optimized :)");
            Console.WriteLine();

            if (cleanupAfterwards)
            {
                Console.WriteLine($"Cleaning up local files '{clonedRepo}'...");
                Directory.SetCurrentDirectory(dirOfClonedRepos);
                CleanupRecursively(clonedRepo);
                //Directory.Delete(clonedRepo, true);
                Console.WriteLine($"Directory {clonedRepo} removed.");
            }
        }

        private async Task<IEnumerable<OptimizableFile>> GoOptimize(string dir, WopConfig config)
        {
            var fileRememberer = new FileProcessedStateRememberer(false);
            var dirRememberer = new DirProcessedStateRememberer(true);

            var fileProcessor = new DeveImageOptimizerProcessor(_deveImageOptimizerConfiguration, _progressReporter, fileRememberer, dirRememberer);
            var optimizedFileResults = await fileProcessor.ProcessDirectory(dir);

            return optimizedFileResults;
        }

        private static void CleanupRecursively(string path)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.IsReadOnly)
                {
                    fileInfo.IsReadOnly = false;
                    Console.WriteLine($"Removing readonly flag for: {file}");
                }

                Console.WriteLine($"Deleting file: {file}");
                File.Delete(file);
            }

            foreach (var dir in Directory.GetDirectories(path))
            {
                CleanupRecursively(dir);
            }

            Console.WriteLine($"Deleting dir: {path}");
            Directory.Delete(path);
        }
    }
}