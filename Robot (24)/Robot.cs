using System;
using System.Collections.Generic;
using System.Linq;
using RobotContracts;

namespace Robot
{
    public class Robot : IRobot
    {
        public string Name
        {
            get
            {
                return "Haritonov";
            }
        }

        public int Colour
        {
            get
            {
                return 2;
            }
        }

        public int battlestep = 0;
        public bool done = true;

        public RobotAction Tick(int robotId, RoundConfig config, GameState state)
        {
            RobotState selfRobot = state.robots[robotId];

            RobotAction action = new RobotAction();

            if (!selfRobot.isAlive)
            {
                return action;
            }

            List<string> friendRobots = new List<string>() { "Ryzhov", "Haritonov", "Nikandrov", "Sinyavsky", "Frolov", "Orlov", "Kamshilov" };
            decimal MIN_ENERGY_TO_GO_RELOAD = 0.85m;
            decimal MIN_HEALTH_TO_GO_RELOAD = 0.85m;
            decimal MAX_ATTACK_DISTANCE_RATIO = 30m;

            List<Point> healthPoints = state.points.Where(p => p.type == PointType.Health).ToList();
            List<Point> energyPoints = state.points.Where(p => p.type == PointType.Energy).ToList();
            bool hasNearestHealth = true;
            bool hasNearestEnergy = true;

            Point nearestHealth = null;
            decimal minDistH = 10000000m;
            int indexH = -1;
            for (int i = 0; i < healthPoints.Count; i++)
            {
                Point p = healthPoints[i];
                decimal dist = GetDistance(p.X, p.Y, selfRobot.X, selfRobot.Y);
                if (dist < minDistH)
                {
                    minDistH = dist;
                    indexH = i;
                }
            }

            if (indexH >= 0)
            {
                nearestHealth = healthPoints[indexH];
            }
            else
            {
                hasNearestHealth = false;
            }

            Point nearestEnergy = null;
            decimal minDistE = 10000000m;
            int indexE = -1;
            for (int i = 0; i < energyPoints.Count; i++)
            {
                Point p = energyPoints[i];
                decimal dist = GetDistance(p.X, p.Y, selfRobot.X, selfRobot.Y);
                if (dist < minDistE)
                {
                    minDistE = dist;
                    indexE = i;
                }
            }

            if (indexE >= 0)
            {
                nearestEnergy = energyPoints[indexE];
            }
            else
            {
                hasNearestEnergy = false;
            }

            decimal energyRest = selfRobot.energy / (decimal)config.max_energy;
            decimal healthRest = (selfRobot.speed + selfRobot.defence + selfRobot.attack) / (decimal)config.max_health;

            decimal step = GetMaxCurrentStep(selfRobot, config);
            decimal attackRadius = GetMaxCurrentAttackRadius(selfRobot, config);
            decimal attackPower = GetMaxCurrentAttackPower(selfRobot, config);

            if ((energyRest <= MIN_ENERGY_TO_GO_RELOAD && hasNearestEnergy) || (healthRest <= MIN_HEALTH_TO_GO_RELOAD && hasNearestHealth) || !done)
            {
                int dx = 0;
                int dy = 0;
                done = false;

                if (energyRest >= 0.99m || healthRest >= 0.99m)
                {
                    done = true;
                }

                if (healthRest <= MIN_HEALTH_TO_GO_RELOAD && nearestHealth != null)
                {
                    Coord deltaCoord = GetCoordXYStepToPoint(nearestHealth, selfRobot, step);

                    if (Math.Abs(selfRobot.Y - nearestHealth.Y) <= 1)
                    {
                        deltaCoord.Y = (selfRobot.Y - nearestHealth.Y) * -1;
                    }

                    if (Math.Abs(selfRobot.X - nearestHealth.X) <= 1)
                    {
                        deltaCoord.X = (selfRobot.X - nearestHealth.X) * -1;
                    }

                    bool checkStep = CheckStepFieldBorders(selfRobot, config, deltaCoord);

                    if (checkStep)
                    {
                        action.dX = deltaCoord.X;
                        action.dY = deltaCoord.Y;
                        action.targetId = state.robots[robotId > 0 ? 0 : 1].id;
                    }
                }
                else if (energyRest <= MIN_ENERGY_TO_GO_RELOAD && nearestEnergy != null)
                {
                    Coord deltaCoord = GetCoordXYStepToPoint(nearestEnergy, selfRobot, step);

                    if (Math.Abs(selfRobot.Y - nearestEnergy.Y) <= 1)
                    {
                        deltaCoord.Y = (selfRobot.Y - nearestEnergy.Y) * -1;
                    }

                    if (Math.Abs(selfRobot.X - nearestEnergy.X) <= 1)
                    {
                        deltaCoord.X = (selfRobot.X - nearestEnergy.X) * -1;
                    }

                    bool checkStep = CheckStepFieldBorders(selfRobot, config, deltaCoord);

                    if (checkStep)
                    {
                        action.dX = deltaCoord.X;
                        action.dY = deltaCoord.Y;
                        action.targetId = state.robots[robotId > 0 ? 0 : 1].id;
                    }
                }
            }
            else
            {
                decimal distToNearestAlienRobot = 10000000;
                List<RobotState> nearRobots = new List<RobotState>();
                foreach (RobotState item in state.robots)
                {
                    if (selfRobot.id != item.id && !friendRobots.Contains(item.name))
                    {
                        decimal dist = GetDistance(item.X, item.Y, selfRobot.X, selfRobot.Y);
                        if (dist <= attackRadius * MAX_ATTACK_DISTANCE_RATIO)
                        {
                            nearRobots.Add(item);
                        }
                    }
                }

                nearRobots = nearRobots.Where(r => r.isAlive == true).OrderBy(r => r.defence).ToList();

                bool foundAlienToAtack = false;

                int prevA = selfRobot.attack;
                int prevD = selfRobot.defence;

                foundAlienToAtack = CountAction(config, selfRobot, action, step, attackRadius, attackPower, nearRobots);

                if (!foundAlienToAtack)
                {
                    RobotState ar = GetNearestAlienRobotId(state, selfRobot, friendRobots);

                    Coord deltaCoord = GetCoordXYStepToPoint(new Point() { X = ar.X, Y = ar.Y }, selfRobot, step);

                    bool checkStep = CheckStepFieldBorders(selfRobot, config, deltaCoord);

                    if (checkStep)
                    {
                        action.dX = deltaCoord.X;
                        action.dY = deltaCoord.Y;
                        action.targetId = ar.id;
                    }
                    else
                    {
                        action.dX = 0;
                    }
                }
            }

            battlestep++;
            return action;
        }

        private bool CountAction(RoundConfig config, RobotState selfRobot, RobotAction action, decimal step, decimal attackRadius, decimal attackPower, List<RobotState> nearRobots)
        {
            bool foundAlienToAtack = false;

            foreach (RobotState nrb in nearRobots)
            {
                if (nrb.defence <= attackPower)
                {
                    decimal dist = GetDistance(nrb.X, nrb.Y, selfRobot.X, selfRobot.Y);
                    decimal currentStep = 0;
                    if (dist > attackRadius)
                    {
                        decimal dtoa = dist - attackRadius;
                        if (step > dtoa)
                        {
                            currentStep = step - dtoa;
                        }
                        else
                        {
                            currentStep = step;
                        }
                    }

                    if (currentStep > 0)
                    {
                        Coord deltaCoord = GetCoordXYStepToPoint(new Point() { X = nrb.X, Y = nrb.Y }, selfRobot, currentStep);

                        bool checkStep = CheckStepFieldBorders(selfRobot, config, deltaCoord);

                        decimal diff = step - currentStep;
                        if (diff > 0)
                        {
                            decimal energyPart = selfRobot.energy / (decimal)config.max_energy;
                            decimal healthPart = (config.max_radius * selfRobot.speed) / (decimal)config.max_health;
                            decimal necessaryV = (config.max_health * (attackRadius - diff)) / (decimal)(energyPart * config.max_radius);
                        }

                        if (checkStep)
                        {
                            action.dX = deltaCoord.X;
                            action.dY = deltaCoord.Y;
                            action.targetId = nrb.id;
                            foundAlienToAtack = true;
                            break;
                        }
                        else
                        {
                            action.dY = 0;
                        }
                    }
                    else
                    {
                        action.dX = 0;
                        action.dY = 0;
                        action.targetId = nrb.id;
                        foundAlienToAtack = true;
                        break;
                    }
                }
            }

            return foundAlienToAtack;
        }

        private RobotState GetNearestAlienRobotId(GameState state, RobotState selfRobot, List<string> friendRobots)
        {
            decimal distToNearestAlienRobot = 10000000;
            RobotState na = state.robots[selfRobot.id > 0 ? 0 : 1];

            foreach (RobotState item in state.robots)
            {
                if (selfRobot.id != item.id && !friendRobots.Contains(item.name) && (item.isAlive == true))
                {
                    decimal dist = GetDistance(item.X, item.Y, selfRobot.X, selfRobot.Y);
                    if (dist <= distToNearestAlienRobot)
                    {
                        na = item;
                        distToNearestAlienRobot = dist;
                    }
                }
            }
            return na;
        }

        private Coord GetCoordXYStepToPoint(Point p, RobotState robot, decimal maxStep)
        {
            Coord coord = new Coord();
            decimal dist = GetDistance(p.X, p.Y, robot.X, robot.Y);
            if (dist == 0)
            {
                return coord;
            }

            decimal sinA = (p.X - robot.X) / dist;
            decimal cosA = (p.Y - robot.Y) / dist;
            coord.X = (int)(sinA * maxStep);
            coord.Y = (int)(cosA * maxStep);

            if (coord.X == 0 && coord.Y == 0)
            {
                if (Math.Abs(sinA) < 0.5m)
                {
                    coord.Y = 1 * (int)(cosA * 1000 / Math.Abs(cosA * 1000));
                    coord.X = 0;
                }
                else if (Math.Abs(sinA) >= 0.85m)
                {
                    coord.Y = 0;
                    coord.X = 1 * (int)(sinA * 1000 / Math.Abs(sinA * 1000));
                }
                else
                {
                    coord.Y = 1 * (int)(cosA * 1000 / Math.Abs(cosA * 1000));
                    coord.X = 1 * (int)(sinA * 1000 / Math.Abs(sinA * 1000));
                }
            }

            return coord;
        }

        private bool CheckStepFieldBorders(RobotState selfRobot, RoundConfig config, Coord deltaCoord)
        {
            int newX = selfRobot.X + deltaCoord.X;
            int newY = selfRobot.Y + deltaCoord.Y;
            if (newX < config.width && newY < config.height && newX > 0 && newY > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private decimal GetMaxCurrentStep(RobotState selfRobot, RoundConfig config)
        {
            decimal energyPart = selfRobot.energy / (decimal)config.max_energy;
            decimal healthPart = (config.max_speed * selfRobot.speed) / (decimal)config.max_health;
            decimal maxStep = energyPart * healthPart;
            return maxStep;
        }

        private decimal GetMaxCurrentAttackRadius(RobotState selfRobot, RoundConfig config)
        {
            decimal energyPart = selfRobot.energy / (decimal)config.max_energy;
            decimal healthPart = (config.max_radius * selfRobot.speed) / (decimal)config.max_health;
            decimal maxRadius = energyPart * healthPart;
            return maxRadius;
        }

        private decimal GetMaxCurrentAttackPower(RobotState selfRobot, RoundConfig config)
        {
            decimal energyPart = selfRobot.energy / (decimal)config.max_energy;
            decimal maxPower = energyPart * selfRobot.attack;
            return maxPower;
        }

        private decimal GetDistance(int x1, int y1, int x2, int y2)
        {
            decimal distance = (decimal)Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
            return distance;
        }
    }

    public class Coord
    {
        public int X;
        public int Y;
    }
}
