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
                return "Orlov";
            }
        }
        public int Colour
        {
            get
            {
                return 1;
            }
        }

        public class coords
        {
            public int x;
            public int y;
        }

        public bool check = true;
        public RobotAction Tick(int robotId, RoundConfig config, GameState state)
        {
            RobotState self = state.robots[robotId];
            RobotAction action = new RobotAction();
            List<string> friendRobots = new List<string>() { "Ryzhov", "Haritonov", "Nikandrov", "Sinyavsky", "Frolov", "Orlov", "Kamshilov" };
            action.targetId = -1;
            Panzar(ref action, self);
            coords t_coords = new coords();
            coords r_coords = new coords();

            
            
            if ((self.energy > 0.7 * config.max_energy) && (check == true))
            {
                t_coords = TargetRobot(state, self);
                r_coords = MotionToPoint(config, self, t_coords);
               
            }
            else
            {
                check = false;
                t_coords = getCoordsOf_E_P(config, state, self);
                r_coords = MotionToPoint(config, self, t_coords);
                if (self.energy >= 0.999 * config.max_energy)
                    check = true;
            }
            action.dX = r_coords.x;
            action.dY = r_coords.y;
            
            return action;
        }


        protected void Panzar(ref RobotAction action, RobotState self)
        {
            if (self.attack <= 10)
            {
                action.dD = self.attack;
                action.dA = -self.attack;
                
                
            }
            else
            {
                
                action.dD = 10;
                action.dA = -10;
            }
        }


        protected int SqrtXY(int X, int Y)
        {
            int  s = (int)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
            return s;
        }

        protected int minDistToPoint(int x1, int y1, int x2, int y2)
        {

            return (int)Math.Sqrt(Math.Pow(Math.Abs(x2 - x1), 2) + Math.Pow(Math.Abs(y2 - y1), 2));
        }
        protected coords getCoordsOf_E_P(RoundConfig config, GameState state, RobotState self)
        {
            int dt = config.width * config.height;
            coords point = new coords();
            foreach (Point p in state.points)
            {
                if ((p.type == PointType.Energy) && (minDistToPoint(self.X, self.Y, p.X, p.Y) < dt))
                {
                    point.x = p.X;
                    point.y = p.Y;
                    dt = minDistToPoint(self.X, self.Y, p.X, p.Y);
                }
            }
            return point;
        }

        protected int sign(int a)
        {
            if (a > 0)
                return 1;
            if (a < 0)
                return -1;
            else
                return 0;
        }
        protected coords MotionToPoint(RoundConfig rc, RobotState self, coords point)
        {
            int max_distance = 10 * rc.max_speed * self.speed / rc.max_health * self.energy / rc.max_energy;
            coords final_point = new coords();
            coords try_point = new coords();
            coords coordsSign = new coords();
            final_point.x = 0;
            final_point.y = 0;


            int dX = point.x - self.X;

            int dY = point.y - self.Y;


            
            coordsSign.x = sign(dX);
            coordsSign.y = sign(dY);


            
            if (minDistToPoint(self.X, self.Y, point.x, point.y) > max_distance)
            {
                try_point.x = 0;
                try_point.y = 0;
                bool ch = false;
                while (SqrtXY(try_point.x, try_point.y) <= max_distance)
                {
                    if (ch)
                    {
                        try_point.x += coordsSign.x;
                        if (SqrtXY(try_point.x, try_point.y) > max_distance)
                            break;
                        else
                            final_point.x += coordsSign.x;

                    }
                    else
                    {
                        try_point.y += coordsSign.y;
                        if (SqrtXY(try_point.x, try_point.y) > max_distance)
                            break;
                        else
                            final_point.y += coordsSign.y;

                    }
                    ch = !ch;
                }


                
            }
            else
            {
                final_point.x = dX;
                final_point.y = dY;

            }
            return final_point;
        }
        protected coords TargetRobot(GameState gs, RobotState myself)
        {
            coords point = new coords();
            int i = 0;
            foreach (RobotState r in gs.robots)
            {
                
                if ((r.isAlive == true) && (r.name == "Gasanov Robot"))
                {
                    point.x = r.X;
                    point.y = r.Y;
                    i++;
                }

            }
            if (i == 0)
                point = ProtectRobot(gs, myself);

            return point;
        }

        protected coords ProtectRobot(GameState gs, RobotState myself)
        {
            coords point = new coords();

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
    }
}
