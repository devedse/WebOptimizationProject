//using DeveImageOptimizer.Helpers;
//using Newtonsoft.Json;
//using System;
//using System.IO;

//namespace WebOptimizationProject.Configuration
//{
//    public static class ConfigHelper
//    {
//        private const string ConfigFileName = "Config.json";

//        public static WopConfig GetConfig()
//        {
//            var configPath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, ConfigFileName);

//            WopConfig config = new WopConfig();
//            string txt = null;

//            if (File.Exists(configPath))
//            {
//                txt = File.ReadAllText(configPath);
//                config = JsonConvert.DeserializeObject<WopConfig>(txt);
//            }

//            var serializedJson = JsonConvert.SerializeObject(config);
//            if (!serializedJson.Equals(txt))
//            {
//                Console.WriteLine($"Updated/created config file at: {configPath}. Ensure it contains the right values.");
//                File.WriteAllText(configPath, serializedJson);
//            }

//            return config;
//        }
//    }
//}
