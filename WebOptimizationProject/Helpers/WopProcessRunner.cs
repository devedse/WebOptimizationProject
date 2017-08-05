using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace WebOptimizationProject.Helpers
{
    public static class WopProcessRunner
    {
        public static Task<int> RunProcessAsync(string fileName, string arguments)
        {
            Console.WriteLine();
            Console.WriteLine($"> {Path.GetFileName(fileName)} {arguments}");

            var psi = new ProcessStartInfo(fileName, arguments);

            var tcs = new TaskCompletionSource<int>();

            var process = new Process
            {
                StartInfo = psi,
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

        public static async Task<int> RunProcessAsyncWithResults(string fileName, string arguments, List<ProcessOutputLine> output)
        {
            Console.WriteLine();
            Console.WriteLine($"> {Path.GetFileName(fileName)} {arguments}");

            var psi = new ProcessStartInfo(fileName, arguments);
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;

            var tcs = new TaskCompletionSource<int>();

            var process = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true
            };

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };

            var tcsLog = new TaskCompletionSource<int>();
            process.OutputDataReceived += (s, e) =>
            {
                lock (output)
                {
                    if (e.Data != null)
                    {
                        output.Add(new ProcessOutputLine(ProcessOutputLineType.Log, e.Data));
                    }
                    else
                    {
                        tcsLog.SetResult(0);
                    }
                }
            };

            var tcsError = new TaskCompletionSource<int>();
            process.ErrorDataReceived += (s, e) =>
            {
                lock (output)
                {
                    if (e.Data != null)
                    {
                        output.Add(new ProcessOutputLine(ProcessOutputLineType.Error, e.Data));
                    }
                    else
                    {
                        tcsError.SetResult(0);
                    }
                }
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();


            await tcsLog.Task;
            await tcsError.Task;
            return await tcs.Task;
        }
    }
}
