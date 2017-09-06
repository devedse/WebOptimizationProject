using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebOptimizationProject.Helpers
{
    public static class TemplatesHandler
    {
        public static async Task<string> GetDescriptionForPullRequest()
        {
            var filePath = Path.Combine(FolderHelperMethods.EntryAssemblyDirectory.Value, "PullRequestMarkdownTemplate.txt");
            var templateText = await Task.Run(() => File.ReadAllText(filePath));


            //var stringForCommitDetails = await GetCommitDescriptionForPullRequest(optimizedFileResults, 1);
            //templateText = templateText + Environment.NewLine + Environment.NewLine + stringForCommitDetails;

            return templateText;
        }

        public static async Task<string> GetCommitDescriptionForPullRequest(string clonedRepoPath, string branchName, IEnumerable<OptimizedFileResult> optimizedFileResults, int commitNumber)
        {
            var filePath = Path.Combine(FolderHelperMethods.EntryAssemblyDirectory.Value, "CommitInPullRequestMarkdownTemplate.txt");
            var templateText = await Task.Run(() => File.ReadAllText(filePath));

            templateText = templateText.Replace("{CommitNumber}", commitNumber.ToString());
            templateText = templateText.Replace("{SupportedFileExtensions}", string.Join(" ", DeveImageOptimizer.Constants.ValidExtensions));
            templateText = templateText.Replace("{Version}", Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString());

            var totalBytesBefore = optimizedFileResults.Sum(t => t.OriginalSize);
            var totalBytesSaved = optimizedFileResults.Where(t => t.Successful).Sum(t => t.OriginalSize - t.OptimizedSize);
            var totalBytesAfter = totalBytesBefore - totalBytesSaved;
            var percentageRemaining = Math.Round((double)totalBytesAfter / (double)totalBytesBefore * 100.0, 2);

            var timeSpan = TimeSpan.Zero;
            foreach (var duration in optimizedFileResults.Select(t => t.Duration))
            {
                timeSpan += duration;
            }

            templateText = templateText.Replace("{OptimizableFileCount}", optimizedFileResults.Count().ToString());
            templateText = templateText.Replace("{FilesOptimizedSuccessfully}", optimizedFileResults.Count(t => t.Successful && t.OriginalSize > t.OptimizedSize).ToString());
            templateText = templateText.Replace("{FilesAlreadyOptimized}", optimizedFileResults.Count(t => t.Successful && t.OriginalSize == t.OptimizedSize).ToString());
            templateText = templateText.Replace("{FilesFailedOptimization}", optimizedFileResults.Count(t => !t.Successful).ToString());
            templateText = templateText.Replace("{TotalBytesBefore}", UomHelper.BytesToString(totalBytesBefore));
            templateText = templateText.Replace("{TotalBytesAfter}", UomHelper.BytesToString(totalBytesAfter));
            templateText = templateText.Replace("{PercentageRemaining}", $"{percentageRemaining}%");
            templateText = templateText.Replace("{TotalBytesSaved}", UomHelper.BytesToString(totalBytesSaved));
            templateText = templateText.Replace("{OptimizationDuration}", UomHelper.SecondsToString((long)timeSpan.TotalSeconds));

            var optimizedFilesTable = new StringBuilder();

            optimizedFilesTable.AppendLine("FileName | Original Size | Optimized Size | Bytes Saved | Duration | Successful");
            optimizedFilesTable.AppendLine("-- | -- | -- | -- | -- | --");
            var filesToPrint = optimizedFileResults.Where(t => t.OriginalSize > t.OptimizedSize || !t.Successful).OrderByDescending(t => t.OriginalSize - t.OptimizedSize);
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

                var originalSize = UomHelper.BytesToString(fileResult.OriginalSize);
                var optimizedSize = UomHelper.BytesToString(fileResult.OptimizedSize);
                var bytesSaved = UomHelper.BytesToString(fileResult.OriginalSize - fileResult.OptimizedSize);
                optimizedFilesTable.AppendLine($"{fileName} | {originalSize} | {optimizedSize} | {bytesSaved} | {UomHelper.SecondsToString((long)fileResult.Duration.TotalSeconds)} | {fileResult.Successful}");
            }

            templateText = templateText.Replace("{OptimizedFiles}", optimizedFilesTable.ToString());

            return templateText;
        }

        public static async Task<string> GetDescriptionForCommit()
        {
            var filePath = Path.Combine(FolderHelperMethods.EntryAssemblyDirectory.Value, "CommitMarkdownTemplate.txt");
            var templateText = await Task.Run(() => File.ReadAllText(filePath));

            return templateText;
        }
    }
}
