namespace WebOptimizationProject.Configuration
{
    public class WopConfig
    {
        public string ClonedRepositoriesDirectoryName { get; set; } = @"C:\XWOP";

        private string _githubusername;
        public string GitHubUserName
        {
            get => _githubusername;
            set => _githubusername = value.ToLowerInvariant();
        }

        public string GitHubEmail { get; set; }

        public string GitHubToken { get; set; }
    }
}
