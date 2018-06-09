using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using RobotContracts;
using System.Text;

namespace Interface
{
    public static class RobotLoader
    {
        public static List<Tuple<string, IRobot>> LoadRobots(IList<string> robotFiles)
        {
            string logPath = "../../log_interface.txt";

            List<Tuple<string, Assembly>> assemblies = new List<Tuple<string, Assembly>>();
            foreach (string robotFile in robotFiles)
            {
                try
                {
                    AssemblyName name = AssemblyName.GetAssemblyName(robotFile);
                    Assembly assembly = Assembly.Load(name);
                    assemblies.Add(Tuple.Create(robotFile, assembly));
                } catch (Exception e)
                {
                    File.AppendAllText(logPath, "RobotLoader failed: " + e.ToString() + Environment.NewLine, Encoding.UTF8);
                }
            }

            Type robotType = typeof(IRobot);
            List<Tuple<string, IRobot>> robots = new List<Tuple<string, IRobot>>();
            foreach (Tuple<string, Assembly> assembly in assemblies)
            {
                if (assembly != null)
                {
                    Type[] types = assembly.Item2.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsInterface || type.IsAbstract)
                        {
                            continue;
                        }
                        else
                        {
                            if (type.GetInterface(robotType.FullName) != null)
                            {
                                IRobot robot = (IRobot)Activator.CreateInstance(type);
                                robots.Add(Tuple.Create(assembly.Item1, robot));
                            }
                        }
                    }
                }
            }

            return robots;
        }
    }
}
