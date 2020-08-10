using Octokit;

namespace WebOptimizationProject.Helpers.Git
{
    public class IssueAndRepo
    {
        public string RepoOwner { get; set; }
        public string RepoName { get; set; }
        public Issue Issue { get; set; }
    }
}
