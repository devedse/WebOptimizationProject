using System;
using System.Collections.Generic;
using System.Text;

namespace WebOptimizationProject.Configuration
{
    public class Config
    {
        public string FileOptimizerFullExePath { get; set; } = @"C:\Program Files\FileOptimizer\FileOptimizer64.exe";
        public string ClonedRepositoriesDirectoryName { get; set; } = @"C:\XWOP";

        public string GitHubToken { get; set; } = "";

        private string githubusername = "Devedse";
        public string GithubUserName
        {
            get
            {
                return githubusername.ToLowerInvariant();
            }
            set
            {
                githubusername = value;
            }
        }
    }
}
