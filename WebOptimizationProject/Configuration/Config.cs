using System;
using System.Collections.Generic;
using System.Text;

namespace WebOptimizationProject.Configuration
{
    public class Config
    {
        public string ClonedRepositoriesDirectoryName { get; set; } = @"C:\XWOP";

        public string GitHubToken { get; set; } = "";

        private string _githubusername = "Devedse";
        public string GithubUserName
        {
            get
            {
                return _githubusername.ToLowerInvariant();
            }
            set
            {
                _githubusername = value;
            }
        }
    }
}
