//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Threading.Tasks;
//using WebOptimizationProject.Helpers.Git;

//namespace WebOptimizationProject.Helpers
//{
//    public static class WopProcessRunner
//    {
//        public static async Task<int> RunProcessAsync(string fileName, string arguments, List<EnvironmentVariable> environmentVariables = null)
//        {
//            Console.WriteLine();
//            Console.WriteLine($"> {Path.GetFileName(fileName)} {arguments}");

//            var psi = new ProcessStartInfo(fileName, arguments);

//            if (environmentVariables != null)
//            {
//                foreach (var envVariable in environmentVariables)
//                {
//                    psi.EnvironmentVariables.Add(envVariable.Key, envVariable.Value);
//                }
//            }

//            using (var process = new Process
//            {
//                StartInfo = psi
//            })
//            {

//                process.Start();

//                await Task.Run(() => process.WaitForExit());

//                return process.ExitCode;
//            }
//        }

//        public static async Task<int> RunProcessAsyncWithResults(string fileName, string arguments, List<ProcessOutputLine> output, List<EnvironmentVariable> environmentVariables = null)
//        {
//            Console.WriteLine();
//            Console.WriteLine($"> {Path.GetFileName(fileName)} {arguments}");

//            var psi = new ProcessStartInfo(fileName, arguments);
//            psi.UseShellExecute = false;
//            psi.RedirectStandardError = true;
//            psi.RedirectStandardOutput = true;

//            if (environmentVariables != null)
//            {
//                foreach (var envVariable in environmentVariables)
//                {
//                    psi.EnvironmentVariables.Add(envVariable.Key, envVariable.Value);
//                }
//            }

//            using (var process = new Process
//            {
//                StartInfo = psi,
//                EnableRaisingEvents = true
//            })
//            {

//                process.OutputDataReceived += (s, e) =>
//                {
//                    lock (output)
//                    {
//                        if (e.Data != null)
//                        {
//                            output.Add(new ProcessOutputLine(ProcessOutputLineType.Log, e.Data));
//                            Console.WriteLine(e.Data);
//                        }
//                    }
//                };

//                process.ErrorDataReceived += (s, e) =>
//                {
//                    lock (output)
//                    {
//                        if (e.Data != null)
//                        {
//                            output.Add(new ProcessOutputLine(ProcessOutputLineType.Error, e.Data));
//                            Console.Error.WriteLine(e.Data);
//                        }
//                    }
//                };

//                process.Start();

//                process.BeginOutputReadLine();
//                process.BeginErrorReadLine();

//                await Task.Run(() => process.WaitForExit());

//                return process.ExitCode;
//            }
//        }
//    }
//}
