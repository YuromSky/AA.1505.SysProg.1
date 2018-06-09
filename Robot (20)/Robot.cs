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
                return "Sinitcina";
            }
        }
        public int Colour
        {
            get
            {
                return 3;
            }
        }
        class coord
        {
            public int x;
            public int y;
        };



        int distanceToEnemy(RobotState self, int x, int y)
        {
                
            int deltax = Math.Abs(x - self.X);
            int deltay = Math.Abs(y - self.Y);
            int result = (int)Math.Sqrt(Math.Pow(deltax, 2) + Math.Pow(deltay, 2));
            return result;
        }

        int getTargetWithCondition(RoundConfig round_config, RobotState self, GameState state)
        {
            int Person_Id = -1;
            int minimal_distance = 100;
            foreach(RobotState robot in state.robots)
            {
                int distance_to_enemy = distanceToEnemy(self, robot.X, robot.Y);
                if (distance_to_enemy <= minimal_distance && robot.id != self.id)
                {
                    int max_power = self.attack * self.energy / round_config.max_energy;

                    int max_target_defence = (int)Math.Round((float)robot.defence * (float)robot.energy / (float)round_config.max_energy);

                    if (max_target_defence <= max_power)
                    {
                        Person_Id = robot.id;
                        minimal_distance = distance_to_enemy;
                    }

                }

            }
            return Person_Id;
        }

        coord findOutNearStatOfCharge(PointType type, RobotState self, GameState state)
        {
            coord result_point = new coord();
            int minimal_distance = 100;

            foreach (Point point in state.points)
            {
                if(point.type == type)
                {
                    int dist = distanceToEnemy(self, point.X, point.Y);
                    if (dist < minimal_distance)
                    {
                        result_point.x = point.X;
                        result_point.y = point.Y;
                        minimal_distance = dist;
                    }
                }
                
            }
            return result_point;
        }

        coord Movement(RoundConfig round_config, coord our_target, RobotState self, GameState state)
        {
            int maximal_move_distance = 10 * round_config.max_speed * self.speed / round_config.max_health * self.energy / round_config.max_energy;
            int itog_distance = distanceToEnemy(self, our_target.x, our_target.y);
            coord move = new coord();
            if (maximal_move_distance > 0)
            {
                if (itog_distance <= maximal_move_distance)
                {
                    move.x = our_target.x - self.X;
                    move.y = our_target.y - self.Y;
                }
                else
                {
                    int steps = itog_distance / maximal_move_distance + 1;
                    move.x = (our_target.x - self.X) / steps;
                    move.y = (our_target.y - self.Y) / steps;
                }
            }
            return move;
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


        public RobotAction Tick(int robotId, RoundConfig config, GameState state)
        {
            RobotState self = state.robots[robotId];
            RobotAction action = new RobotAction();
            int health = self.attack + self.defence + self.speed;
            coord healt = findOutNearStatOfCharge(PointType.Health, self, state);
            coord energy = findOutNearStatOfCharge(PointType.Energy, self, state);
            coord move = new coord();
            action.targetId = -1;
            if (self.attack > 30)
            {
                action.dA = -10;
                action.dD = 10;
                action.dV = 0;
            }
            if(self.attack >= 0)
            {
                action.targetId = getTargetWithCondition(config, self, state);
            }
            if(self.defence < 20)
            {
                HealthRedestribution(self, config, action, self.attack, self.defence, self.speed);
            }
            if (health < 50)
            {
                move = Movement(config, healt, self, state);
            }
            else
            {
                move = Movement(config, energy, self, state);
            }
            action.dX = move.x;
            action.dY = move.y;
            return action;
        }
    }
}
