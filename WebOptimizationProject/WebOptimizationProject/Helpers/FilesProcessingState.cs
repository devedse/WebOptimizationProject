using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebOptimizationProject.Helpers
{
    class FilesProcessingState : IFilesProcessingState
    {
        public List<string> ProcessedFiles { get; set; } = new List<string>();
        public List<string> FailedFiles { get; set; } = new List<string>();

        public void AddFailedFile(string file)
        {
            FailedFiles.Add(file);
        }

        public void AddProcessedFile(string file)
        {
            ProcessedFiles.Add(file);
        }
    }
}
