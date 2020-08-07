using AdysTech.CredentialManager;
using DeveCoolLib.ProcessAsTask;
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
        private readonly WopConfig _config;

        public GitCommandLineHandler(WopConfig config)
        {
            this._config = config;

            Console.WriteLine($"Configuring git credentials in CredentialManager...");
            var cred = new System.Net.NetworkCredential("PersonalAccessToken", config.GitHubToken);
            var icred = cred.ToICredential();

            icred.TargetName = "git:https://github.com";
            icred.Persistance = Persistance.LocalMachine;

            var result = icred.SaveCredential();
            Console.WriteLine($"Credential configuration result: {result}");
        }

        public async Task<string> GitClone(string repositoriesDir, string userName, string repositoryName)
        {
            Directory.SetCurrentDirectory(repositoriesDir);
            var cloneingDir = Path.Combine(repositoriesDir, repositoryName);
            //await DirectoryHelper.DeleteDirectory(cloneingDir);

            var totalUrl = $"https://github.com/{userName}/{repositoryName}.git";

            await RunHubCommand($"clone {totalUrl}");

            return cloneingDir;
        }

        public async Task<ProcessResults> Commit(string message, string description)
        {
            var stringForCommit = $"commit -m \"{message}{Environment.NewLine}{Environment.NewLine}{description}\"";

            stringForCommit = stringForCommit.Replace("\r", "");

            return await RunHubCommand(stringForCommit);
        }

        public async Task<ProcessResults> PullRequest(string message, string description)
        {
            var stringForPullRequest = $"pull-request -m \"{message}{Environment.NewLine}{Environment.NewLine}{description}\"";

            stringForPullRequest = stringForPullRequest.Replace("\r", "");

            return await RunHubCommand(stringForPullRequest);
        }

        //public async Task<int> RunGitCommand(string command)
        //{
        //    return await WopProcessRunner.RunProcessAsync("git", command);
        //}

        public async Task<ProcessResults> RunHubCommand(string command)
        {
            var envs = new Dictionary<string, string>()
            {
                {"GITHUB_TOKEN", _config.GitHubToken },
                {"GITHUB_USER", _config.GitHubUserName }
            };
            return await ProcessRunner.RunAsyncAndLogToConsole("hub", command, envs);
        }

        public async Task<string> GetHeadBranch()
        {
            var result = await ProcessRunner.RunAsyncAndLogToConsole("git", "symbolic-ref refs/remotes/origin/HEAD");
            if (result.ExitCode != 0)
            {
                Console.WriteLine("Couldn't determine head branch, command failed.");
                return null;
            }
            var theName = result.StandardOutput.FirstOrDefault(t => t.ToLowerInvariant().Contains("origin"));
            if (theName == null || string.IsNullOrWhiteSpace(theName) || theName.Count(t => t == '/') == 0)
            {
                Console.WriteLine($"Couldn't determine head branch, output isn't as expected: {theName}");
                return null;
            }

            var lio = theName.LastIndexOf('/');
            var headBranchName = theName.Substring(lio + 1);
            return headBranchName;
        }
    }
}
