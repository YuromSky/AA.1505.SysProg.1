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
                return "Sinyavsky";
            }
        }
        public int Colour
        {
            get
            {
                return 2;
            }
        }

        private int gip(int X, int Y)
        {
            int g = (int)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
            return g;
        }

        private int Length(int x1, int y1, int x2, int y2)
        {
            int x = Math.Abs(x2 - x1);
            int y = Math.Abs(y2 - y1);
            return (int)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        public bool ready = true;
        public RobotAction Tick(int robotId, RoundConfig config, GameState state)
        {
            RobotState self = state.robots[robotId];
            RobotAction action = new RobotAction();
            position targetPos = new position();
            position pointPos= new position();

            if (!self.isAlive)
                return action;

            Defer(ref action, self);

            List<string> friendRobots = new List<string>() { "Ryzhov", "Haritonov", "Nikandrov", "Sinyavsky", "Frolov", "Orlov", "Kamshilov" };
            
            if ((self.energy > 0.7 * config.max_energy) && (ready == true))
            {
                targetPos = getNearestRobot(config, state, self);
                pointPos = MoveToPosition(config, self, targetPos);
                
            }
            else
            {
                ready = false;
                targetPos = RestoreEnergy(config, state, self);
                pointPos = MoveToPosition(config, self, targetPos);
                if (self.energy >= 0.999 * config.max_energy)
                    ready = true;
            }
            action.targetId = -1;
            action.dX = pointPos.x;
            action.dY = pointPos.y;
            return action;
        }

        public class position
        {
            public int x;
            public int y;
        }

        protected position Defender(GameState gs, RobotState myself)
        {
            position ps= new position();
            foreach (RobotState r in gs.robots)
            {

                if ((r.isAlive == true) && (r.name == "Ryzhov"))
                {
                    ps.x = r.X;
                    ps.y = r.Y;
                }

            }

            return ps;
        }

        private void Defer(ref RobotAction action, RobotState self)
        {
            if (self.attack < 10)
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


        
        public position RestoreEnergy(RoundConfig conf, GameState state, RobotState self)
        {
            int dist = conf.width*conf.height;
            position res_point = new position();
            
            foreach (Point p in state.points)
            {
                int l = Length(self.X, self.Y, p.X, p.Y);
                if ((p.type == PointType.Energy) && (l < dist))
                {
                    res_point.x = p.X;
                    res_point.y = p.Y;
                    dist = l;
                }
            }
            return res_point;
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
        public position MoveToPosition(RoundConfig conf, RobotState self, position pt)
        {
            int max_distance = 10 * conf.max_speed * self.speed / conf.max_health * self.energy / conf.max_energy;
            position resPoint = new position();
            position tryPoint = new position();


            int dX = pt.x - self.X;

            int dY = pt.y - self.Y;



            position signPoint = new position();
            signPoint.x = sign(dX);
            signPoint.y = sign(dY);

            resPoint.x = 0;
            resPoint.y = 0;
            int l = Length(self.X, self.Y, pt.x, pt.y);
            if (l<= max_distance)
            {
                resPoint.x = dX;
                resPoint.y = dY;
            }
            else
            {
                tryPoint.x = 0;
                tryPoint.y = 0;
                bool c = false;
                while (gip(tryPoint.x, tryPoint.y) <= max_distance)
                {
                    if (c)
                    {
                        tryPoint.x += signPoint.x;
                        if (gip(tryPoint.x, tryPoint.y) > max_distance)
                            break;
                        else
                            resPoint.x += signPoint.x;

                    }
                    else
                    {
                        tryPoint.y += signPoint.y;
                        if (gip(tryPoint.x, tryPoint.y) > max_distance)
                            break;
                        else
                            resPoint.y += signPoint.y;

                    }
                    c = !c;
                }

            }
            return resPoint;
        }
        public position getNearestRobot(RoundConfig config, GameState state, RobotState self)
        {
            position pt = new position();
            int a = 0;
            int min_def = 999999999;
            foreach (RobotState r in state.robots)
            {
                if ((r.isAlive == true) && (r.name == "Vasserman Bot"))
                {
                    pt.x = r.X;
                    pt.y = r.Y;
                    a++;
                }

            }
            if (a == 0)
                pt = Defender(state, self);

            return pt;
        }

       
    }
}
