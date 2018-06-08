using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using RobotContracts;

namespace Interface
{
    public class Game
    {
        string logpath;
        public GameConfig config;
        public GameState state;
        public List<RobotState> future_robots = new List<RobotState>();
        delegate int del();

        public Game(GameConfig config)
        {
            this.config = config;
            state = new GameState();

            SingleRandom.Seed(config.seed);
        }

        private void UpdateState(RoundConfig round_config, int id, RobotAction action)
        {
            RobotState robot = state.robots[id].ShallowCopy();
            future_robots.Add(robot);
            if(!robot.isAlive)
            {
                return;
            }
            if (action == null)
            {
                File.AppendAllText(logpath, robot.id + " timed out" + Environment.NewLine, Encoding.UTF8);
                robot.energy -= round_config.dEs;
                return;
            }

            del GetHealth = () => { return robot.attack + robot.defence + robot.speed; };

            // charging stations
            foreach (Point point in state.points)
            {
                if ((robot.X == point.X) && (robot.Y == point.Y))
                {
                    switch (point.type)
                    {
                        case PointType.Energy:
                            robot.energy = (robot.energy + round_config.dE <= round_config.max_energy) ? (robot.energy + round_config.dE) : round_config.max_energy;
                            File.AppendAllText(logpath, "Added robot energy on point. id robot:" + robot.id + " point:(" + point.X + ", " + point.Y + "), energy:" + robot.energy + Environment.NewLine, Encoding.UTF8);
                            break;
                        case PointType.Health:
                            int health = GetHealth();
                            if (health < round_config.max_health)
                            {
                                int dAt = (robot.attack == 0)? 1 : (int)(((float)robot.attack / (float)health) * round_config.dHealth);
                                int dDf = (robot.defence == 0) ? 1: (int)(((float)robot.defence / (float)health) * round_config.dHealth);
                                int dSp = (robot.speed == 0) ? 1 : (int)(((float)robot.speed / (float)health) * round_config.dHealth);
                                robot.attack += (dAt + GetHealth() < round_config.max_health) ? dAt: round_config.max_health - GetHealth();
                                robot.defence += (dDf + GetHealth() < round_config.max_health) ? dDf : round_config.max_health - GetHealth(); 
                                if (robot.speed < round_config.max_speed)
                                    robot.speed += (dSp + GetHealth() < round_config.max_health) ? dSp : round_config.max_health - GetHealth();
                                File.AppendAllText(logpath, "Added robot health on point. id robot:" + robot.id + " point:(" + point.X + ", " + point.Y + "), health:" + GetHealth() + Environment.NewLine, Encoding.UTF8);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            // drain energy from dead robots
            foreach (RobotState donor in state.robots)
            {
                if ((robot.id != donor.id) && (robot.X == donor.X) && (robot.Y == donor.Y) && !donor.isAlive)
                {
                    int health = GetHealth();
                    if (health < round_config.max_health)
                    {
                        int donor_health = donor.attack + donor.defence + donor.speed;
                        if (donor.attack >= 0)
                        {
                            robot.attack += robot.attack / health * round_config.dHealth;
                            donor.attack -= donor.attack / donor_health * round_config.dHealth;
                        }

                        if (donor.defence >= 0)
                        {
                            robot.defence += robot.defence / health * round_config.dHealth;
                            donor.defence -= donor.defence / donor_health * round_config.dHealth;
                        }

                        if (donor.speed >= 0 && robot.speed <= round_config.max_speed)
                        {
                            robot.speed += robot.speed / health * round_config.dHealth;
                            donor.speed -= donor.speed / donor_health * round_config.dHealth;
                        }
                        File.AppendAllText(logpath, "Drain health from dead robots. id robot:" + robot.id + " point:(" + robot.X + ", " + robot.Y + "), id dead robot: " + donor.id + ", health:" + GetHealth() + Environment.NewLine, Encoding.UTF8);
                        break;
                    }

                }
            }

            // movement
            int distance = (int)Math.Sqrt(Math.Pow(action.dX, 2) + Math.Pow(action.dY, 2));
            int max_distance = 10 * round_config.max_speed * robot.speed / round_config.max_health * robot.energy / round_config.max_energy;

            if ((distance > 0) && (distance <= max_distance))
            {
                robot.X = Math.Max(Math.Min(robot.X + action.dX, round_config.width - 1), 0);
                robot.Y = Math.Max(Math.Min(robot.Y + action.dY, round_config.height - 1), 0);
                robot.energy -= round_config.dEv;
                File.AppendAllText(logpath, "Movement. id robot:" + robot.id + " point:(" + robot.X + ", " + robot.Y + ") , energy:" + robot.energy + Environment.NewLine, Encoding.UTF8);
            }
            else
            {
                robot.energy -= round_config.dEs;
                File.AppendAllText(logpath, "Did not movement. id robot:" + robot.id + " point:(" + robot.X + ", " + robot.Y + ") , energy:" + robot.energy + Environment.NewLine, Encoding.UTF8);
            }

            // attack and defence
            if (action.targetId != -1)
            {
                RobotState target_robot = null;
                if (action.targetId < robot.id)
                {
                    target_robot = future_robots[action.targetId];
                }
                else
                {
                    target_robot = state.robots[action.targetId];
                }
                

                int distance_attack = (int)Math.Sqrt(Math.Pow((target_robot.X - robot.X), 2) + Math.Pow((target_robot.Y - robot.Y), 2));
                int max_distance_attack = 10 * round_config.max_radius * robot.speed / round_config.max_health * robot.energy / round_config.max_energy;

                int real_power = SingleRandom.Instance.Next((int)(round_config.minRND), (int)(round_config.maxRND)) * robot.attack / 10;
                int max_power = real_power * robot.energy / round_config.max_energy;

                int real_target_defence = ((10 - SingleRandom.Instance.Next((int)(round_config.minRND), (int)(round_config.maxRND))) * target_robot.defence) / 10;
                int max_target_defence = real_target_defence * robot.energy / round_config.max_energy;

                if ((distance_attack <= max_distance_attack) && (target_robot.energy > 0))
                {
                    if (max_power >= max_target_defence)
                    {
                        if (target_robot.defence <= 0)
                        {
                            target_robot.defence = 0;
                            target_robot.energy -= (int)(max_power - max_target_defence) * round_config.max_energy / round_config.max_health;
                        }
                        else
                        {
                            target_robot.defence -= (int)(max_power - max_target_defence);
                        }
                        File.AppendAllText(logpath, "Attack robot. id robot:" + robot.id + " point:(" + robot.X + ", " + robot.Y + "), id target robot: " + target_robot.id + ", attack:" + (int)(max_power - max_target_defence) + Environment.NewLine, Encoding.UTF8);
                    }
                    else
                    {
                        if (robot.attack <= 0)
                        {
                            robot.attack = 0;
                            robot.energy -= (int)(max_target_defence - max_power) * round_config.max_energy / round_config.max_health;
                        }
                        else
                        {
                            robot.attack -= (int)(max_target_defence - max_power);
                        }
                        File.AppendAllText(logpath, "Defence target robot. id robot:" + robot.id + " point:(" + robot.X + ", " + robot.Y + "), id target robot: " + target_robot.id + ", defence:" + (int)(max_power - max_target_defence) + Environment.NewLine, Encoding.UTF8);
                    }
                    robot.energy -= round_config.dEa;
                    target_robot.energy -= round_config.dEd;
                }

                if ((target_robot.energy <= 0)&&(target_robot.defence == 0))
                {
                    bool flag = true;
                    foreach (RobotState rb in state.robots)
                    {
                        for (int i = 0; i < rb.kill; i++)
                        {
                            if (target_robot.id == rb.kill_id[i])
                            {
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        target_robot.isAlive = false;
                        robot.kill++;
                        robot.kill_id[robot.kill - 1] = target_robot.id;
                        File.AppendAllText(logpath, "Dead target robot. id robot:" + robot.id + " point:(" + robot.X + ", " + robot.Y + "), id target robot: " + target_robot.id + Environment.NewLine, Encoding.UTF8);

                    }
                }

                target_robot.defence = (target_robot.defence < 0) ? 0 : target_robot.defence;
                target_robot.energy = (target_robot.energy < 0) ? 0 : target_robot.energy;
            }

            robot.defence = (robot.defence < 0) ? 0 : robot.defence;
            robot.attack = (robot.attack < 0) ? 0 : robot.attack;
            robot.speed = (robot.speed < 0) ? 0 : robot.speed;


            if (robot.energy <= 0)
            {
                robot.isAlive = false;
                File.AppendAllText(logpath, "Dead robot. id robot:" + robot.id + " point:(" + robot.X + ", " + robot.Y + ")" + Environment.NewLine, Encoding.UTF8);

            }

            // +
            if ((Math.Abs(action.dA) + Math.Abs(action.dD) + Math.Abs(action.dV) != 0) && (Math.Abs(action.dA) + Math.Abs(action.dD) + Math.Abs(action.dV) <= 2 * round_config.dHealth) && (robot.speed + action.dV <= round_config.max_speed) && (Math.Abs(action.dA + action.dD + action.dV) <= 0.0000001))
            {
                robot.speed += action.dV;
                robot.attack += action.dA;
                robot.defence += action.dD;
                File.AppendAllText(logpath, "Resource movement. id robot:" + robot.id + " point:(" + robot.X + ", " + robot.Y + "), health " + GetHealth() + Environment.NewLine, Encoding.UTF8);
            }


            robot.energy = (robot.energy > round_config.max_energy) ? round_config.max_energy : robot.energy;
        }

        public void Loop(IList<IRobot> robots, int round, int dN)
        {
            logpath = "../../log{" + round + "}.txt";
            File.WriteAllText(logpath, "round: " + round + Environment.NewLine, Encoding.UTF8);
            Console.WriteLine("rounds: " + round);
            RoundConfig round_config = config.rounds[round];
            state.robots.Clear();

            int j;
            for (j = 0; j < robots.Count - dN; j++)
            {
                RobotState robot = new RobotState
                {
                    id = j,
                    X = SingleRandom.Instance.Next(0, round_config.width),
                    Y = SingleRandom.Instance.Next(0, round_config.height),
                    energy = round_config.max_energy,
                    attack = 45,
                    speed = 10,
                    defence = 45,
                    isAlive = true,
                    kill = 0,
                    kill_id = new int[robots.Count],
                    name = robots[j].Name, // попросили добавить чтобы кооперироваться
                    colour = robots[j].Colour // попросили добавить чтобы дебажить
                };
                state.robots.Add(robot);
            }

            foreach (RobotState rs in future_robots)
            {
                if (rs.isAlive)
                {
                    RobotState rss = rs.ShallowCopy();
                    rss.id = j++;
                    for (int kkk = 0; kkk < rss.kill; kkk++)
                    {
                        rss.kill_id[kkk] = 0;
                    }
                    rss.kill = 0;
                    state.robots.Add(rss);
                }
            }

            state.points.Clear();
            for (int i = 0; i < round_config.nEnergy; i++)
            {
                Point point = new Point
                {
                    X = SingleRandom.Instance.Next(0, round_config.width),
                    Y = SingleRandom.Instance.Next(0, round_config.height),
                    type = PointType.Energy
                };
                state.points.Add(point);
            }

            for (int i = 0; i < round_config.nHealth; i++)
            {
                Point point = new Point
                {
                    X = SingleRandom.Instance.Next(0, round_config.width),
                    Y = SingleRandom.Instance.Next(0, round_config.height),
                    type = PointType.Health
                };
                state.points.Add(point);
            }


            for (int n = 0; n < round_config.steps; n++)
            {
                future_robots.Clear();
                File.AppendAllText(logpath, "step: " + n + Environment.NewLine, Encoding.UTF8);

                for (int i = 0; i < robots.Count; i++)
                {
                    IRobot robot = robots[i];
                    Task<RobotAction> task = Task.Run(() => robot.Tick(i, round_config, state.DeepCopy()));

                    try
                    {
                        UpdateState(round_config, i, task.Wait(round_config.timeout) ? task.Result : null);
                    }
                    catch (Exception e)
                    {
                        if (task.IsFaulted)
                        {
                            File.AppendAllText(logpath, "robot \"" + robot.Name + "\" failed" + Environment.NewLine, Encoding.UTF8);
                            Console.WriteLine("robot \"" + robot.Name + "\" failed");
                        }
                        else
                        {
                            File.AppendAllText(logpath, "oh no" + Environment.NewLine, Encoding.UTF8);
                            Console.WriteLine("oh no");
                        }
                        File.AppendAllText(logpath, e.Message + Environment.NewLine, Encoding.UTF8);
                        Console.WriteLine(e.Message);
                        robots.RemoveAt(i);
                        continue;
                    }

                    Console.WriteLine("{0}: {1}x{2}; health: {3}; energy: {4}", state.robots[i].id, state.robots[i].X, state.robots[i].Y, state.robots[i].attack + state.robots[i].defence + state.robots[i].speed, state.robots[i].energy);
                }

                if (n < round_config.steps - 1)
                {
                    state.robots.Clear();
                    foreach (RobotState rs in future_robots)
                    {
                        state.robots.Add(rs);
                    }
                }
                if (n == round_config.steps -1)
                {
                    for (int k = 0; k < future_robots.Count; k++)
                    {
                        if (future_robots[k].isAlive)
                        {
                            future_robots[k].energy += round_config.K * future_robots[k].kill;
                        }
                        else
                        {
                            future_robots[k].energy += round_config.K * future_robots[k].kill / 2;
                        }
                      
                    }
                }

                Draw.drawing(state.robots, state.points, round_config.width, round_config.height);

                for (int i = 0; i < future_robots.Count; i++)
                {
                    if (future_robots[i].energy < 0)
                    {
                        future_robots[i].energy = 0;
                    }
                    //if (future_robots[i].energy > 1000)
                    //{
                    //    future_robots[i].energy = 1000;
                    //}
                    if (future_robots[i].defence + future_robots[i].attack + future_robots[i].speed < 0)
                    {
                        future_robots[i].defence = 0;
                        future_robots[i].attack = 0;
                        future_robots[i].speed = 0;
                    }
                    //if (future_robots[i].energy > 1000)
                    //{
                    //    future_robots[i].energy = 1000;
                    //}
                    if (i % 2 == 0)
                    {
                        int energyValue = future_robots[i].energy / 10;
                        //Form1.pbList[i].Value = energyValue;
                    }
                    else
                    {
                        //Form1.pbList[i].Value = future_robots[i].defence + future_robots[i].attack + future_robots[i].speed;
                    }
                }
                
            }
        }
    }
}
