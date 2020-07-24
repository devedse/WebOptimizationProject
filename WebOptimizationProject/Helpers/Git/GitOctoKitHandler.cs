using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOptimizationProject.Configuration;

namespace WebOptimizationProject.Helpers.Git
{
    public class GitOctoKitHandler
    {
        public GitHubClient GitHubClient { get; }
        private readonly WopConfig _config;

        public GitOctoKitHandler(WopConfig config)
        {
            _config = config;
            GitHubClient = CreateGitHubClient(config);
        }

        private GitHubClient CreateGitHubClient(WopConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.GitHubToken))
            {
                throw new InvalidOperationException("Github token is null or empty, make sure the token is configured in the Environment Variables or secrets.json");
            }
            //var credentials = new InMemoryCredentialStore(new Credentials(config.GitHubToken));
            var githubclient = new GitHubClient(new ProductHeaderValue(Constants.FeatureName));

            var cred = new Credentials(config.GitHubToken);
            githubclient.Credentials = cred;
            return githubclient;
        }

        public async Task<PullRequest> GetPullRequest(string repositoryOwner, string repositoryName)
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
                    Author = _config.GitHubUserName,
                    Type = IssueTypeQualifier.PullRequest,
                    State = ItemState.Open,
                    Head = Constants.FeatureName,
                    Repos = new RepositoryCollection() { $"{repositoryOwner}/{repositoryName}" }
                };

                bool shouldStop = false;

                //Retry 3 times
                for (int i = 0; i < 3; i++)
                {
                    var pullRequestsThisPage = await GitHubClient.Search.SearchIssues(searchIssuesRequest);
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
                var pr = await GitHubClient.PullRequest.Get(repositoryOwner, repositoryName, issue.Number);
                allPullRequests.Add(pr);
            }

            var thePullRequest = allPullRequests.Where(t => t.Head.Ref == Constants.FeatureName).ToList();
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
        }
    }
}
