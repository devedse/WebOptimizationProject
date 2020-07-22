using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebOptimizationProject.ImageOptimization
{
    public class WopProgressReporter : IProgressReporter
    {
        public void OptimizableFileProgressUpdated(OptimizableFile optimizableFile)
        {
            Console.WriteLine($"File optimized result: {optimizableFile}");
        }
    }
}
