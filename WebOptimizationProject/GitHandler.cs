using DeveImageOptimizer.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WebOptimizationProject.Configuration;
using WebOptimizationProject.Helpers;

namespace WebOptimizationProject
{
    public class GitHandler
    {
        private readonly Config config;

        public GitHandler(Config config)
        {
            this.config = config;
        }

        public async Task<string> GitClone(string repositoriesDir, string userName, string repositoryName)
        {
            Directory.SetCurrentDirectory(repositoriesDir);
            var cloneingDir = Path.Combine(repositoriesDir, repositoryName);
            //await DirectoryHelper.DeleteDirectory(cloneingDir);

            var totalUrl = $"https://github.com/{userName}/{repositoryName}.git";

            await RunGitCommand($"clone {totalUrl}");

            return cloneingDir;
        }

        public async Task PullRequest(string message, string description)
        {
            var stringForPullRequest = $"pull-request -m \"{message}{Environment.NewLine}{Environment.NewLine}{description}\"";

            stringForPullRequest = stringForPullRequest.Replace("\r", "");

            await RunHubCommand(stringForPullRequest);
        }

        public async Task RunGitCommand(string command)
        {
            var psi = new ProcessStartInfo("git", command);

            await WopProcessRunner.RunProcessAsync(psi);
        }

        public async Task RunHubCommand(string command)
        {
            var psi = new ProcessStartInfo("hub", command);

            await WopProcessRunner.RunProcessAsync(psi);
        }
    }
}
