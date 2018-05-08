using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using RobotContracts;

namespace Interface
{
    public static class RobotLoader
    {
        public static IList<IRobot> LoadRobots(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return null;
            }

            string[] robotFileNames = Directory.GetFiles(directoryPath, "*.dll");
            IList<Assembly> assemblies = new List<Assembly>(robotFileNames.Length);
            foreach (string robotFile in robotFileNames)
            {
                AssemblyName name = AssemblyName.GetAssemblyName(robotFile);
                Assembly assembly = Assembly.Load(name);
                assemblies.Add(assembly);
            }

            Type robotType = typeof(IRobot);
            IList<IRobot> robots = new List<IRobot>();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly != null)
                {
                    Type[] types = assembly.GetTypes();
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
                                robots.Add(robot);
                            }
                        }
                    }
                }
            }

            return robots;
        }
    }
}
