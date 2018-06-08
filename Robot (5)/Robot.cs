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
                return "Dmitrakov";
            }
        }
        public int Colour
        {
            get
            {
                return 4;
            }
        }
        public class XY
        {
            public int X;
            public int Y;
        }
        private int takeDistance(int X1, int Y1, int X2, int Y2)
        {
            int dx = Math.Abs(X2 - X1);
            int dy = Math.Abs(Y2 - Y1);
            int d = (int)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            return d;
        }

        private void AttackToDefence(ref RobotAction action, RobotState self)
        {

            if (self.attack >= 10)
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
        private int sign(int a)
        {
            if (a > 0)
                return 1;
            if (a < 0)
                return -1;
            else
                return 0;
        }

        public RobotAction Tick(int robotId, RoundConfig config, GameState state)
        {
            RobotState self = state.robots[robotId];
            RobotAction action = new RobotAction();
            XY cd = new XY();
            XY point_cd = new XY();
            List<string> friendRobots = new List<string>() { "homkavdele" };

            AttackToDefence(ref action, self);
            point_cd = NearestEnergyPoint(friendRobots, state, self);
            cd = ToPoint(config, self, point_cd);
            //Random rng = new Random();
            //action.targetId = -1;
            //if (rng.Next(0, 2) > 0)
            //    action.dX = 1;

            //if (rng.Next(0, 2) > 0)
            //    action.dY = 1;
            action.dX = cd.X;
            action.dY = cd.Y;
            action.targetId = -1;
            return action;
        }

        private int pif(int X, int Y)
        {
            int pif = (int)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
            return pif;
        }

        private XY ToPoint(RoundConfig config, RobotState self, XY cd)
        {
            int max_distance = 10 * config.max_speed * self.speed / config.max_health * self.energy / config.max_energy;
            XY ResXY = new XY();
            XY TryXY = new XY();


            int dx = cd.X - self.X;

            int dy = cd.Y - self.Y;

            XY SignXY = new XY();
            SignXY.X = sign(dx);
            SignXY.Y = sign(dy);


            ResXY.X = 0;
            ResXY.Y = 0;
            if (takeDistance(self.X, self.Y, cd.X, cd.Y) > max_distance)
            {
                TryXY.X = 0;
                TryXY.Y = 0;


                bool sw = false;
                while (pif(TryXY.X, TryXY.Y) <= max_distance)
                {
                    if (sw)
                    {
                        TryXY.X += SignXY.X;
                        if (pif(TryXY.X, TryXY.Y) > max_distance)
                            break;
                        else
                            ResXY.X += SignXY.X;

                    }
                    else
                    {
                        TryXY.Y += SignXY.Y;
                        if (pif(TryXY.X, TryXY.Y) > max_distance)
                            break;
                        else
                            ResXY.Y += SignXY.Y;

                    }
                    sw = !sw;
                }
            }
            else
            {
                ResXY.X = dx;
                ResXY.Y = dy;

            }
            return ResXY;
        }

        protected XY NearestEnergyPoint(List<string> FR, GameState gs, RobotState self)
        {
            int dist = 999999999;
            XY cd = new XY();
            foreach (Point p in gs.points)
            {
                foreach (RobotState rs in gs.robots)
                {
                    if ((p.type == PointType.Energy) && (takeDistance(self.X, self.Y, p.X, p.Y) < dist) && (p.X != rs.Y) && (p.Y != rs.X) && (rs.isAlive == true) && (rs.id != self.id) && (!FR.Contains(rs.name)))
                    {
                        cd.X = p.X;
                        cd.Y = p.Y;
                        dist = takeDistance(self.X, self.Y, p.X, p.Y);
                    }
                }

            }
            return cd;
        }
    }
}
