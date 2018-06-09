using System;
using RobotContracts;
using System.Collections.Generic;

namespace Robot
{
    public class Robot : IRobot
    {
        public string Name
        {
            get
            {
                return "Frolov";
            }
        }
        public int Colour
        {
            get
            {
                return 1;
            }
        }

        public class coordinates
        {
            public int x;
            public int y;
        }

        public bool check = true;
        public RobotAction Tick(int robotId, RoundConfig config, GameState state)
        {
            RobotState self = state.robots[robotId];
            RobotAction action = new RobotAction();
            coordinates T_cd = new coordinates();
            coordinates P_cd = new coordinates();
            
            List<string> friendRobots = new List<string>() { "Ryzhov", "Haritonov", "Nikandrov",  "Sinyavsky", "Frolov", "Orlov", "Kamshilov" };
            AttackToDefence(ref action, self);
            if ((self.energy > 0.7* config.max_energy) && (check==true)) 
            {
                T_cd = getNearestRobot(config, state, self);
                P_cd = MoveTo(config, self, T_cd);
                action.dX = P_cd.x;
                action.dY = P_cd.y;
            }
            else
            {
                check = false;
                T_cd = EnergyPoint(config, state, self);
                P_cd = MoveTo(config, self, T_cd);
                action.dX = P_cd.x;
                action.dY = P_cd.y;
                if (self.energy >= 0.999*config.max_energy)
                    check = true;
            }
            action.targetId = -1;
            return action;
        }


        public void AttackToDefence(ref RobotAction action, RobotState self)
        {
            if (self.attack > 10)
            {
                action.dA = -10;
                action.dD = 10;
            }
            else
            {
                action.dA = -self.attack;
                action.dD = self.attack;
            }
        }


        public int Pifagor(int X, int Y)
        {
            double a = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
            return (int)a;
        }

        public int Distance(int x1, int y1, int x2, int y2)
        {

            return (int)Math.Sqrt(Math.Pow(Math.Abs(x2 - x1), 2) + Math.Pow(Math.Abs(y2 - y1), 2));
        }
        public coordinates EnergyPoint(RoundConfig config, GameState state, RobotState self)
        {
            int dt = config.width * config.height;
            coordinates cd_point = new coordinates();
            foreach (Point p in state.points)
            {
                if ((p.type == PointType.Energy) && (Distance(self.X, self.Y, p.X, p.Y) < dt))
                {
                    cd_point.x = p.X;
                    cd_point.y = p.Y;
                    dt = Distance(self.X, self.Y, p.X, p.Y);
                }
            }
            return cd_point;
        }

        protected coordinates Cortege(GameState gs, RobotState myself)
        {
            coordinates point = new coordinates();

            foreach (RobotState r in gs.robots)
            {

                if ((r.isAlive == true) && (r.name == "Ryzhov"))
                {
                    point.x = r.X;
                    point.y = r.Y;
                }

            }
            return point;
        }
        public int sign(int a)
        {
            if (a > 0)
                return 1;
            if (a < 0)
                return -1;
            else
                return 0;
        }
        public coordinates MoveTo(RoundConfig rc, RobotState self, coordinates coords)
        {
            int max_distance = 10 * rc.max_speed * self.speed / rc.max_health * self.energy / rc.max_energy;
            coordinates coordsRes = new coordinates();
            coordinates coordsTry = new coordinates();


            int dX = coords.x - self.X;

            int dY = coords.y - self.Y;

            

            coordinates coordsSign = new coordinates();
            coordsSign.x = sign(dX);
            coordsSign.y = sign(dY);

            coordsRes.x = 0;
            coordsRes.y = 0;
            if (Distance(self.X, self.Y, coords.x, coords.y) <= max_distance)
            {
                coordsRes.x = dX;
                coordsRes.y = dY;
            }
            else
            {
                coordsTry.x = 0;
                coordsTry.y = 0;
                bool ch = false;
                while (Pifagor(coordsTry.x, coordsTry.y) <= max_distance)
                {
                    if (ch)
                    {
                        coordsTry.x += coordsSign.x;
                        if (Pifagor(coordsTry.x, coordsTry.y) > max_distance)
                            break;
                        else
                            coordsRes.x += coordsSign.x;

                    }
                    else
                    {
                        coordsTry.y += coordsSign.y;
                        if (Pifagor(coordsTry.x, coordsTry.y) > max_distance)
                            break;
                        else
                            coordsRes.y += coordsSign.y;

                    }
                    ch = !ch;
                }

            }
            return coordsRes;
        }
        public  coordinates getNearestRobot(RoundConfig config, GameState gs, RobotState myself)
        {
            coordinates cd = new coordinates();
            int j = 0;
            int min_def = 999999999;
            foreach (RobotState r in gs.robots)
            {
                if ((r.isAlive == true) && (r.name== "Mikhaylova"))
                {
                    cd.x = r.X;
                    cd.y = r.Y;
                    j++;
                }

            }
            if (j == 0)
                cd = Cortege(gs, myself);
            return cd;
        }

        
    }
}
