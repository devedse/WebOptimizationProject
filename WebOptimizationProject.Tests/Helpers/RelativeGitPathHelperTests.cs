using System;
using WebOptimizationProject.Helpers;
using Xunit;

namespace WebOptimizationProject.Tests.Helpers
{
    public class RelativeGitPathHelperTests
    {
        [Fact]
        public void ReturnsTheRightRelativeGitPath()
        {
            var clonedRepoPath = @"C:\XWOP\kitematic\";
            var fullFilePath = @"C:\XWOP\kitematic\images\inspection@2x.png";
            var branchName = "master";
            var gitPath = RelativeGitPathHelper.GetRelativeGitPath(clonedRepoPath, fullFilePath, branchName);

            var expected = @"../blob/master/images/inspection@2x.png";
            Assert.Equal(expected, gitPath);
        }

        [Fact]
        public void ReturnsTheRightRelativeGitPathWhenClonedRepoHasNoSlash()
        {
            var clonedRepoPath = @"C:\XWOP\kitematic";
            var fullFilePath = @"C:\XWOP\kitematic\images\inspection@2x.png";
            var branchName = "master";
            var gitPath = RelativeGitPathHelper.GetRelativeGitPath(clonedRepoPath, fullFilePath, branchName);

            var expected = @"../blob/master/images/inspection@2x.png";
            Assert.Equal(expected, gitPath);
        }

        [Fact]
        public void ReturnsTheRightRelativeGitPathWhenClonedRepoHasDifferentCasing()
        {
            var clonedRepoPath = @"C:\xwop\KITEMATIC";
            var fullFilePath = @"C:\XWOP\kitematic\images\inspection@2x.png";
            var branchName = "master";
            var gitPath = RelativeGitPathHelper.GetRelativeGitPath(clonedRepoPath, fullFilePath, branchName);

            var expected = @"../blob/master/images/inspection@2x.png";
            Assert.Equal(expected, gitPath);
        }

        [Fact]
        public void ReturnsNullWhenStartDoesntMatch()
        {
            var clonedRepoPath = @"C:\XWOP\kitematic";
            var fullFilePath = @"C:\XSOMETHINGDIFFERENT\kitematic\images\inspection@2x.png";
            var branchName = "master";
            var gitPath = RelativeGitPathHelper.GetRelativeGitPath(clonedRepoPath, fullFilePath, branchName);

            Assert.Null(gitPath);
        }
    }
}
