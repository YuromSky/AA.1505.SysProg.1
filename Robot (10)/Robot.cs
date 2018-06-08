using System;
using System.Windows.Forms;
using RobotContracts;

namespace Robot
{
    public class Robot : IRobot
    {
        public string Name
        {
            get
            {
                return "dimkuc";
            }
        }
        public int Colour
        {
            get
            {
                return 2;
            }
        }
        
        public class position
        {
            public int x;
            public int y;
        }

        public int getDistance(int x1, int y1, int x2, int y2)
        {
            return (int)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        public position getNearestChargingStation(GameState state, RobotState self)
        {
            position PointCoords = new position();
            int MinDistance = 99;
            foreach (Point p in state.points)
            {
                int min_dist = getDistance(self.X, self.Y, p.X, p.Y);
                if (p.type == PointType.Health && (min_dist < MinDistance))
                {
                    MinDistance = min_dist;
                    PointCoords.x = p.X;
                    PointCoords.y = p.Y;
                }
            }
            return PointCoords;
        }
        
        public position getNearestServiceStation(GameState state, RobotState self)
        {

            position PointCoords = new position();
            int MinDistance = 99;
            foreach (Point p in state.points)
            {
                int min_dist = getDistance(self.X, self.Y, p.X, p.Y);
                if (p.type == PointType.Energy && (min_dist < MinDistance))
                {
                    MinDistance = min_dist;
                    PointCoords.x = p.X;
                    PointCoords.y = p.Y;
                }
            }
            return PointCoords;
        }

        int pifagor(position vect)
        {
            return (int)Math.Sqrt(vect.x * vect.x + vect.y * vect.y);
        }

        public void MoveTo(position target, RobotState self, RoundConfig config, RobotAction action)
        {
            position pos = new position();
            pos.x = self.X;
            pos.y = self.Y;
            int max_move_dist = 10* config.max_speed * self.speed * self.energy / (config.max_energy * config.max_health);
            int dx1 = target.x - pos.x;
            int dy1 = target.y - pos.y;

            int dist = (int)Math.Sqrt((Math.Abs(dx1)) * (Math.Abs(dx1)) + (Math.Abs(dy1)) * (Math.Abs(dy1)));

            if (dist <= max_move_dist)
            {
                action.dX = target.x - self.X;
                action.dY = target.y - self.Y;
            }
            else
            {
                if(max_move_dist != 0)
                {
                    int count = dist / max_move_dist + 1;
                    action.dX = (target.x - self.X) / count;
                    action.dY = (target.y - self.Y) / count;
                }
            }
        }


        public RobotAction Tick(int robotId, RoundConfig config, GameState state)
        {
            RobotState self = state.robots[robotId];
            RobotAction action = new RobotAction();
            action.targetId = -1;
            position mine = new position();
            position charging_station = new position();
            position service_station = new position();
            charging_station = getNearestChargingStation(state, self);
            service_station = getNearestServiceStation(state, self);
            mine.x = self.X;
            mine.y = self.Y;
            HealthExchange(self, config, action, 0.0f, 0.4f, 0.6f);
            if (mine.x == charging_station.x
                && mine.y == charging_station.y
                && (self.attack + self.defence + self.speed) < 0.95 * config.max_health)
            {

                return action;
            }
            if (mine.x == service_station.x
                && mine.y == service_station.y
                && self.energy < 0.99 * config.max_energy)
            {
                return action;
            }
            //или пополняем энергию
          
            //MoveTo(charging_station, self, config, action);
            if (self.energy <= 0.8 * config.max_energy)
            {
                HealthExchange(self, config, action, 0.0f, 0.2f, 0.8f);
                MoveTo(getNearestServiceStation(state, self), self, config, action);               
                return action;
            }
            else
            {
                //или пополняем техн. состояние
                int service_sost = self.attack + self.defence + self.speed;
                if (service_sost <= 0.9 * config.max_health)
                {
                    MoveTo(getNearestChargingStation(state, self), self, config, action);
                    //записываем информацию о движении
                    return action;
                }
                else
                {
                    int attack_range = (int)(config.max_radius) * 10 * self.speed * self.energy /
                        (config.max_energy * config.max_health);
                    HealthExchange(self, config, action, 0.4f, 0.4f, 0.2f);
                    //или ищем цель  для удара и защиты
                    int enemy_id = -1;
                    bool under_attack = false;
                    for (int id = 0; id < state.robots.Count; id++)
                    {
                        RobotState rs = state.robots[id];
                        if (rs.name != self.name)
                        {
                            int enemy_distance_attack = 10 * config.max_radius * rs.speed * rs.energy / config.max_health / config.max_energy;
                            int distance = getDistance(self.X, self.Y, rs.X, rs.Y);
                            if (distance <= enemy_distance_attack && distance <= attack_range)
                            {
                                under_attack = true;
                                enemy_id = id;
                            }
                        }
                    }


                    int target_id = GetNearestTarget(robotId,config,state,true,true);
                    HealthExchange(self, config, action, 0.4f, 0.4f, 0.2f);
                    if(target_id >=0)
                    {
                        RobotState target = state.robots[target_id];
                        position target_pos = new position();
                        target_pos.x = target.X;
                        target_pos.y = target.Y;
                        MoveTo(target_pos, self,config, action);
                        position distance = new position();
                        distance.x = self.X + action.dX;
                        distance.y = self.Y + action.dY;

                        int dist = getDistance(self.X + action.dX, self.Y + action.dY, target_pos.x, target_pos.y);
                        if (dist <= attack_range)
                        {
                            action.targetId = target_id;

                        }
                    }
                }
            }
            return action;
        }

        public void HealthExchange(RobotState self, RoundConfig config, RobotAction action, float attack, float defence, float speed)
        {
            int health = self.attack + self.defence + self.speed;
            action.dA = (int)((attack - self.attack / (float)health) * (config.dHealth - 3));
            action.dD = (int)((defence - self.defence / (float)health) * (config.dHealth - 3));
            action.dV = (int)((speed - self.speed / (float)health) * (config.dHealth - 3));

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
                action.dV -= sum;
            }
        }

        private int GetNearestTarget(int robotId, RoundConfig config, GameState state, bool enemyOnly, bool aliveOnly)
        {
            RobotState self = state.robots[robotId];

            int targetDistance = config.width * config.height;
            int targetId = -1;
            for (int id = 0; id < state.robots.Count; id++)
            {
                RobotState rs = state.robots[id];

                if (!enemyOnly || rs.name != self.name)
                {
                    if (!aliveOnly || rs.isAlive)
                    {
                        int rsDistance = getDistance(self.X, self.Y, rs.X, rs.Y);
                        if (rsDistance < targetDistance)
                        {
                            targetDistance = rsDistance;
                            targetId = id;
                        }
                    }
                }
            }
            return targetId;
        }
    }
}