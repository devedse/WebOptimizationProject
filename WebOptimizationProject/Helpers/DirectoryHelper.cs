using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WebOptimizationProject.Helpers
{
    public static class DirectoryHelper
    {
        public static async Task DeleteDirectory(string dir)
        {
            if (Directory.Exists(dir))
            {
                await Task.Run(() =>
                {
                    Directory.Delete(dir, true);
                    Console.WriteLine("NU1");
                });
            }
        }
    }
}
