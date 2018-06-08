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
                return "Mikhaylova";
            }
        }

        public int Colour
        {
            get
            {
                return 3;
            }
        }

        public RobotAction Tick(int robotId, RoundConfig config, GameState state)
        {
            RobotState self = state.robots[robotId];
            RobotAction action = new RobotAction();
            action.targetId = -1;

            if (self.energy > 0 && self.isAlive)
            {
                int max_distance_attack = (int)Math.Round(10 * (float)config.max_radius * (float)self.speed / (float)config.max_health * (float)self.energy / (float)config.max_energy);
                int health = self.attack + self.defence + self.speed;

                int enemy_id = -1;
                if (self.energy > 0.7 * config.max_energy && health > 0.7 * config.max_health)
                {
                    for (int id = 0; id < state.robots.Count; id++)
                    {
                        RobotState rs = state.robots[id];
                        if (rs.name != self.name && rs.isAlive)
                        {
                            int enemy_distance_attack = (int)Math.Round(10 * (float)config.max_radius * (float)rs.speed / (float)config.max_health * (float)rs.energy / (float)config.max_energy);
                            int distance = CalcDistance(self.X, self.Y, rs.X, rs.Y);
                            if (distance <= enemy_distance_attack && distance <= max_distance_attack)
                            {
                                enemy_id = id;
                            }
                        }
                    }
                }

                if (enemy_id > 0)
                {
                    HealthRedestribution(self, config, action, 0.4f, 0.4f, 0.2f);

                    action.targetId = enemy_id;

                    MoveTo(robotId, config, state, action, state.robots[enemy_id].X, state.robots[enemy_id].Y);
                }
                else
                {
                    bool attack_mode = false;

                    HealthRedestribution(self, config, action, 0.0f, 0.4f, 0.6f);

                    if (self.energy < 0.95 * config.max_energy)
                    {
                        Point target = GetNearestStation(robotId, config, state, PointType.Energy);
                        MoveTo(robotId, config, state, action, target.X, target.Y);
                    }
                    else
                    {
                        if (health < 0.8 * config.max_health)
                        {
                            Point target_station = GetNearestStation(robotId, config, state, PointType.Health);

                            int X = target_station.X;
                            int Y = target_station.Y;

                            int target_robot_id = -1;

                            if (health > 0.4 * config.max_health)
                            {
                                int target_robot_dist = config.width * config.height;
                                for (int id = 0; id < state.robots.Count; id++)
                                {
                                    RobotState rs = state.robots[id];
                                    if (rs.isAlive == false && (rs.attack + rs.defence + rs.speed) > 0.4 * config.max_health)
                                    {
                                        int distance = CalcDistance(self.X, self.Y, rs.X, rs.Y);
                                        if (distance < target_robot_dist)
                                        {
                                            target_robot_id = id;
                                            target_robot_dist = distance;
                                        }
                                    }
                                }
                            }

                            if (target_robot_id >= 0)
                            {
                                RobotState taregt_robot = state.robots[target_robot_id];
                                if (CalcDistance(self.X, self.Y, X, Y) > CalcDistance(self.X, self.Y, taregt_robot.X, taregt_robot.Y))
                                {
                                    X = taregt_robot.X;
                                    Y = taregt_robot.Y;
                                }
                            }

                            MoveTo(robotId, config, state, action, X, Y);
                        }
                        else
                        {
                            int targetId = GetNearestRobot(robotId, config, state);
                            if (targetId >= 0)
                            {
                                attack_mode = true;

                                RobotState target = state.robots[targetId];
                                MoveTo(robotId, config, state, action, target.X, target.Y);

                                int distance = CalcDistance(self.X + action.dX, self.Y + action.dY, target.X, target.Y);
                                if (distance <= max_distance_attack)
                                {
                                    action.targetId = targetId;
                                }
                            }
                        }
                    }

                    if (!attack_mode)
                    {
                        int target_id = -1;
                        for (int id = 0; id < state.robots.Count; id++)
                        {
                            RobotState rs = state.robots[id];
                            if (rs.name != self.name)
                            {
                                int distance = CalcDistance(self.X + action.dX, self.Y + action.dY, rs.X, rs.Y);
                                if (distance <= max_distance_attack)
                                {
                                    target_id = id;
                                }
                            }
                        }

                        if (target_id >= 0)
                        {
                            action.targetId = target_id;
                        }
                    }
                }
            }

            return action;
        }

        private int CalcDistance(int x1, int y1, int x2, int y2)
        {
            return (int)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        private Point GetNearestStation(int robotId, RoundConfig config, GameState state, PointType type)
        {
            RobotState self = state.robots[robotId];

            int pointDistance = config.width * config.height;
            int pointId = 0;
            for (int id = 0; id < state.points.Count; id++)
            {
                Point pt = state.points[id];
                if (pt.type == type)
                {
                    int ptDistance = CalcDistance(self.X, self.Y, pt.X, pt.Y);
                    if (ptDistance < pointDistance)
                    {
                        pointDistance = ptDistance;
                        pointId = id;
                    }
                }
            }

            return state.points[pointId];
        }

        private int GetNearestRobot(int robotId, RoundConfig config, GameState state)
        {
            RobotState self = state.robots[robotId];

            int targetDistance = config.width * config.height;
            int targetId = -1;
            for (int id = 0; id < state.robots.Count; id++)
            {
                RobotState rs = state.robots[id];

                if (rs.name != self.name && rs.isAlive)
                {
                    int rsDistance = CalcDistance(self.X, self.Y, rs.X, rs.Y);
                    if (rsDistance < targetDistance)
                    {
                        targetDistance = rsDistance;
                        targetId = id;
                    }
                }
            }

            return targetId;
        }

        private void MoveTo(int robotId, RoundConfig config, GameState state, RobotAction action, int X, int Y)
        {
            RobotState self = state.robots[robotId];

            int max_distance = 10 * config.max_speed * self.speed / config.max_health * self.energy / config.max_energy;
            int distance = CalcDistance(self.X, self.Y, X, Y);
            if (max_distance > 0)
            {
                if (distance <= max_distance)
                {
                    action.dX = X - self.X;
                    action.dY = Y - self.Y;
                }
                else
                {
                    int num_steps = distance / max_distance + 1;
                    action.dX = (X - self.X) / num_steps;
                    action.dY = (Y - self.Y) / num_steps;
                }
            }
        }

        private void HealthRedestribution(RobotState self, RoundConfig config, RobotAction action, float attack, float defence, float speed)
        {
            int health = self.attack + self.defence + self.speed;
            action.dA = (int)((attack - self.attack / (float)health) * (2 * config.dHealth - 3));
            action.dD = (int)((defence - self.defence / (float)health) * (2 * config.dHealth - 3));
            action.dV = (int)((speed - self.speed / (float)health) * (2 * config.dHealth - 3));

            if (self.speed + action.dV > config.max_speed)
            {
                int delta = action.dV - (config.max_speed - self.speed);
                action.dA += delta / 2;
                action.dD += delta / 2;
                action.dV -= delta;
            }

            int sum = action.dA + action.dD + action.dV;
            if (sum != 0)
            {
                action.dD -= sum;
            }
        }
    }
}
