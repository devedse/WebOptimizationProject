using DeveImageOptimizer.State;
using System;
using System.Threading;

namespace WebOptimizationProject.Runner
{
    public class WopProgressReporter : IProgressReporter
    {
        public void OptimizableFileProgressUpdated(OptimizableFile optimizableFile)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} File Optimized: {optimizableFile}");
        }

        public void TotalFileCountDiscovered(int count)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} Total file count: {count}");
        }
    }
}
