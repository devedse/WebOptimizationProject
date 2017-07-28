using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace WebOptimizationProject.Helpers
{
    public static class WopProcessRunner
    {
        public static Task<int> RunProcessAsync(ProcessStartInfo processStartInfo)
        {
            // there is no non-generic TaskCompletionSource
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
