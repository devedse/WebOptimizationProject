using DeveCoolLib.Conversion;
using DeveImageOptimizer;
using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WebOptimizationProject.Resources;

namespace WebOptimizationProject.Helpers
{
    public static class TemplatesHandler
    {
        public static string GetDescriptionForPullRequest()
        {
            var templateText = Templates.PullRequestMarkdownTemplate;


            //var stringForCommitDetails = await GetCommitDescriptionForPullRequest(optimizedFileResults, 1);
            //templateText = templateText + Environment.NewLine + Environment.NewLine + stringForCommitDetails;

            return templateText;
        }

        public static string GetCommitDescriptionForPullRequest(string clonedRepoPath, string branchName, IEnumerable<OptimizableFile> optimizedFileResults, int commitNumber)
        {
            var templateText = Templates.CommitInPullRequestMarkdownTemplate;

            templateText = templateText.Replace("{CommitNumber}", commitNumber.ToString());
            templateText = templateText.Replace("{SupportedFileExtensions}", string.Join(" ", ConstantsAndConfig.ValidExtensions));
            templateText = templateText.Replace("{Version}", Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString());

            var totalBytesBefore = optimizedFileResults.Sum(t => t.OriginalSize);
            var totalBytesSaved = optimizedFileResults.Where(t => t.OptimizationResult == OptimizationResult.Success).Sum(t => t.OriginalSize - t.OptimizedSize);
            var totalBytesAfter = totalBytesBefore - totalBytesSaved;
            var percentageRemaining = Math.Round(totalBytesAfter / (double)totalBytesBefore * 100.0, 2);

            var timeSpan = TimeSpan.Zero;
            foreach (var duration in optimizedFileResults.Select(t => t.Duration))
            {
                timeSpan += duration;
            }

            templateText = templateText.Replace("{OptimizableFileCount}", optimizedFileResults.Count().ToString());
            templateText = templateText.Replace("{FilesOptimizedSuccessfully}", optimizedFileResults.Count(t => t.OptimizationResult == OptimizationResult.Success).ToString());
            templateText = templateText.Replace("{FilesAlreadyOptimized}", optimizedFileResults.Count(t => t.OptimizationResult == OptimizationResult.Skipped).ToString());
            templateText = templateText.Replace("{FilesFailedOptimization}", optimizedFileResults.Count(t => t.OptimizationResult == OptimizationResult.Failed).ToString());
            templateText = templateText.Replace("{TotalBytesBefore}", ValuesToStringHelper.BytesToString(totalBytesBefore));
            templateText = templateText.Replace("{TotalBytesAfter}", ValuesToStringHelper.BytesToString(totalBytesAfter));
            templateText = templateText.Replace("{PercentageRemaining}", $"{percentageRemaining}%");
            templateText = templateText.Replace("{TotalBytesSaved}", ValuesToStringHelper.BytesToString(totalBytesSaved));
            templateText = templateText.Replace("{OptimizationDuration}", ValuesToStringHelper.SecondsToString((long)timeSpan.TotalSeconds));

            var optimizedFilesTable = new StringBuilder();

            optimizedFilesTable.AppendLine("FileName | Original Size | Optimized Size | Bytes Saved | Duration | Status");
            optimizedFilesTable.AppendLine("-- | -- | -- | -- | -- | --");
            var filesToPrint = optimizedFileResults.Where(t => t.OriginalSize > t.OptimizedSize || t.OptimizationResult == OptimizationResult.Failed).OrderByDescending(t => t.OriginalSize - t.OptimizedSize);
            foreach (var fileResult in filesToPrint)
            {
                //Reduce length of filename
                string fileName = Path.GetFileName(fileResult.Path);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                if (fileNameWithoutExtension.Length > 20)
                {
                    var fileNameWithoutExtensionShortened = fileNameWithoutExtension.Substring(0, Math.Min(fileNameWithoutExtension.Length, 20));
                    var extension = Path.GetExtension(fileName);
                    fileName = $"{fileNameWithoutExtensionShortened}..{extension}";
                }

                var relativeGitPath = RelativeGitPathHelper.GetRelativeGitPath(clonedRepoPath, fileResult.Path, branchName);
                if (relativeGitPath != null)
                {
                    fileName = $"[{fileName}]({relativeGitPath})";
                }

                var originalSize = ValuesToStringHelper.BytesToString(fileResult.OriginalSize);
                var optimizedSize = ValuesToStringHelper.BytesToString(fileResult.OptimizedSize);
                var bytesSaved = ValuesToStringHelper.BytesToString(fileResult.OriginalSize - fileResult.OptimizedSize);
                optimizedFilesTable.AppendLine($"{fileName} | {originalSize} | {optimizedSize} | {bytesSaved} | {ValuesToStringHelper.SecondsToString((long)fileResult.Duration.TotalSeconds)} | {fileResult.OptimizationResult}");
            }

            templateText = templateText.Replace("{OptimizedFiles}", optimizedFilesTable.ToString());

            return templateText;
        }

        public static string GetDescriptionForCommit()
        {
            var templateText = Templates.CommitMarkdownTemplate;

            return templateText;
        }
    }
}
