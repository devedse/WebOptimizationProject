using DeveImageOptimizer.Helpers;
using Newtonsoft.Json;
using System;
using System.IO;

namespace WebOptimizationProject.Configuration
{
    public static class ConfigHelper
    {
        private const string configFileName = "Config.json";

        public static Config GetConfig()
        {
            var configPath = Path.Combine(FolderHelperMethods.EntryAssemblyDirectory.Value, configFileName);

            Config config = new Config();
            string txt = null;

            if (File.Exists(configPath))
            {
                txt = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<Config>(txt);
            }

            var serializedJson = JsonConvert.SerializeObject(config);
            if (!serializedJson.Equals(txt))
            {
                Console.WriteLine($"Updated/created config file at: {configPath}. Ensure it contains the right values.");
                File.WriteAllText(configPath, serializedJson);
            }

            return config;
        }
    }
}
