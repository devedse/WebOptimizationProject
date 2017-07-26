using DeveImageOptimizer.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebOptimizationProject.Configuration
{
    public static class ConfigHelper
    {
        private const string configFileName = "Config.json";

        public static Config GetConfig()
        {
            var configPath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, configFileName);

            if (!File.Exists(configPath))
            {
                var serializedJson = JsonConvert.SerializeObject(new Config());
                File.WriteAllText(configPath, serializedJson);

                Console.WriteLine($"Created new config file at: {configPath}. Ensure it contains the right values.");
            }
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
        }
    }
}
