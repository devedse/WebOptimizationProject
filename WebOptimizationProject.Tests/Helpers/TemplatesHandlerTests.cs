using DeveImageOptimizer.ImageOptimization;
using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using WebOptimizationProject.Helpers;
using Xunit;

namespace WebOptimizationProject.Tests.Helpers
{
    public class TemplatesHandlerTests
    {
        [Fact]
        public void DoesNotCreateTemplateContentHigherThenMaxLength()
        {
            //Arrange
            var fileList = new List<OptimizableFile>();

            for (int i = 0; i < 10000; i++)
            {
                fileList.Add(new OptimizableFile("test2.png", "test2.png", 9000));
            }

            foreach (var file in fileList)
            {
                file.SetSuccess(50, TimeSpan.FromSeconds(5), ImageOptimizationLevel.Maximum);
            }

            //Act
            var result = TemplatesHandler.GetCommitDescriptionForPullRequest("coolPath", "coolBranch", fileList, "date");

            //Assert
            Assert.True(result.Length < Constants.MaxLengthPullRequestDescriptionAndComment);
        }
    }
}
