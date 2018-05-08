using System;
using System.IO;
using Newtonsoft.Json;
using RobotContracts;

namespace Engine
{
    public static class ConfigLoader
    {
        public static GameConfig LoadConfig(string configPath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(configPath))
                {
                    string json = sr.ReadToEnd();
                    return JsonConvert.DeserializeObject<GameConfig>(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ConfigLoader failed: {0}", e.ToString());
            }

            return null;
        }
        public static void SaveConfig(string configPath, GameConfig obj)
        {
            try
            {
                string json = JsonConvert.SerializeObject(obj);
                File.WriteAllText(configPath, json);
            }
            catch (Exception e)
            {
                Console.WriteLine("ConfigLoader failed: {0}", e.ToString());
            }
        }
    }
}
