using System;
using System.IO;
using Newtonsoft.Json;
using RobotContracts;
using System.Text;

namespace Interface
{
    public static class ConfigLoader
    {
        public static GameConfig LoadConfig(string configPath)
        {
            string logPath = "../../log_interface.txt";

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
                File.AppendAllText(logPath, "ConfigLoader failed: " + e.ToString() + Environment.NewLine, Encoding.UTF8);
            }

            return null;
        }

        public static void SaveConfig(string configPath, GameConfig obj)
        {
            string logPath = "../../log_interface.txt";

            try
            {
                string json = JsonConvert.SerializeObject(obj);
                File.WriteAllText(configPath, json);
            }
            catch (Exception e)
            {
                File.AppendAllText(logPath, "ConfigLoader failed: " + e.ToString() + Environment.NewLine, Encoding.UTF8);
            }
        }
    }
}
