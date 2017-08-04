using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace WebOptimizationProject.Helpers
{
    public static class WopProcessRunner
    {
        public static Task<int> RunProcessAsync(ProcessStartInfo processStartInfo)
        {
            Console.WriteLine();
            Console.WriteLine($"> {Path.GetFileName(processStartInfo.FileName)} {processStartInfo.Arguments}");

            var tcs = new TaskCompletionSource<int>();

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };

            process.Start();

            return tcs.Task;
        }
    }
}
