using DeveImageOptimizer;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebOptimizationProject
{
    public static class TemplatesHandler
    {
        public static async Task<string> GetDescriptionForPullRequest(IEnumerable<OptimizedFileResult> optimizedFileResults)
        {
            var filePath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "PullRequestMarkdownTemplate.txt");
            var templateText = await Task.Run(() => File.ReadAllText(filePath));


            var stringForCommitDetails = await GetCommitDescriptionForPullRequest(optimizedFileResults, 1);
            templateText = templateText + Environment.NewLine + Environment.NewLine + stringForCommitDetails;

            return templateText;
        }

        public static async Task<string> GetCommitDescriptionForPullRequest(IEnumerable<OptimizedFileResult> optimizedFileResults, int commitNumber)
        {
            var filePath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "CommitInPullRequestMarkdownTemplate.txt");
            var templateText = await Task.Run(() => File.ReadAllText(filePath));

            templateText = templateText.Replace("{CommitNumber}", commitNumber.ToString());
            templateText = templateText.Replace("{SupportedFileExtensions}", string.Join(" ", Constants.ValidExtensions));
            templateText = templateText.Replace("{Version}", Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString());

            var totalBytesBefore = optimizedFileResults.Sum(t => t.OriginalSize);
            var totalBytesSaved = optimizedFileResults.Where(t => t.Successful).Sum(t => t.OriginalSize - t.OptimizedSize);
            var bytesRemaining = totalBytesBefore - totalBytesSaved;
            var percentageRemaining = Math.Round((double)bytesRemaining / (double)totalBytesBefore * 100.0, 2);

            templateText = templateText.Replace("{TotalBytesBefore}", BytesToString(totalBytesBefore));
            templateText = templateText.Replace("{TotalBytesSaved}", BytesToString(totalBytesSaved));
            templateText = templateText.Replace("{PercentageRemaining}", $"{percentageRemaining}%");

            var optimizedFilesTable = new StringBuilder();

            optimizedFilesTable.AppendLine("FileName | Original Size | Optimized Size | Bytes Saved | Successful");
            optimizedFilesTable.AppendLine("-- | -- | -- | -- | --");
            foreach (var fileResult in optimizedFileResults)
            {
                var fileName = Path.GetFileName(fileResult.Path);
                var originalSize = BytesToString(fileResult.OriginalSize);
                var optimizedSize = BytesToString(fileResult.OptimizedSize);
                var bytesSaved = BytesToString(fileResult.OriginalSize - fileResult.OptimizedSize);
                optimizedFilesTable.AppendLine($"{fileName} | {originalSize} | {optimizedSize} | {bytesSaved} | {fileResult.Successful}");
            }

            templateText = templateText.Replace("{OptimizedFiles}", optimizedFilesTable.ToString());

            return templateText;
        }

        public static async Task<string> GetDescriptionForCommit()
        {
            var filePath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "CommitMarkdownTemplate.txt");
            var templateText = await Task.Run(() => File.ReadAllText(filePath));

            return templateText;
        }

        public static String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}
