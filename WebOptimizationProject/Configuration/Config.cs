using System;
using System.Collections.Generic;
using System.Text;

namespace WebOptimizationProject.Configuration
{
    public class Config
    {
        public string FileOptimizerFullExePath { get; set; } = @"C:\Program Files\FileOptimizer\FileOptimizer64.exe";
        public string ClonedRepositoriesDirectoryName { get; set; } = "ClonedRepos";
        public string GithubUserName { get; set; } = "Devedse";
    }
}
