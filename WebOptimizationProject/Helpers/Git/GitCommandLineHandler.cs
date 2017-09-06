using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebOptimizationProject.Configuration;

namespace WebOptimizationProject.Helpers.Git
{
    public class GitCommandLineHandler
    {
        private readonly Config config;

        public GitCommandLineHandler(Config config)
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

        public async Task<int> Commit(string message, string description)
        {
            var stringForCommit = $"commit -m \"{message}{Environment.NewLine}{Environment.NewLine}{description}\"";

            stringForCommit = stringForCommit.Replace("\r", "");

            return await RunHubCommand(stringForCommit);
        }

        public async Task<int> PullRequest(string message, string description)
        {
            var stringForPullRequest = $"pull-request -m \"{message}{Environment.NewLine}{Environment.NewLine}{description}\"";

            stringForPullRequest = stringForPullRequest.Replace("\r", "");

            return await RunHubCommand(stringForPullRequest);
        }

        public async Task<int> RunGitCommand(string command)
        {
            return await WopProcessRunner.RunProcessAsync("git", command);
        }

        public async Task<int> RunHubCommand(string command)
        {
            return await WopProcessRunner.RunProcessAsync("hub", command);
        }

        public async Task<string> GetHeadBranch()
        {
            var outList = new List<ProcessOutputLine>();
            var result = await WopProcessRunner.RunProcessAsyncWithResults("git", "symbolic-ref refs/remotes/origin/HEAD", outList);
            if (result != 0)
            {
                Console.WriteLine("Couldn't determine head branch, command failed.");
                return null;
            }
            var theName = outList.FirstOrDefault(t => t.Type == ProcessOutputLineType.Log && t.Txt.ToLowerInvariant().Contains("origin"));
            if (theName == null || string.IsNullOrWhiteSpace(theName.Txt) || theName.Txt.Count(t => t == '/') == 0)
            {
                Console.WriteLine($"Couldn't determine head branch, output isn't as expected: {theName}");
                return null;
            }

            var lio = theName.Txt.LastIndexOf('/');
            var headBranchName = theName.Txt.Substring(lio + 1);
            return headBranchName;
        }
    }
}
