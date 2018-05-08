using System;
using System.Collections.Generic;
using RobotContracts;

namespace Engine
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string configPath = "../../config.json";
            GameConfig config = ConfigLoader.LoadConfig(configPath);
            if (config == null)
            {
                return;
            }

            Game game = new Game(config);

            int round = 0;

            string directoryPath = "../../Robots";
            List<IRobot> robots = new List<IRobot>();
            IList<IRobot> robots_base = RobotLoader.LoadRobots(directoryPath);
            
            int dN = 0;
            //if (robots != null)
            //{
            //    game.Loop(robots, round, dN);
            //    dN = 0;
            //    foreach (RobotState rs in game.future_robots)
            //    {
            //        if (rs.kill == 0)
            //        {
            //            robots.Add(robots[rs.id]);
            //            dN++;
            //        }
            //    }
            //}
            for (int i = 0; i < 5; i++)
            {
                if (robots_base != null)
                {

                    
                    dN = 0;
                    robots_base = RobotLoader.LoadRobots(directoryPath);
                    foreach (RobotState rs in game.future_robots)
                    {
                        if (rs.isAlive == true)
                        {
                            robots_base.Add(robots[rs.id]);
                            dN++;
                        }
                    }
                    robots.Clear();
                    robots.AddRange(robots_base);
                    game.Loop(robots, i, dN);
                   
                    

                  
                }
            }


        }
    }
}
