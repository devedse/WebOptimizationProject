using System;
using System.Collections.Generic;
using System.Text;

namespace WebOptimizationProject.Helpers
{
    public static class RelativeGitPathHelper
    {
        public static string GetRelativeGitPath(string clonedRepoPath, string fullFilePath, string branchName)
        {
            clonedRepoPath = clonedRepoPath.Replace('\\', '/');
            clonedRepoPath = clonedRepoPath.TrimEnd('/');

            fullFilePath = fullFilePath.Replace('\\', '/');

            if (!fullFilePath.ToUpperInvariant().StartsWith(clonedRepoPath.ToUpperInvariant()))
            {
                return null;
            }

            string relativePath = fullFilePath.Substring(clonedRepoPath.Length);
            string retval = $"../blob/{branchName}{relativePath}";
            return retval;
        }
    }
}
