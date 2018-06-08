using System;
using RobotContracts;

namespace Robot
{
    public class Robot : IRobot
    {
        public string Name
        {
            get
            {
                return "Vasserman Bot";
            }
        }

        public int Colour
        {
            get
            {
                return 1; // pink
            }
        }

        public RobotAction Tick(int robotId, RoundConfig config, GameState state)
        {
            RobotState self = state.robots[robotId];
            RobotAction action = new RobotAction();
            action.targetId = -1;

            if (self.energy > 0 && self.isAlive)
            {
                // find nearest energy station
                int pointId = 0;
                int pointDist = config.height * config.width;
                for (int ptId = 0; ptId < state.points.Count; ptId++)
                {
                    Point pt = state.points[ptId];
                    if (pt.type == PointType.Energy)
                    {
                        int ptDist = CalcDistance(self.X, self.Y, pt.X, pt.Y);
                        if (ptDist < pointDist)
                        {
                            pointDist = ptDist;
                            pointId = ptId;
                        }
                    }
                }

                Point targetPoint = state.points[pointId];

                int maxDistance = 10 * config.max_speed * self.speed / config.max_health * self.energy / config.max_energy;
                if (maxDistance > 0)
                {
                    if (pointDist <= maxDistance)
                    {
                        action.dX = targetPoint.X - self.X;
                        action.dY = targetPoint.Y - self.Y;
                    }
                    else
                    {
                        int steps = pointDist / maxDistance + 1;
                        action.dX = (targetPoint.X - self.X) / steps;
                        action.dY = (targetPoint.Y - self.Y) / steps;
                    }
                }

                // defense
                int targetId = -1;
                int targetDist = config.width * config.height;
                for (int rsId = 0; rsId < state.robots.Count; rsId++)
                {
                    RobotState rs = state.robots[rsId];
                    int rsDist = CalcDistance(self.X, self.Y, rs.X, rs.Y);
                    if (rs.name != self.name && rs.energy > 0 && rsDist < targetDist)
                    {
                        targetDist = rsDist;
                        targetId = rsId;
                    }
                }

                int maxDistanceAttack = 10 * config.max_radius * self.speed / config.max_health * self.energy / config.max_energy;
                if (targetDist <= maxDistanceAttack)
                {
                    action.targetId = targetId;
                }
            }

            return action;
        }

        private int CalcDistance(int x1, int y1, int x2, int y2)
        {
            return (int)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
    }
}
